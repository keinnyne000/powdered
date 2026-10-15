Shader "Hidden/OutlineV1"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1, 0, 1, 1)
        _OutlineThickness("Outline Thickness", Float) = 1.0
        _DepthThreshold("Depth Threshold", Float) = 0.01
        _NormalThreshold("Normal Threshold", Float) = 0.2
        _ColorThreshold("Color Threshold", Float) = 0.2
    }
    
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_LinearClamp2);
            
            static const float sobelX[3][3] = {{-1, 0, 1}, {-2, 0, 2}, {-1, 0, 1}};
            static const float sobelY[3][3] = {{1, 2, 1}, {0, 0, 0}, {-1, -2, -1}};
            
            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineThickness;
                float _DepthThreshold;
                float _NormalThreshold;
                float _ColorThreshold;
            CBUFFER_END
            
            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                output.uv = GetFullScreenTriangleTexCoord(input.vertexID);
                return output;
            }
            
            half4 Frag(Varyings input) : SV_Target
            {
                float2 texelSize = _OutlineThickness / _ScreenParams.xy;
                
                float2 uv = input.uv;
                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp2, uv);
                float3 normalVS = SampleSceneNormals(uv);
                float rawDepth = SampleSceneDepth(uv);
                float eyeDepth = Linear01Depth(rawDepth, _ZBufferParams);

                float dx = 0;
                float dy = 0;
            
                float3 nx = 0;
                float3 ny = 0;

                float3 cx = 0;
                float3 cy = 0;
                
                for (int y = -1; y <= 1; y++)
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 sampleUVs = uv + float2(x, y) * texelSize;

                        float rawD = SampleSceneDepth(sampleUVs);
                        float d = Linear01Depth(rawD, _ZBufferParams);
                        dx += d * sobelX[x + 1][y + 1];
                        dy += d * sobelY[x + 1][y + 1];
                        
                        float3 n = SampleSceneNormals(sampleUVs);
                        nx += sobelX[x + 1][y + 1] * n;
                        ny += sobelY[x + 1][y + 1] * n;

                        float3 c = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp2, sampleUVs);
                        cx += c * sobelX[x + 1][y + 1];
                        cy += c * sobelY[x + 1][y + 1];
                    }

                float depthEdge = length(float2(dx, dy));
                depthEdge = step(_DepthThreshold, depthEdge);
                
                float normalEdge = length(nx) + length(ny);
                normalEdge = step(_NormalThreshold, normalEdge);

                float colorEdge = length(cx) + length(cy);
                colorEdge = step(_ColorThreshold, colorEdge);

                float edge = max(depthEdge, normalEdge);
                edge = max(edge, colorEdge);

                return lerp(color, _OutlineColor, edge);
            }
            ENDHLSL
        }
    }
}

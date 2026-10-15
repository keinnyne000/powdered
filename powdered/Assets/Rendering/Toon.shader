Shader "Custom/ToonMatV1Shader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.3, 0.3, 0.3, 1)
        _XTex("X Texture", 2D) = "white" {}
        _YTex("Y Texture", 2D) = "white" {}
        _ZTex("Z Texture", 2D) = "white" {}
        _TexScale("Tex Scale", Float) = 1
        _ShadowHardness("Shadow Hardness", Range(0, 1)) = 0.8
        _LightEdge("Light Edge", Float) = 0.4
        _DarkEdge("Dark Edge", Float) = 0.9
        _ShadowDivisions("Shadow Divisions", Integer) = 3
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowBlur("Shadow Blur", Float) = 1.0
    }

    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "Forward"
            Tags {"LightMode" = "UniversalForward"}
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionOS : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 normalOS : TEXCOORD2;
            };
            
            TEXTURE2D(_XTex);
            SAMPLER(sampler_XTex);
            TEXTURE2D(_YTex);
            SAMPLER(sampler_YTex);
            TEXTURE2D(_ZTex);
            SAMPLER(sampler_ZTex);
            
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _ShadowColor;
                float4 _XTex_ST;
                float4 _YTex_ST;
                float4 _ZTex_ST;
                float _ShadowHardness;
                float _TexScale;
                float _LightEdge;
                float _DarkEdge;
                int _ShadowDivisions;
                float _ShadowThreshold;
                float _ShadowBlur;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionOS = IN.positionOS.xyz;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv = IN.uv; 
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.normalOS = IN.normalOS;
                
                return OUT;
            }

            float3 GetObjectWorldScale()
            {
                float3 scale;
                scale.x = length(unity_ObjectToWorld._m00_m10_m20);
                scale.y = length(unity_ObjectToWorld._m01_m11_m21);
                scale.z = length(unity_ObjectToWorld._m02_m12_m22);
                return scale;
            }
            
            float SampleMainShadowAtWorldPos(float3 positionWS)
            {
                float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
                return MainLightRealtimeShadow(shadowCoord);
            }
            
            float SampleBlurredMainShadow(float3 positionWS, float3 normalWS)
            {
                float3 lightDir = normalize(_MainLightPosition.xyz);
            
                // Build a small tangent basis on the surface.
                float3 tangent = normalize(cross(normalWS, lightDir));
            
                // Fallback in case normal and light direction are nearly parallel.
                if (length(tangent) < 0.001)
                    tangent = float3(1, 0, 0);
            
                float3 bitangent = normalize(cross(normalWS, tangent));
            
                float radius = _ShadowBlur;
            
                float shadow = 0.0;
            
                shadow += SampleMainShadowAtWorldPos(positionWS + (-tangent - bitangent) * radius);
                shadow += 2 * SampleMainShadowAtWorldPos(positionWS + (          - bitangent) * radius);
                shadow += SampleMainShadowAtWorldPos(positionWS + ( tangent - bitangent) * radius);
            
                shadow += 2 * SampleMainShadowAtWorldPos(positionWS + (-tangent) * radius);
                shadow += 4 * SampleMainShadowAtWorldPos(positionWS);
                shadow += 2 * SampleMainShadowAtWorldPos(positionWS + ( tangent) * radius);
            
                shadow += SampleMainShadowAtWorldPos(positionWS + (-tangent + bitangent) * radius);
                shadow += 2 * SampleMainShadowAtWorldPos(positionWS + (           bitangent) * radius);
                shadow += SampleMainShadowAtWorldPos(positionWS + ( tangent + bitangent) * radius);
            
                return shadow / 16.0;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                half3 lightDir    = mainLight.direction;
                half3 lightColor  = mainLight.color;
                float attenuation = mainLight.shadowAttenuation;

                float3 worldScale = GetObjectWorldScale();
                worldScale *= _TexScale;

                float3 pos = IN.positionOS.xyz;

                float2 xUV = pos.yz * worldScale.yz;
                float2 yUV = pos.xz * worldScale.xz;
                float2 zUV = pos.xy * worldScale.xy;

                half xWeight = abs(dot(normalize(IN.normalOS), half3(1, 0, 0)));
                half yWeight = abs(dot(normalize(IN.normalOS), half3(0, 1, 0)));
                half zWeight = abs(dot(normalize(IN.normalOS), half3(0, 0, 1)));

                half4 xColor = SAMPLE_TEXTURE2D(_XTex, sampler_XTex, xUV) * _BaseColor;
                half4 yColor = SAMPLE_TEXTURE2D(_YTex, sampler_YTex, yUV) * _BaseColor;
                half4 zColor = SAMPLE_TEXTURE2D(_ZTex, sampler_ZTex, zUV) * _BaseColor;

                xColor *= xWeight;
                yColor *= yWeight;
                zColor *= zWeight;

                half4 color = xColor + yColor + zColor;

                // Shadows
                float lightness = dot(IN.normalWS, lightDir);
                lightness =  Remap(-1, 1, 0, 1, lightness);
                lightness = step(_ShadowThreshold, lightness);
                
                float stepAttenuation = step(_ShadowThreshold, SampleBlurredMainShadow(IN.positionWS, IN.normalWS));
                stepAttenuation = saturate(stepAttenuation);
                
                lightness = min(lightness, stepAttenuation);
                color = lerp(_ShadowColor, color,
                    lightness);
                
                return color;
            }
            ENDHLSL
        }
        
        
        // Pass writes to depth/normal buffer for outlines
        Pass
        {
            Name "DepthNormals"
            Tags {"LightMode" = "DepthNormalsOnly"}
            
            ZWrite On
            
            HLSLPROGRAM
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment
        
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
            ENDHLSL
        }

        // Pass makes the material contribute to the global shadow map.
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
        
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
        
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
        
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }
    }
}

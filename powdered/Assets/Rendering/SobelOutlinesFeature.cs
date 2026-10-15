using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public sealed class SobelOutlinesFeature : ScriptableRendererFeature
    {
        [SerializeField] Material material;
        [SerializeField] RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;

        NormalPostPass pass;

        public override void Create()
        {
            pass = new NormalPostPass(material)
            {
                renderPassEvent = injectionPoint
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (material == null)
                return;

            renderer.EnqueuePass(pass);
        }

        sealed class NormalPostPass : ScriptableRenderPass
        {
            readonly Material material;

            public NormalPostPass(Material material)
            {
                this.material = material;

                // Needed because we sample the camera color as _BlitTexture.
                requiresIntermediateTexture = true;

                // Critical: tells URP to create/bind _CameraNormalsTexture.
                ConfigureInput(ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resources = frameData.Get<UniversalResourceData>();

                TextureHandle source = resources.activeColorTexture;

                TextureDesc desc = renderGraph.GetTextureDesc(source);
                desc.name = "NormalPostTemp";
                desc.clearBuffer = false;

                TextureHandle destination = renderGraph.CreateTexture(desc);

                var blitParams = new RenderGraphUtils.BlitMaterialParameters(
                    source,
                    destination,
                    material,
                    0
                );

                renderGraph.AddBlitPass(blitParams, "SobelOutlinesEffect");

                // Avoid a second blit back; make the temp texture the new camera color.
                resources.cameraColor = destination;
            }
        }
    }
}
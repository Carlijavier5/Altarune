using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class PixelizePass : ScriptableRenderPass
{
    private readonly PixelizeFeature.CustomPassSettings settings;
    private readonly Material material;

    public PixelizePass(PixelizeFeature.CustomPassSettings settings, Shader shader) {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
        material = CoreUtils.CreateEngineMaterial(shader);
    }

    private class PassData {
        internal TextureHandle source;
        internal Material material;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        TextureHandle source = resourceData.cameraColor;
        if (!source.IsValid()) return;

        int pixelScreenHeight = settings.screenHeight;
        int pixelScreenWidth = (int) (pixelScreenHeight * cameraData.camera.aspect + 0.5f);

        material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
        material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

        TextureDesc pixelDesc = renderGraph.GetTextureDesc(source);
        pixelDesc.width = pixelScreenWidth;
        pixelDesc.height = pixelScreenHeight;
        pixelDesc.depthBufferBits = 0;
        pixelDesc.name = "_PixelBuffer";
        pixelDesc.clearBuffer = false;
        pixelDesc.filterMode = FilterMode.Point;
        TextureHandle pixelBuffer = renderGraph.CreateTexture(pixelDesc);

        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Pixelize Down", out PassData passData)) {
            passData.source = source;
            passData.material = material;
            builder.UseTexture(source);
            builder.SetRenderAttachment(pixelBuffer, 0);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                Blitter.BlitTexture(context.cmd, data.source, new(1, 1, 0, 0), data.material, 0);
            });
        }

        TextureDesc outputDesc = renderGraph.GetTextureDesc(source);
        outputDesc.depthBufferBits = 0;
        outputDesc.name = "_PixelizeOutput";
        outputDesc.clearBuffer = false;
        TextureHandle output = renderGraph.CreateTexture(outputDesc);

        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Pixelize Up", out PassData passData)) {
            passData.source = pixelBuffer;
            passData.material = null;
            builder.UseTexture(pixelBuffer);
            builder.SetRenderAttachment(output, 0);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                Blitter.BlitTexture(context.cmd, data.source, new(1, 1, 0, 0), 0, false);
            });
        }

        resourceData.cameraColor = output;
    }
}

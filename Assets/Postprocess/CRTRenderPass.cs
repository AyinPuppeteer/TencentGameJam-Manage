using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CRTRenderPass : ScriptableRenderPass
{
    private readonly CRTRendererFeature.CRTSettings settings;

    private RTHandle source;
    private RTHandle lowResRT;
    private RTHandle tempRT;

    private const string ProfilerTag = "CRT Render Pass";

    public CRTRenderPass(CRTRendererFeature.CRTSettings settings)
    {
        this.settings = settings;
    }

    public void Setup(RTHandle source)
    {
        this.source = source;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor cameraDesc = renderingData.cameraData.cameraTargetDescriptor;
        cameraDesc.depthBufferBits = 0;
        cameraDesc.msaaSamples = 1;

        RenderTextureDescriptor lowResDesc = cameraDesc;
        lowResDesc.width = settings.lowResWidth;
        lowResDesc.height = settings.lowResHeight;

        RenderingUtils.ReAllocateIfNeeded(
            ref lowResRT,
            lowResDesc,
            settings.filterMode,
            TextureWrapMode.Clamp,
            name: "_CRTLowResRT"
        );

        RenderTextureDescriptor tempDesc = cameraDesc;
        tempDesc.depthBufferBits = 0;
        tempDesc.msaaSamples = 1;

        RenderingUtils.ReAllocateIfNeeded(
            ref tempRT,
            tempDesc,
            FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: "_CRTTempRT"
        );
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (settings.crtMaterial == null || source == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);

        using (new ProfilingScope(cmd, new ProfilingSampler(ProfilerTag)))
        {
            // 1. 原始画面 -> 低分辨率 RT
            Blitter.BlitCameraTexture(cmd, source, lowResRT);

            // 给 shader 传一些参数
            settings.crtMaterial.SetTexture("_LowResTex", lowResRT);
            settings.crtMaterial.SetFloat("_LowResWidth", settings.lowResWidth);
            settings.crtMaterial.SetFloat("_LowResHeight", settings.lowResHeight);

            // 2. 低分辨率 RT -> 临时 RT，并套 CRT 材质
            Blitter.BlitCameraTexture(cmd, lowResRT, tempRT, settings.crtMaterial, 0);

            // 3. 临时 RT -> 屏幕
            Blitter.BlitCameraTexture(cmd, tempRT, source);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        lowResRT?.Release();
        tempRT?.Release();
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CRTRenderPass : ScriptableRenderPass
{
    private readonly CRTRendererFeature.CRTSettings settings;

    private RTHandle source;
    private RTHandle lowResRT;
    private RTHandle tempRT;

    private static readonly int LowResTexID = Shader.PropertyToID("_LowResTex");
    private static readonly int LowResWidthID = Shader.PropertyToID("_LowResWidth");
    private static readonly int LowResHeightID = Shader.PropertyToID("_LowResHeight");

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

        // 保留相机原本的颜色格式，避免破坏 HDR
        cameraDesc.depthBufferBits = 0;
        cameraDesc.msaaSamples = 1;
        cameraDesc.bindMS = false;
        cameraDesc.useMipMap = false;
        cameraDesc.autoGenerateMips = false;

        RenderTextureDescriptor lowResDesc = cameraDesc;
        lowResDesc.width = settings.lowResWidth;
        lowResDesc.height = settings.lowResHeight;

        RenderingUtils.ReAllocateIfNeeded(
            ref lowResRT,
            lowResDesc,
            settings.lowResFilterMode,
            TextureWrapMode.Clamp,
            name: "_CRTLowResRT"
        );

        RenderTextureDescriptor tempDesc = cameraDesc;

        RenderingUtils.ReAllocateIfNeeded(
            ref tempRT,
            tempDesc,
            settings.tempFilterMode,
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
            // 1. 当前最终画面 -> 低分辨率 RT
            Blitter.BlitCameraTexture(cmd, source, lowResRT);

            // 2. 把低分辨率 RT 传给 CRT shader
            settings.crtMaterial.SetTexture(LowResTexID, lowResRT);
            settings.crtMaterial.SetFloat(LowResWidthID, settings.lowResWidth);
            settings.crtMaterial.SetFloat(LowResHeightID, settings.lowResHeight);

            // 3. CRT shader 处理 -> tempRT
            Blitter.BlitCameraTexture(cmd, lowResRT, tempRT, settings.crtMaterial, 0);

            // 4. tempRT -> 屏幕
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
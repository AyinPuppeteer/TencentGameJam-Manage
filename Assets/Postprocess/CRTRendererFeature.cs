using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CRTRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class CRTSettings
    {
        [Header("Material")]
        public Material crtMaterial;

        [Header("Render Pass Event")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Header("Low Resolution")]
        [Range(64, 1920)] public int lowResWidth = 320;
        [Range(64, 1080)] public int lowResHeight = 180;

        [Header("Filter Mode")]
        public FilterMode filterMode = FilterMode.Point;
    }

    public CRTSettings settings = new CRTSettings();

    private CRTRenderPass crtPass;

    public override void Create()
    {
        crtPass = new CRTRenderPass(settings);
        crtPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.crtMaterial == null)
        {
            Debug.LogWarning("CRTRendererFeature: CRT Material is missing.");
            return;
        }

        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;

        renderer.EnqueuePass(crtPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (settings.crtMaterial == null)
            return;

        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;

        crtPass.Setup(renderer.cameraColorTargetHandle);
    }

    protected override void Dispose(bool disposing)
    {
        crtPass?.Dispose();
    }
}
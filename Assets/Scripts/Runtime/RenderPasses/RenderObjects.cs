using System.Collections.Generic;

namespace UnityEngine.Rendering.LWRP
{
    public enum RenderQueueType
    {
        Opaque,
        Transparent,
    }

    public class RenderObjects : ScriptableRendererFeature
    {
        [System.Serializable]
        public class RenderObjectsSettings
        {
            public string passTag = "RenderObjectsFeature";
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

            public FilterSettings filterSettings = new FilterSettings();

            public Material overrideMaterial = null;
            public int overrideMaterialPassIndex = 0;

            public bool overrideDepthState = false;
            public CompareFunction depthCompareFunction = CompareFunction.Less;
            public bool enableWrite = true;

            public StencilStateData stencilSettings = new StencilStateData();

            public CustomCameraSettings cameraSettings = new CustomCameraSettings();
        }
        
        [System.Serializable]
        public class FilterSettings
        {
            // TODO: expose opaque, transparent, all ranges as drop down
            public RenderQueueType RenderQueueType;
            public LayerMask LayerMask;
            public string[] PassNames;

            public FilterSettings()
            {
                RenderQueueType = RenderQueueType.Opaque;
                LayerMask = -1;
            }
        }

        [System.Serializable]
        public class CustomCameraSettings
        {
            public bool overrideCamera = false;
            public bool restoreCamera = true;
            public Vector4 offset;
            public float cameraFieldOfView = 60.0f;
        }

        public RenderObjectsSettings settings = new RenderObjectsSettings();

        RenderObjectsPass renderObjectsPass;

        public override void Create()
        {
            FilterSettings filter = settings.filterSettings;
            renderObjectsPass = new RenderObjectsPass(settings.passTag, settings.Event, filter.PassNames,
                filter.RenderQueueType, filter.LayerMask, settings.cameraSettings);

            renderObjectsPass.overrideMaterial = settings.overrideMaterial;
            renderObjectsPass.overrideMaterialPassIndex = settings.overrideMaterialPassIndex;

            if (settings.overrideDepthState)
                renderObjectsPass.SetDetphState(settings.enableWrite, settings.depthCompareFunction);

            if (settings.stencilSettings.overrideStencilState)
                renderObjectsPass.SetStencilState(settings.stencilSettings.stencilReference,
                    settings.stencilSettings.stencilCompareFunction, settings.stencilSettings.passOperation,
                    settings.stencilSettings.failOperation, settings.stencilSettings.zFailOperation);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(renderObjectsPass);
        }
    }
}

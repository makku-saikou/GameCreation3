namespace Game2DWaterKit.Rendering
{
    using UnityEngine;

#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
#endif

    public class WaterRenderingMode
    {
        private readonly bool _isReflectionMode;

        private float _renderTextureResizingFactor;
        private bool _renderTextureUseFixedSize;
        private int _renderTextureFixedSize;
        private float _viewingFrustumHeightScalingFactor;
        private LayerMask _cullingMask;
        private FilterMode _renderTextureFilterMode;

        private int _lastCullingMask;
        private int _refractionMaskLayer;

        public WaterRenderingMode(WaterRenderingModeParameters parameters, bool isReflectionMode)
        {
            _renderTextureResizingFactor = parameters.TextuerResizingFactor;
            _renderTextureFixedSize = parameters.TextureFixedSize;
            _renderTextureUseFixedSize = parameters.TextureUseFixedSize;
            _viewingFrustumHeightScalingFactor = parameters.ViewingFrustumHeightScalingFactor;
            _cullingMask = parameters.CullingMask;
            _renderTextureFilterMode = parameters.FilterMode;

            _isReflectionMode = isReflectionMode;
        }

#region Properties
        public float ViewingFrustumHeightScalingFactor { get { return _viewingFrustumHeightScalingFactor; } set { _viewingFrustumHeightScalingFactor = Mathf.Clamp(value,-0.999f,float.MaxValue); } }
        public float RenderTextureResizingFactor { get { return _renderTextureResizingFactor; } set { _renderTextureResizingFactor = Mathf.Clamp01(value); } }
        public bool RenderTextureUseFixedSize { get { return _renderTextureUseFixedSize; } set { _renderTextureUseFixedSize = value; } }
        public int RenderTextureFixedSize { get { return _renderTextureFixedSize; } set { _renderTextureFixedSize = Mathf.Clamp(value, 1, int.MaxValue); } }
        public LayerMask CullingMask { get { return _cullingMask; } set { _cullingMask = value; } }
        public FilterMode RenderTextureFilterMode { get { return _renderTextureFilterMode; } set { _renderTextureFilterMode = value; } }
#endregion

#region Methods

#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
        internal void Render(UnityEngine.Rendering.ScriptableRenderContext context, Camera camera, ref RenderTexture renderTexture, WaterRenderingVisibleArea visibleArea, Color backgroundColor, bool hdr, bool msaa, int extraLayersToIgnoreMask = 0)
#else
        internal void Render(Camera camera, ref RenderTexture renderTexture, WaterRenderingVisibleArea visibleArea, Color backgroundColor, bool hdr, bool msaa, int extraLayersToIgnoreMask = 0)
#endif
        {
            if (!visibleArea.IsValid)
                return;

            int renderTextureWidth, renderTextureHeight;

            if (!_renderTextureUseFixedSize)
            {
                renderTextureWidth = (int)(visibleArea.PixelWidth * _renderTextureResizingFactor);
                renderTextureHeight = (int)(visibleArea.PixelHeight * _renderTextureResizingFactor);

                if (renderTextureWidth < 1 || renderTextureHeight < 1)
                    return;

#if UNITY_6000_0_OR_NEWER && GAME_2D_WATER_KIT_URP
                // temporary workaround for render graph
                bool useRenderGraph = !GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>().enableRenderCompatibilityMode;
                if(useRenderGraph)
                {
                    renderTextureWidth = Mathf.Clamp(Mathf.ClosestPowerOfTwo(renderTextureWidth), 2, Screen.width);
                    renderTextureHeight = Mathf.Clamp(Mathf.ClosestPowerOfTwo(renderTextureHeight), 2, Screen.height);
                }
#endif
            }
            else renderTextureWidth = renderTextureHeight = _renderTextureFixedSize;

            var modeProperties = _isReflectionMode ? visibleArea.ReflectionProperties : visibleArea.RefractionProperties;

            var cullingMask = _cullingMask & (~extraLayersToIgnoreMask);

            camera.orthographic = visibleArea.IsOrthographicCamera;
            camera.projectionMatrix = modeProperties.ProjectionMatrix;
            camera.nearClipPlane = modeProperties.NearClipPlane;
            camera.farClipPlane = visibleArea.FarClipPlane;

            camera.cullingMask = cullingMask;
            camera.backgroundColor = backgroundColor;

            if (visibleArea.IsOrthographicCamera)
                camera.orthographicSize = visibleArea.OrthographicSize;
            else
                camera.fieldOfView = visibleArea.FieldOfView;

            camera.aspect = visibleArea.Aspect;
            camera.allowHDR = hdr;
            camera.allowMSAA = msaa;

            camera.transform.SetPositionAndRotation(modeProperties.Position, modeProperties.Rotation);

            camera.targetTexture = GetRenderTexture(ref renderTexture, renderTextureWidth, renderTextureHeight, hdr, msaa);

            var oRenderTexture = RenderTexture.active;
            RenderTexture.active = camera.targetTexture;

#if GAME_2D_WATER_KIT_LWRP
            UnityEngine.Rendering.LWRP.LightweightRenderPipeline.RenderSingleCamera(context, camera);
#elif GAME_2D_WATER_KIT_URP
#pragma warning disable CS0618 // Type or member is obsolete
            UnityEngine.Rendering.Universal.UniversalRenderPipeline.RenderSingleCamera(context, camera);
#pragma warning restore CS0618 // Type or member is obsolete
#else
            camera.Render();
#endif

            RenderTexture.active = oRenderTexture;
        }

        private RenderTexture GetRenderTexture(ref RenderTexture renderTexture, int width, int height, bool hdr, bool msaa)
        {
            var format = hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.ARGB32;
            var antiAliasing = msaa && QualitySettings.antiAliasing > 1 ? QualitySettings.antiAliasing : 1;

            if (renderTexture == null)
            {
                renderTexture = GetTemporaryRenderTexture(width, height, _renderTextureFilterMode, format, antiAliasing);
                return renderTexture;
            }

            //get a new temporary render texture for any change in texture size larger than this threshold
            const int changeInTextureSizeMinimumThreshold = 5; //5 pixels (You could vary this to your liking)
            bool getNewRenderTexture = ((Mathf.Abs(renderTexture.height - height) > changeInTextureSizeMinimumThreshold) || (Mathf.Abs(renderTexture.width - width) > changeInTextureSizeMinimumThreshold));

            getNewRenderTexture |= renderTexture.format != format;
            getNewRenderTexture |= renderTexture.antiAliasing != antiAliasing;
            getNewRenderTexture |= renderTexture.filterMode != _renderTextureFilterMode;

            if (getNewRenderTexture)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
                renderTexture = GetTemporaryRenderTexture(width, height, _renderTextureFilterMode, format, antiAliasing);
            }

            return renderTexture;
        }

        private static RenderTexture GetTemporaryRenderTexture(int width, int height,FilterMode filterMode, RenderTextureFormat format, int antiAliasing)
        {
            var renderTexture = RenderTexture.GetTemporary(width, height, 24, format, RenderTextureReadWrite.Default, antiAliasing);
            renderTexture.filterMode = filterMode;

            return renderTexture;
        }

        internal int GetValidRefractionMaskLayer(int extraLayersToIgnoreMask = 0)
        {
            var cullingMask = _cullingMask & (~extraLayersToIgnoreMask);
            if (cullingMask != _lastCullingMask)
            {
                _refractionMaskLayer = GetValidLayer(cullingMask);
                _lastCullingMask = cullingMask;
            }

            return _refractionMaskLayer;
        }

        private int GetValidLayer(LayerMask layerMask)
        {
            for (int i = 0; i < 32; i++)
            {
                if (layerMask == (layerMask | (1 << i)))
                    return i;
            }

            return -1;
        }

#endregion

#region Editor Only Methods

#if UNITY_EDITOR
        internal void Validate(WaterRenderingModeParameters parameters)
        {
            RenderTextureResizingFactor = parameters.TextuerResizingFactor;
            RenderTextureUseFixedSize = parameters.TextureUseFixedSize;
            RenderTextureFixedSize = parameters.TextureFixedSize;
            ViewingFrustumHeightScalingFactor = parameters.ViewingFrustumHeightScalingFactor;
            CullingMask = parameters.CullingMask;
            RenderTextureFilterMode = parameters.FilterMode;
        }
#endif

#endregion
    }

    public struct WaterRenderingModeParameters
    {
        public float TextuerResizingFactor;
        public bool TextureUseFixedSize;
        public int TextureFixedSize;
        public float ViewingFrustumHeightScalingFactor;
        public LayerMask CullingMask;
        public FilterMode FilterMode;
    }

}

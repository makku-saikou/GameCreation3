namespace Game2DWaterKit.Rendering
{
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Utils;
    using Game2DWaterKit.Rendering.Mask;
    using UnityEngine;
    using System.Collections.Generic;

    public class WaterfallRenderingModule : RenderingModule
    {
        private static SimpleFixedSizeList<Vector2> _clipeePoints;

        private Game2DWaterfall _waterfallObject;

        private WaterRenderingVisibleArea _visibleArea;
        private WaterRenderingCameraFrustum _renderingCameraFrustum;

        private List<RenderingCameraOutput> _renderingOutputs;
        private class RenderingCameraOutput
        {
            public Camera renderingCamera = null;
            public RenderTexture refractionRenderTexture = null;
            public Matrix4x4 waterfallMatrix = Matrix4x4.identity;
        }

        public WaterfallRenderingModule(Game2DWaterfall waterfallObject, WaterfallRenderingModuleParameters parameters)
            : base(parameters.RenderPixelLights, parameters.FarClipPlane, parameters.AllowMSAA, parameters.AllowHDR, parameters.SortingLayerID, parameters.SortingOrder, parameters.MeshMaskParameters)
        {
            _waterfallObject = waterfallObject;

            Refraction = new WaterRenderingMode(parameters.RefractionParameters, false);
        }

        public WaterRenderingMode Refraction { get; private set; }

        override internal void Initialize()
        {
            _mainModule = _waterfallObject.MainModule;
            _meshModule = _waterfallObject.MeshModule;
            _materialModule = _waterfallObject.MaterialModule;

            _visibleArea = new WaterRenderingVisibleArea(_mainModule);
            _renderingCameraFrustum = new WaterRenderingCameraFrustum(_mainModule);

            if (_clipeePoints == null)
                _clipeePoints = new SimpleFixedSizeList<Vector2>(8);

            _renderingOutputs = new List<RenderingCameraOutput>();

            base.Initialize();
        }

        internal override bool IsVisibleToRenderingCamera(RenderingCameraInformation renderingCameraInformation)
        {
            var renderingCameraCullingMask = renderingCameraInformation.CurrentCamera.cullingMask;
            if (renderingCameraCullingMask != (renderingCameraCullingMask | (1 << _mainModule.GameobjectLayer)))
                return false;

            bool isValidWaterfallSize = _mainModule.Width > 0f && _mainModule.Height > 0f;
            if (!isValidWaterfallSize)
                return false;

            _renderingCameraFrustum.Setup(renderingCameraInformation);

            Vector2 aabbMin = _renderingCameraFrustum.AABBMin;
            Vector2 aabbMax = _renderingCameraFrustum.AABBMax;
            Vector2 aabbExtents = (aabbMax - aabbMin) * 0.5f;
            Vector2 aabbCenter = (aabbMax + aabbMin) * 0.5f;

            if (Mathf.Abs(aabbCenter.x) > (_mainModule.Width * 0.5f + aabbExtents.x))
                return false;

            if (Mathf.Abs(aabbCenter.y) > (_mainModule.Height * 0.5f + aabbExtents.y))
                return false;

            return true;
        }

#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
        internal override void Render(UnityEngine.Rendering.ScriptableRenderContext context, RenderingCameraInformation renderingCameraInformation)
#else
        internal override void Render(RenderingCameraInformation renderingCameraInformation)
#endif
        {
            var materialModule = _materialModule as WaterfallMaterialModule;

            if (!materialModule.IsRefractionEnabled)
                return;

            var renderingOutput = GetRenderingCameraOutput(renderingCameraInformation.CurrentCamera);

            ComputeVisibleAreas(_meshModule.BoundsLocalSpace);

#if !GAME_2D_WATER_KIT_LWRP && !GAME_2D_WATER_KIT_URP
            int pixelLightCount = QualitySettings.pixelLightCount;
            if (!_renderPixelLights)
                QualitySettings.pixelLightCount = 0;
#endif

            _meshModule.SetRendererActive(false);

            Color backgroundColor = _renderingCameraFrustum.CurrentCamera.backgroundColor;
            backgroundColor.a = 1f;

            bool isUsingOpaqueRenderQueue = _materialModule.RenderQueue == 2000;

            if (!isUsingOpaqueRenderQueue)
            {
                var refractionRenderingPorperties = _visibleArea.RefractionProperties;
                Vector3 maskPosition = refractionRenderingPorperties.Position;
                maskPosition.z += refractionRenderingPorperties.NearClipPlane + 0.001f;
                SetupRefractionMask(maskPosition);
                SetRefractionMaskLayer(Refraction.GetValidRefractionMaskLayer());
            }

#if GAME_2D_WATER_KIT_LWRP || GAME_2D_WATER_KIT_URP
            Refraction.Render(context, _g2dwCamera, ref renderingOutput.refractionRenderTexture, _visibleArea, backgroundColor, _allowHDR, _allowMSAA);
#else
            Refraction.Render(_g2dwCamera, ref renderingOutput.refractionRenderTexture, _visibleArea, backgroundColor, _allowHDR, _allowMSAA);
#endif

            _refractionMask.SetActive(false);

#if !GAME_2D_WATER_KIT_LWRP && !GAME_2D_WATER_KIT_URP
            QualitySettings.pixelLightCount = pixelLightCount;
#endif

            _meshModule.SetRendererActive(true);

            var worldToVisibleAreaMatrix = Matrix4x4.TRS(_visibleArea.RefractionProperties.Position, _visibleArea.RefractionProperties.Rotation, new Vector3(1f, 1f, -1f)).inverse;
            var projectionMatrix = _visibleArea.RefractionProperties.ProjectionMatrix;
            renderingOutput.waterfallMatrix = (projectionMatrix * worldToVisibleAreaMatrix * _mainModule.LocalToWorldMatrix);
        }

        private void UpdateMaterialsRenderingProperties(RenderingCameraOutput renderingCameraOutput)
        {
            var materialModule = _materialModule as WaterfallMaterialModule;

            materialModule.SetRefractionRenderTexture(renderingCameraOutput.refractionRenderTexture);
            materialModule.SetWaterfallMatrix(renderingCameraOutput.waterfallMatrix);
            materialModule.ValidateMaterialPropertyBlock();
        }

        internal override void ValidateMaterialProperties(Camera currentCamera)
        {
            var renderingOutput = GetRenderingCameraOutput(currentCamera);
            UpdateMaterialsRenderingProperties(renderingOutput);
        }

        private void ComputeVisibleAreas(Bounds waterfallBounds)
        {
            _clipeePoints.Clear();
            _clipeePoints.Add(_renderingCameraFrustum.TopLeft);
            _clipeePoints.Add(_renderingCameraFrustum.TopRight);
            _clipeePoints.Add(_renderingCameraFrustum.BottomRight);
            _clipeePoints.Add(_renderingCameraFrustum.BottomLeft);

            bool isRenderingCameraFullyContainedInWaterBox = true;

            //Finding visible Waterfall Area

            Vector2 waterfallBoundsMin = waterfallBounds.min;
            Vector2 waterfallBoundsMax = waterfallBounds.max;

            //Clip camera frustrum against waterfall box edges to find the visible water area
            //top edge
            isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: true, isBottomOrLeftEdge: false, edgePosition: waterfallBoundsMax.y);
            //right edge
            isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: false, isBottomOrLeftEdge: false, edgePosition: waterfallBoundsMax.x);
            //bottom edge
            isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: true, isBottomOrLeftEdge: true, edgePosition: waterfallBoundsMin.y);
            //left edge
            isRenderingCameraFullyContainedInWaterBox &= WaterUtility.ClipPointsAgainstAABBEdge(_clipeePoints, isBottomOrTopEdge: false, isBottomOrLeftEdge: true, edgePosition: waterfallBoundsMin.x);

            _visibleArea.UpdateArea(_clipeePoints, _renderingCameraFrustum, isRenderingCameraFullyContainedInWaterBox, true, _farClipPlane);
        }

        private RenderingCameraOutput GetRenderingCameraOutput(Camera currentRenderingCamera)
        {
            RenderingCameraOutput renderingOutput = null;
            for (int i = 0, imax = _renderingOutputs.Count; i < imax; i++)
            {
                if (_renderingOutputs[i].renderingCamera == currentRenderingCamera)
                {
                    renderingOutput = _renderingOutputs[i];
                    break;
                }
            }
            if (renderingOutput == null)
            {
                renderingOutput = new RenderingCameraOutput();
                renderingOutput.renderingCamera = currentRenderingCamera;
                _renderingOutputs.Add(renderingOutput);
            }
            return renderingOutput;
        }

#if UNITY_EDITOR

        internal void Validate(WaterfallRenderingModuleParameters parameters)
        {
            Refraction.Validate(parameters.RefractionParameters);

            RenderPixelLights = parameters.RenderPixelLights;
            FarClipPlane = parameters.FarClipPlane;
            AllowMSAA = parameters.AllowMSAA;
            AllowHDR = parameters.AllowHDR;
            SortingLayerID = parameters.SortingLayerID;
            SortingOrder = parameters.SortingOrder;

            MeshMask.Validate(parameters.MeshMaskParameters);
        }

#endif

    }

    public struct WaterfallRenderingModuleParameters
    {
        public WaterRenderingModeParameters RefractionParameters;
        public float FarClipPlane;
        public bool RenderPixelLights;
        public bool AllowMSAA;
        public bool AllowHDR;
        public int SortingLayerID;
        public int SortingOrder;
        public MeshMaskParameters MeshMaskParameters;
    }
}

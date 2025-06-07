using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerTongueCurveProxy : MonoPlayerProxy
    {
        [Header("General References:")]
        [SerializeField] private PlayerTongue tongue;
        [SerializeField] private LineRenderer lineRenderer;
        public LineRenderer LineRenderer => lineRenderer;

        [Header("General Settings:")]
        [SerializeField] private int precision = 40;
        [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

        [Header("Rope Animation Settings:")]
        [SerializeField] private AnimationCurve ropeAnimationCurve;
        [SerializeField] private AnimationCurve waveSizeMultiplyAnimationCurve;
        [Range(0.01f, 4)] [SerializeField] private float startWaveSize = 4;
        private float _waveSize;

        [Header("Rope Progression:")]
        [SerializeField] private AnimationCurve ropeProgressionCurve;
        [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

        private float _moveTime;
        private bool _straightLine = true;

        public bool Enable
        {
            get => lineRenderer.enabled;
            set
            {
                if (lineRenderer.enabled == value) return;
                lineRenderer.enabled = value;
            }
        }

        private void OnEnable()
        {
            Init();
            _straightLine = false;
            
            lineRenderer.enabled = true;
            
            tongue.OnTongueLaunch += Init;
        }
        
        private void OnDisable()
        {
            lineRenderer.enabled = false;
            tongue.OnTongueLaunch -= Init;
        }
        
        protected override void Init()
        {
            _moveTime = 0;
            _waveSize = startWaveSize;
            
            lineRenderer.positionCount = precision;
            for (int i = 0; i < precision; i++)
            {
                lineRenderer.SetPosition(i, tongue.transform.position);
            }
        }

        private void Update()
        {
            _moveTime += Time.deltaTime;
            DrawRope();
        }

        void DrawRope()
        {
            if (_waveSize > 0)
            {
                _waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                _waveSize = 0;
                if (lineRenderer.positionCount != 2) { lineRenderer.positionCount = 2; }
            
                DrawRopeNoWaves();
            }
        }

        void DrawRopeWaves()
        {
            for (int i = 0; i < precision; i++)
            {
                float delta = i / (precision - 1f);
                var theWaveSize = _waveSize * waveSizeMultiplyAnimationCurve.Evaluate(_moveTime);
                Vector2 offset = Vector2.Perpendicular(tongue.transform.right).normalized * (ropeAnimationCurve.Evaluate(delta) * theWaveSize);
                Vector2 targetPosition = Vector2.Lerp(tongue.transform.position, tongue.TonguePoint.position, delta) + offset;
                Vector2 currentPosition = Vector2.Lerp(tongue.transform.position, targetPosition, ropeProgressionCurve.Evaluate(_moveTime) * ropeProgressionSpeed);
            
                lineRenderer.SetPosition(i, currentPosition);
            }
        }

        void DrawRopeNoWaves()
        {
            lineRenderer.SetPosition(0, tongue.transform.position);
            lineRenderer.SetPosition(1, tongue.TonguePoint.position);
        }
    }
}

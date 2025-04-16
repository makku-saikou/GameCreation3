using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerTongueCurveProxy : MonoBehaviour
    {
        [Header("General Refernces:")]
        [SerializeField] private PlayerTongue tongue;
        [SerializeField] private LineRenderer _lineRenderer;

        [Header("General Settings:")]
        [SerializeField] private int percision = 40;
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

        [HideInInspector] public bool isGrappling = true;

        private bool _straightLine = true;

        private void OnEnable()
        {
            Init();
            _straightLine = false;
            
            _lineRenderer.enabled = true;
            
            tongue.OnTongueLaunch += Init;
        }

        private void OnDisable()
        {
            _lineRenderer.enabled = false;
            isGrappling = false;
            
            tongue.OnTongueLaunch -= Init;
        }
        
        private void Init()
        {
            _moveTime = 0;
            _waveSize = startWaveSize;
            
            _lineRenderer.positionCount = percision;
            for (int i = 0; i < percision; i++)
            {
                _lineRenderer.SetPosition(i, tongue.transform.position);
            }
        }

        private void Update()
        {
            _moveTime += Time.deltaTime;
            DrawRope();
        }

        void DrawRope()
        {
            // if (!_straightLine)
            // {
            //     if (Mathf.Approximately(_lineRenderer.GetPosition(percision - 1).x, tongue.TonguePoint.position.x))
            //     {
            //         _straightLine = true;
            //     }
            //     else
            //     {
            //         DrawRopeWaves();
            //     }
            // }
            // else
            // {
            //     if (!isGrappling)
            //     {
            //         // tongue.Grapple();
            //         isGrappling = true;
            //     }
            //     if (_waveSize > 0)
            //     {
            //         _waveSize -= Time.deltaTime * straightenLineSpeed;
            //         DrawRopeWaves();
            //     }
            //     else
            //     {
            //         _waveSize = 0;
            //
            //         if (_lineRenderer.positionCount != 2) { _lineRenderer.positionCount = 2; }
            //
            //         DrawRopeNoWaves();
            //     }
            // }
            if (_waveSize > 0)
            {
                _waveSize -= Time.deltaTime * straightenLineSpeed;
                // if(_lineRenderer.positionCount != percision) { _lineRenderer.positionCount = percision; }
                DrawRopeWaves();
            }
            else
            {
                _waveSize = 0;
                if (_lineRenderer.positionCount != 2) { _lineRenderer.positionCount = 2; }
            
                DrawRopeNoWaves();
            }
        }

        void DrawRopeWaves()
        {
            for (int i = 0; i < percision; i++)
            {
                float delta = i / (percision - 1f);
                var theWaveSize = _waveSize * waveSizeMultiplyAnimationCurve.Evaluate(_moveTime);
                Vector2 offset = Vector2.Perpendicular(tongue.transform.right).normalized * (ropeAnimationCurve.Evaluate(delta) * theWaveSize);
                Vector2 targetPosition = Vector2.Lerp(tongue.transform.position, tongue.TonguePoint.position, delta) + offset;
                Vector2 currentPosition = Vector2.Lerp(tongue.transform.position, targetPosition, ropeProgressionCurve.Evaluate(_moveTime) * ropeProgressionSpeed);
            
                _lineRenderer.SetPosition(i, currentPosition);
            }
        }

        void DrawRopeNoWaves()
        {
            _lineRenderer.SetPosition(0, tongue.transform.position);
            _lineRenderer.SetPosition(1, tongue.TonguePoint.position);
        }
    }
}

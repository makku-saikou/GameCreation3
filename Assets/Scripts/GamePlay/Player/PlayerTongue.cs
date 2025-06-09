// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerTongue.cs
// Description: 舌头相关控制逻辑
// -------------------------------------------------
using System;
using GamePlay.Item.Target;
using GamePlay.Player.Particle;
using PurpleFlowerCore;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Player
{
    public enum ETongueState
    {
        Idle,
        Launching,
        Connecting,
        Pushing,
        Retracting
    }
    public class PlayerTongue : MonoBehaviour
    {
        [SerializeField] private DistanceJoint2D distanceJoint2D;
        [SerializeField] private PlayerHead head;
        private PlayerController Player => head.Player;
        private PlayerProperty Property => Player.Property;
        private PlayerConfig Config => Player.Config;
        [SerializeField]private ETongueState tongueState;
        public ETongueState TongueState => tongueState;
        
        [SerializeField] private Transform tonguePoint;
        public Transform TonguePoint => tonguePoint;
        [SerializeField] private PlayerTongueCurveProxy tongueCurveProxy;
        
        // todo: UI管理
        public Image targetImage;
        [SerializeField] private Transform root0;
        [SerializeField] private Transform root1;
        [SerializeField] private Transform root2;
        public event Action OnTongueLaunch;
        public event Action OnTongueIdle;
        public event Action OnTongueRetract;
        private int _layerBit;

        private ITarget _currentTarget;
        private RaycastHit2D _currentHit;
        private Vector3 _targetPosition;
        public Vector3 TargetPosition
        {
            get
            {
                if (_currentTarget is { IsAdsorb: true })
                    return _currentTarget.AdsorbPosition;
                return _targetPosition;
            }
        }

        public bool Enable
        {
            get => tongueCurveProxy.Enable;
            set => tongueCurveProxy.Enable = value;
        }

        private void Start()
        {
            tonguePoint.position = transform.position;
            transform.position = root0.position;
            var layers = Config.targetLayers;
            foreach (var layer in layers)
            {
                _layerBit |= layer;
            }
        }

        private void Update()
        {
            switch (tongueState)
            {
                case ETongueState.Idle:
                    UpdateTarget();
                    break;
                case ETongueState.Launching:
                    UpdateLaunch();
                    break;
                case ETongueState.Connecting:
                    UpdateConnecting();
                    break;
                case ETongueState.Retracting:
                    UpdateRetract();
                    break;
                case ETongueState.Pushing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ChangeRootPos();
            Debug.DrawLine(head.transform.position, Config.tongueMaxLength * head.transform.right + head.transform.position, Color.red);
        }
        
        public void Launch(Vector2 direction)
        {
            if(tongueState != ETongueState.Idle) return;
            targetImage.gameObject.SetActive(false);
            Property.HeadCanMove = false;
            transform.right = direction;
            tongueState = ETongueState.Launching;
            tonguePoint.parent = null;
            Property.IsLaunching = true;
            OnTongueLaunch?.Invoke();
        }
        
        private void UpdateLaunch()
        {
            Vector3 direction = TargetPosition - tonguePoint.position;
            direction.Normalize();
            tonguePoint.position += direction * (Time.deltaTime * Config.tongueSpeed);
            if(Vector3.SqrMagnitude(tonguePoint.position - TargetPosition) < 0.05f)
            {
                TryConnect();
            }
        }
        
        private void UpdateConnecting()
        {
            if(Property.CurrentTongueLength >  Config.tongueMaxLength)
            {
                Property.CurrentTongueLength = Config.tongueMaxLength;
            }
            else if(Property.CurrentTongueLength < Config.tongueMinLength)
            {
                Property.CurrentTongueLength = Config.tongueMinLength;
            }
            if(!Mathf.Approximately(distanceJoint2D.distance, Property.CurrentTongueLength))
            {
                distanceJoint2D.distance = Property.CurrentTongueLength;
            }

            distanceJoint2D.connectedAnchor = TargetPosition;
            
            Property.CurrentHongAngle = Vector2.SignedAngle(Vector2.down,
                Player.Entity.transform.position - tonguePoint.position);
        }
        
        private void UpdateRetract()
        {
            if (Vector3.SqrMagnitude(tonguePoint.position - transform.position) < 0.05f)
            {
                tongueState = ETongueState.Idle;
                tonguePoint.position = transform.position;
                Property.HeadCanMove = true;
                tonguePoint.parent = transform;
                Property.IsRetracting = false;
                OnTongueIdle?.Invoke();
            }
            Vector3 direction = transform.position - tonguePoint.position;
            direction.Normalize();
            tonguePoint.position += direction * (Time.deltaTime * Config.retractSpeed);
        }

        public void Retract()
        {
            if (tongueState is ETongueState.Retracting or ETongueState.Idle)
                return;
            OnTongueRetract?.Invoke();
            tongueState = ETongueState.Retracting;
            distanceJoint2D.enabled = false;
            _currentTarget = null;
            Player.Property.IsConnecting = false;
            Property.IsRetracting = true;
            Property.IsLaunching = false;
            tonguePoint.parent = null;
            if(_currentHit.normal.Equals(Vector2.up))
                Player.OnCollisionEnter -= OnNormalUp;
        }

        public void Interact()
        {
            if(_currentTarget == null) return;
            if (tongueState != ETongueState.Connecting) return;
            var retract = _currentTarget.Interact(Player);
            if(retract)
                Retract();
        }

        private void UpdateTarget()
        {   
            
            var hit = Physics2D.Raycast(transform.position, transform.right, Config.tongueMaxLength, _layerBit);
            _currentHit = hit;
            if (hit.collider)
            {
                _currentTarget = hit.collider.GetComponent<ITarget>();
                if (_currentTarget != null)
                {
                    if(_currentTarget.IsAdsorb)
                        _targetPosition = _currentTarget.AdsorbPosition;
                    else
                        _targetPosition = hit.point;
                    targetImage.gameObject.SetActive(true);
                    targetImage.transform.position = Camera.main.WorldToScreenPoint(_targetPosition);
                }
                else
                {
                    // todo: UI管理
                    targetImage.gameObject.SetActive(false);
                    _targetPosition = hit.point;
                }
            }
            else
            {
                targetImage.gameObject.SetActive(false);
                _currentTarget = null;
                _targetPosition = transform.position + transform.right * Config.tongueMaxLength;
            }
        }

        private void TryConnect()
        {
            PFCLog.Debug("Tongue",$"TryConnect: {_currentTarget}");
            Property.IsLaunching = false;
            if (_currentTarget == null)
            {
                Retract();
                return;
            }
            if(_currentTarget != null)
                tonguePoint.parent = _currentTarget.Root;
            Property.CurrentTongueLength = Vector3.Distance(Player.transform.position, TargetPosition);
            tongueState = ETongueState.Connecting;
            distanceJoint2D.enabled = true;
            distanceJoint2D.connectedAnchor = TargetPosition;
            Player.Property.IsConnecting = true;
            PFCLog.Debug("Tongue", _currentHit.normal);
            if (_currentHit.normal.Equals(Vector2.up))
                Player.OnCollisionEnter += OnNormalUp;
            Player.PlayerParticle.Get<HitPieces>().Play(tonguePoint.position, transform.right);
        }
        
        /// <summary>
        /// 当玩家勾到并撞到法线向上的平面时
        /// </summary>
        private void OnNormalUp(Collision2D collision)
        {
            PFCLog.Debug("Tongue", collision.contacts[0].normal);
            if(collision.contacts[0].normal.Equals(Vector2.up))
                Retract();
        }

        private void ChangeRootPos()
        {
            transform.position = Player.CurrentStateName switch
            {
                "Air" => root1.position,
                "Hang" => root1.position,
                "OnBackground" => root2.position,
                "OnPillar" => root2.position,
                _ => root0.position
            };
        }
    }
}
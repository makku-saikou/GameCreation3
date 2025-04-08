// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerTongue.cs
// Description: 舌头相关控制逻辑
// -------------------------------------------------
using System;
using GamePlay.Item;
using PurpleFlowerCore;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Player
{
    public enum TongueState
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
        [SerializeField] private PlayerController player;
        private PlayerProperty _property;
        private float _currentFlightDistance;
        private ITarget _currentTarget;
        [SerializeField]private TongueState _tongueState;
        [SerializeField] private PlayerTonguePoint tonguePoint;  // todo: 没有发现舌尖作为单独物体的优势,尝试简化为坐标
        private Vector3 _targetPosition;
        [SerializeField] private Image targetImage;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform root0;
        [SerializeField] private Transform root1;
        
        
        private void Start()
        {
            _property = player.Property;
            tonguePoint.transform.position = transform.position;
            transform.position = root0.position;
        }

        private void Update()
        {
            switch (_tongueState)
            {
                case TongueState.Idle:
                    UpdateTarget();
                    break;
                case TongueState.Launching:
                    UpdateLaunch();
                    break;
                case TongueState.Connecting:
                    UpdateConnecting();
                    break;
                case TongueState.Retracting:
                    UpdateRetract();
                    break;
                case TongueState.Pushing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            DrawTongue();
            ChangeRootPos();
            Debug.DrawLine(head.transform.position, _property.tongueMaxLength * head.transform.right + head.transform.position, Color.red);
        }
        
        public void Launch(Vector2 direction)
        {
            if(_tongueState != TongueState.Idle) return;
            targetImage.gameObject.SetActive(false);
            _property.HeadCanMove = false;
            _currentFlightDistance = 0;
            transform.right = direction; // temp
            _tongueState = TongueState.Launching;
            tonguePoint.transform.parent = null;
            lineRenderer.enabled = true;
        }
        
        private void UpdateLaunch()
        {
            PFCLog.Debug("Tongue", $"target: {_currentTarget}" );
            Vector3 direction = _targetPosition - tonguePoint.transform.position;
            direction.Normalize();
            tonguePoint.transform.position += direction * (Time.deltaTime * _property.tongueSpeed);
            if(Vector3.SqrMagnitude(tonguePoint.transform.position - _targetPosition) < 0.05f)
            {
                TryConnect();
            }
        }
        
        private void UpdateConnecting()
        {
            if(_property.CurrentTongueLength >  _property.tongueMaxLength)
            {
                _property.CurrentTongueLength = _property.tongueMaxLength;
            }
            else if(_property.CurrentTongueLength < _property.tongueMinLength)
            {
                _property.CurrentTongueLength = _property.tongueMinLength;
            }
            if(!Mathf.Approximately(distanceJoint2D.distance, _property.CurrentTongueLength))
            {
                distanceJoint2D.distance = _property.CurrentTongueLength;
            }
            // PFCLog.Debug("PlayerTongue", $"CurrentTongueLength: {_property.CurrentTongueLength}");
            
            // playerController.Property.ConnectAngle = Vector2.SignedAngle(Vector2.down,
            //     playerController.transform.position - transform.position);
            // playerController.Property.ConnectDirection = (playerController.transform.position - transform.position).normalized;
        }
        
        private void UpdateRetract()
        {
            if (Vector3.SqrMagnitude(tonguePoint.transform.position - transform.position) < 0.05f)
            {
                _tongueState = TongueState.Idle;
                tonguePoint.transform.position = transform.position;
                _property.HeadCanMove = true;
                tonguePoint.transform.parent = transform;
                lineRenderer.enabled = false;
            }
            Vector3 direction = transform.position - tonguePoint.transform.position;
            direction.Normalize();
            tonguePoint.transform.position += direction * (Time.deltaTime * _property.retractSpeed);
        }

        public void Retract()
        {
            _tongueState = TongueState.Retracting;
            distanceJoint2D.enabled = false;
            _currentTarget = null;
            player.Property.IsConnecting = false;
        }

        public void Interact()
        {
            if(_currentTarget == null) return;
            if (_tongueState != TongueState.Connecting) return;
            _currentTarget.Interact(player);
            Retract();
        }

        private void UpdateTarget()
        {   
            var hit = Physics2D.Raycast(transform.position, transform.right, _property.tongueMaxLength);
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
                    targetImage.gameObject.SetActive(false);
                    _targetPosition = hit.point;
                }
            }
            else
            {
                targetImage.gameObject.SetActive(false);
                _currentTarget = null;
                _targetPosition = transform.position + transform.right * _property.tongueMaxLength;
            }
        }

        private void TryConnect()
        {
            PFCLog.Debug("Tongue",$"TryConnect: {_currentTarget}");
            if (_currentTarget == null)
            {
                Retract();
                return;
            }
            _property.CurrentTongueLength = Vector3.Distance(player.transform.position, _targetPosition);
            _tongueState = TongueState.Connecting;
            distanceJoint2D.enabled = true;
            distanceJoint2D.connectedAnchor = _targetPosition;
            player.Property.IsConnecting = true;
            _property.HangPoint = _targetPosition;
        }

        private void DrawTongue()
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, tonguePoint.transform.position);
        }

        private void ChangeRootPos()
        {
            if (player.CurrentStateName == "Hang")
                transform.position = root1.position;
            else
                transform.position = root0.position;
        }
    }
}
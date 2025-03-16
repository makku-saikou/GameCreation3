// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerTongue.cs
// Description: 舌头相关控制逻辑
// -------------------------------------------------
using System;
using GamePlay.Item;
using UnityEngine;

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
    // todo: temp状态机，之后考虑使用统一的状态机模块
    public class PlayerTongue : MonoBehaviour
    {
        [SerializeField] private float tongueDistance = 8f;
        [SerializeField] private float tongueSpeed;
        [SerializeField] private float retractSpeed;
        [SerializeField] private DistanceJoint2D distanceJoint2D;
        [SerializeField] private PlayerHead head;
        [SerializeField] private PlayerController entity;
        private float _currentFlightDistance;
        [SerializeField]private TongueState _tongueState;
        private IConnectable _currentConnectableItem;

        private void Start()
        {
            transform.position = head.TongueRoot.position;
        }

        private void Update()
        {
            switch (_tongueState)
            {
                case TongueState.Idle:
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
            
            Debug.DrawLine(head.transform.position, tongueDistance * head.transform.right + head.transform.position, Color.red);
        }
        
        public void Launch(Vector3 position, Vector2 direction)
        {
            if(_tongueState != TongueState.Idle) return;
            _currentFlightDistance = 0;
            transform.position = position;
            transform.right = direction;
            _tongueState = TongueState.Launching;
        }
        
        private void UpdateLaunch()
        {
            if (_currentFlightDistance >= tongueDistance)
            {
                Retract();
                return;
            }
            _currentFlightDistance += Time.deltaTime * tongueSpeed;
            transform.position += transform.right * (Time.deltaTime * tongueSpeed);
            var res = Physics2D.OverlapCircle(transform.position, 0.1f);
            if (res != null && res.CompareTag("Connectable"))
            {
                _currentConnectableItem = res.GetComponent<IConnectable>();
                _tongueState = TongueState.Connecting;
                head.canMove = false;
                transform.parent = null;
                distanceJoint2D.enabled = true;
                distanceJoint2D.connectedAnchor = transform.position;
            }
        }
        
        private void UpdateConnecting()
        {
            // todo: 判断目标是否可移动，之后要注意是否考虑质量
            if(Vector3.SqrMagnitude(transform.position - entity.transform.position) < tongueDistance * tongueDistance)
            {
                distanceJoint2D.distance = Vector3.Distance(transform.position, entity.transform.position);
            }else
            {
                distanceJoint2D.distance = tongueDistance;
            }
        }
        
        private void UpdateRetract()
        {
            if (Vector3.SqrMagnitude(transform.position - head.TongueRoot.position) < 0.05f)
            {
                _tongueState = TongueState.Idle;
                transform.position = head.TongueRoot.position;
                head.canMove = true;
            }
            Vector3 direction = head.TongueRoot.position - transform.position;
            direction.Normalize();
            transform.position += direction.normalized * (Time.deltaTime * retractSpeed);
        }

        public void Retract()
        {
            transform.parent = head.transform;
            transform.localScale = Vector3.one; // temp
            _tongueState = TongueState.Retracting;
            distanceJoint2D.enabled = false;
            _currentConnectableItem = null;
        }

        public void Interact()
        {
            if(_currentConnectableItem == null) return;
            if (_tongueState != TongueState.Connecting) return;
            _currentConnectableItem.Interact(entity);
            Retract();
        }
    }
}
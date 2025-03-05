using System;
using PurpleFlowerCore;
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
    // todo: temp状态机
    public class PlayerTongue : MonoBehaviour
    {
        [SerializeField] private float tongueDistance = 8f;
        [SerializeField] private float flightTime;
        [SerializeField] private float tongueSpeed;
        // [SerializeField] private SpringJoint2D springJoint2D; // 当前的实现中,考虑到需要SpringJoint的情况较少并且会造成更多麻烦,我们暂不使用joing
        [SerializeField] private PlayerHead head;
        [SerializeField] private Transform root;
        private float _currentFlightTime;
        private TongueState _tongueState;
        
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
            gameObject.SetActive(true);
            _currentFlightTime = flightTime;
            transform.position = position;
            transform.right = direction;
            _tongueState = TongueState.Launching;
        }
        
        private void UpdateLaunch()
        {
            if (_currentFlightTime <= 0)
            {
                _tongueState = TongueState.Retracting;
                gameObject.SetActive(false);
            }
            _currentFlightTime -= Time.deltaTime;
            transform.position += transform.right * (Time.deltaTime * tongueSpeed);
        }
        
        private void UpdateConnecting()
        {
            // todo: 判断目标是否可移动，之后要主要是否考虑质量
            Transform headTransform = head.transform;
            if(Vector3.SqrMagnitude(transform.position - headTransform.position) > tongueDistance * tongueDistance)
            {
                headTransform.position = Vector3.Lerp(headTransform.position, transform.position, 0.1f);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(1);
            if (other.CompareTag("Connectable") && _tongueState == TongueState.Launching)
            {
                PFCLog.Debug("Connect");
                _tongueState = TongueState.Connecting;
                head.canMove = false;
                transform.parent = root;
                // todo: 获得交互物，以某种方式处理交互事件
            }
        }
        
        private void UpdateRetract()
        {
            if (Vector3.SqrMagnitude(transform.position - head.transform.position) < 0.1f)
            {
                _tongueState = TongueState.Idle;
                head.canMove = true;
                gameObject.SetActive(false);
            }
            transform.position = Vector3.Lerp(transform.position, head.transform.position, 0.1f);
        }

        public void Retract()
        {
            transform.parent = head.transform;
            _tongueState = TongueState.Retracting;
        }
    }
}
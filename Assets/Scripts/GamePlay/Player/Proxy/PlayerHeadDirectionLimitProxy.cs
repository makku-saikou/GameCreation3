// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_15
// File: PlayerHeadDirectionLimitProxy.cs
// Description: 将玩家头部方向限制的逻辑从PlayerHead中分离出来，用状态机或其他事件控制
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerHeadDirectionLimitProxy : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerHead playerHead;
        private PlayerProperty _property;
        
        private DirectionLimit _onGroundLimit;
        private DirectionLimit _onAirLimit;
        
        private void Start()
        {
            _property = playerController.Property;
            _onGroundLimit = OnGroundLimit;
            _onAirLimit = OnAirLimit;
            
            // 为了确保事件注册在玩家初始化之后，且由于我们不会使该组建失效，所以在Start中注册事件
            playerController.StateMachine.OnStateChanged += CheckDirectionLimit;
        }
        
        private void OnDisable()
        {
            playerController.StateMachine.OnStateChanged -= CheckDirectionLimit;
        }

        private Vector3 OnGroundLimit(Vector3 direction)
        {
            if (direction.y > 0)
            {
                if (direction.y > _property.onGroundUpLimit)
                {
                    direction.y = _property.onGroundUpLimit;
                    if(playerController.Property.IsFacingRight)
                        direction.x = 1 - _property.onGroundUpLimit * _property.onGroundUpLimit;
                    else
                        direction.x = -1 + _property.onGroundUpLimit * _property.onGroundUpLimit;
                }
            }
            else
            {
                if (direction.y < -_property.onGroundDownLimit)
                {
                    direction.y = -_property.onGroundDownLimit;
                    if(playerController.Property.IsFacingRight)
                        direction.x = 1 - _property.onGroundDownLimit * _property.onGroundDownLimit;
                    else
                        direction.x = -1 + _property.onGroundDownLimit * _property.onGroundDownLimit;
                }
            }
            
            // Vector3 lineDirection = new Vector3(1 - OnGroundUpLimit * OnGroundUpLimit, OnGroundUpLimit);
            // Debug.DrawLine(playerHead.transform.position, playerHead.transform.position + lineDirection, Color.red);
            // lineDirection = new Vector3(-1 + OnGroundDownLimit * OnGroundDownLimit, -OnGroundDownLimit);
            // Debug.DrawLine(playerHead.transform.position, playerHead.transform.position + lineDirection, Color.red);
            // lineDirection = new Vector3(1 - OnGroundDownLimit * OnGroundDownLimit, -OnGroundDownLimit);
            // Debug.DrawLine(playerHead.transform.position, playerHead.transform.position + lineDirection, Color.red);
            // lineDirection = new Vector3(-1 + OnGroundUpLimit * OnGroundUpLimit, OnGroundUpLimit);
            // Debug.DrawLine(playerHead.transform.position, playerHead.transform.position + lineDirection, Color.red);
            return direction;
        }
        
        private Vector3 OnAirLimit(Vector3 direction)
        {
            return direction;
        }
        
        private void CheckDirectionLimit(HState from, HState to)
        {
            playerHead.DirectionLimit = to.Name switch
            {
                "OnGround" => _onGroundLimit,
                "Air" => _onAirLimit,
                _ => playerHead.DirectionLimit
            };
        }
    }
}
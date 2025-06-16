// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_15
// File: PlayerHeadDirectionLimitProxy.cs
// Description: 将玩家头部方向限制的逻辑从PlayerHead中分离出来，用状态机或其他事件控制
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerHeadDirectionLimitProxy : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        private PlayerHead PlayerHead => playerController.Head;
        private PlayerProperty Property => playerController.Property;
        private PlayerConfig Config => playerController.Config;
        
        private DirectionLimit _onGroundLimit;
        private DirectionLimit _onBackgroundLimit;
        private DirectionLimit _none;
        
        protected void Start()
        {
            _onGroundLimit = OnGroundLimit;
            _none = direction => direction;

            // 为了确保事件注册在玩家初始化之后，且由于我们不会使该组建失效，所以在Start中注册事件
            playerController.StateMachine.OnStateChanged += CheckDirectionLimit;
            DelayUtility.DelayFrame(2, () =>
            {
                // 确保玩家初始化完成后再设置初始方向限制
                CheckDirectionLimit(null, playerController.StateMachine.CurrentState);
            });
        }

        private Vector3 OnGroundLimit(Vector3 direction)
        {
            if (direction.y > 0)
            {
                if (direction.y > Config.onGroundUpLimit)
                {
                    direction.y = Config.onGroundUpLimit;
                    if(playerController.Property.IsFacingRight)
                        direction.x = 1 - Config.onGroundUpLimit * Config.onGroundUpLimit;
                    else
                        direction.x = -1 + Config.onGroundUpLimit * Config.onGroundUpLimit;
                }
            }
            else
            {
                if (direction.y < -Config.onGroundDownLimit)
                {
                    direction.y = -Config.onGroundDownLimit;
                    if(playerController.Property.IsFacingRight)
                        direction.x = 1 - Config.onGroundDownLimit * Config.onGroundDownLimit;
                    else
                        direction.x = -1 + Config.onGroundDownLimit * Config.onGroundDownLimit;
                }
            }

            _onBackgroundLimit = _ => playerController.Entity.up;
            
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
        
        private void CheckDirectionLimit(HState from, HState to)
        {
            // if (to == null)
            // {
            //     PlayerHead.DirectionLimit = _onGroundLimit;
            //     return;
            // }
            PlayerHead.DirectionLimit = to.Name switch
            {
                "OnGround" => _onGroundLimit,
                "OnBackground" => _onBackgroundLimit,
                _ => _none
            };
        }
    }
}
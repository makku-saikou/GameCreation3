// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_04_10
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player
{
    // todo:考虑将碰撞体移到Entity上，但由于历史遗留问题，许多组件都通过碰撞体获得玩家引用，暂不做此操作
    public class PlayerColliderShapeProxy : MonoPlayerProxy
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private Collider2D collider0;
        [SerializeField] private Collider2D collider1;

        protected override void Init()
        {
            player.StateMachine.OnStateChanged += ChangeColliderShape;
        }

        private void ChangeColliderShape(HState from, HState to)
        {
            if (to.Name == "Hang" || to.Name == "OnPillar")
            {
                collider0.enabled = false;
                collider1.enabled = true;
            }
            else
            {
                collider0.enabled = true;
                collider1.enabled = false;
            }
        }
    }
}
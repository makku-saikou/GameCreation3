using UnityEngine;

namespace GamePlay.Player
{
    // TODO: 这个写法非常temp,之后我们要考虑InputSystem,如果玩家状态过多,考虑把PlayerController改成大状态机
    public class PlayerHead : MonoBehaviour
    {
        [SerializeField] private PlayerTongue playerTongue;
        public bool canMove;
        private void Update()
        {
            UpdateDirection();
            if (Input.GetMouseButtonDown(0))
            {
                LaunchTongue();
            }
            
        }

        private void UpdateDirection()
        {
            if (!canMove) return;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = (mousePos - transform.position).normalized;
            direction.z = 0;
            transform.right = Vector3.Lerp(transform.right, direction, 0.1f);
        }

        private void LaunchTongue()
        {
            playerTongue.Launch(transform.position, transform.right);
        }

        private void RetractTongue()
        {
            playerTongue.Retract();
        }
    }
}
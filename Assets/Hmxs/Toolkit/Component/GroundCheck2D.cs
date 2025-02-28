using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hmxs.Toolkit
{
    public class GroundCheck2D : MonoBehaviour
    {
        public bool IsGrounded
        {
            private set
            {
                if (isGrounded == value) return;
                if (isGrounded)
                    onLeftGround?.Invoke();
                else
                    onAtGround?.Invoke();
                isGrounded = value;
            }
            get => isGrounded;
        }

        public Action onAtGround;
        public Action onLeftGround;

        [Title("Setting")]
        [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private Vector2 boxSize = Vector2.one;
        [SerializeField] private float angle;
        [SerializeField] private LayerMask groundMask;

        [Title("Info")]
        [SerializeField] [ReadOnly] private bool isGrounded;
        [SerializeField] [ReadOnly] private List<Collider2D> colliders = new();

        private void Awake() => isGrounded = false;

        private void Update() => CheckGround();

        private void CheckGround()
        {
            colliders.Clear();
            colliders = Physics2D.OverlapBoxAll(
                (Vector2)transform.position + offset,
                boxSize,
                angle,
                groundMask).ToList();

            IsGrounded = colliders.Count > 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + (Vector3)offset, boxSize);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
}
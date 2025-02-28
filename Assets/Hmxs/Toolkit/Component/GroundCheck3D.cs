using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hmxs.Toolkit
{
    public class GroundCheck3D : MonoBehaviour
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
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private Vector3 boxSize = Vector3.one;
        [SerializeField] private LayerMask groundMask;

        [Title("Info")]
        [SerializeField] [ReadOnly] private bool isGrounded;
        [SerializeField] [ReadOnly] private List<Collider> colliders = new();

        private void Awake() => isGrounded = false;

        private void Update() => CheckGround();

        private void CheckGround()
        {
            colliders.Clear();
            colliders = Physics.OverlapBox(
                    transform.position + offset,
                    boxSize / 2,
                    transform.rotation,
                    groundMask).ToList();

            IsGrounded = colliders.Count > 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position + offset, boxSize);
        }
    }
}
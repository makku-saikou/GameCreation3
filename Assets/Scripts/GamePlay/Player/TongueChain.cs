// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: TongueChain.cs
// Description:
// -------------------------------------------------

using System;
using UnityEngine;

namespace GamePlay.Player
{
    public class TongueChain : MonoBehaviour
    {
        private SpringJoint2D _springJoint2D;
        public SpringJoint2D SpringJoint2D => _springJoint2D;
        private float _distance;
        private void Start()
        {
            _springJoint2D = GetComponent<SpringJoint2D>();
            _distance = _springJoint2D.distance;
        }
        private void Update()
        {
            ResetJoint();
        }
        
        public void ResetJoint()
        {
            _springJoint2D.distance = _distance;
        }
    }
}
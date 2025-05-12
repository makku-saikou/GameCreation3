// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_12
// Description:
// -------------------------------------------------

using System;
using UnityEngine;
using System.Collections;

namespace PurpleTest
{
    public class TestMoveTarget : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float time;
        private int _direction = 1;

        private void Start()
        {
            StartCoroutine(DoMove());
        }

        private void Update()
        {
            transform.position += new Vector3(speed * Time.deltaTime * _direction, 0, 0);
        }

        private IEnumerator DoMove()
        {
            while (true)
            {
                _direction = -_direction;
                yield return new WaitForSeconds(time);
            }
        }
        
    }
}
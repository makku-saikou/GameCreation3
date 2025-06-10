// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_10
// Description:
// -------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIBackGround : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> backgrounds;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float offset = 10f;
        private float _initPos;

        private void Start()
        {
            _initPos = backgrounds[0].anchoredPosition.x;
        }

        private void Update()
        {
            for (int i = 0; i < backgrounds.Count; i++)
            {
                backgrounds[i].anchoredPosition += Vector2.left * ((speed + i * offset) * Time.deltaTime);
                if (backgrounds[i].anchoredPosition.x <= 0)
                {
                    backgrounds[i].anchoredPosition = new Vector2(_initPos, backgrounds[i].anchoredPosition.y);
                }
            }
        }
    }
}
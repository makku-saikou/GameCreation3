// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_16
// Description:
// -------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FlowerPanel : MonoBehaviour
    {
        [SerializeField] private Image flowerImage;
        [SerializeField] private RectTransform theTransform;
        private float _target;
        private Action _callback;
        private float _speed = 1000f;
        private bool _isPlaying;
        public bool Enabled
        {
            get => flowerImage.enabled;
            set => flowerImage.enabled = value;
        }

        private void Update()
        {
            DoMove();
        }

        public void Move(float from, float to, Action callback = null, float speed = 1000f)
        {
            theTransform.anchoredPosition = new Vector2(from, theTransform.anchoredPosition.y);
            _target = to;
            _callback = callback;
            _speed = speed;
            _isPlaying = true;
        }

        private void DoMove()
        {
            if (!_isPlaying) return;
            float currentX = theTransform.anchoredPosition.x;
            currentX = Mathf.MoveTowards(currentX, _target, _speed * Time.deltaTime);
            theTransform.anchoredPosition = new Vector2(currentX, theTransform.anchoredPosition.y);
            if (Mathf.Approximately(currentX, _target))
            {
                _callback?.Invoke();
                _callback = null;
                _isPlaying = false;
            }
        }
    }
}
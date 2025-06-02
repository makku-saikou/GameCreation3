using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Manager
{
	public class BackgroundManager : MonoBehaviour
	{
		[Serializable]
		public class Background
		{
			public SpriteRenderer background;
			public Vector2 speed = Vector2.one;
			[VerticalGroup("Info")] [ReadOnly] public bool isInitialized = false;

			public Transform Transform => background.transform;

			public void Init()
			{
				if (!background || !background.sprite || !background.sprite.texture) return;
				isInitialized = true;
			}
		}

		[InfoBox("speed是背景相对于相机的移动速度，如果为0则静止，1则完全跟随相机")]

		[SerializeField] [TableList(AlwaysExpanded = true, DrawScrollView = false)]
		private List<Background> backgrounds = new();

		[SerializeField] private Transform cameraTransform;
		[SerializeField] private Camera mainCamera;
		private Vector3 _lastCameraPosition;

		private void Start()
		{
			_lastCameraPosition = cameraTransform.position;
			foreach (var bg in backgrounds) bg.Init();
		}

		private void Update()
		{
			UpdateBackgrounds();
		}

		private void LateUpdate()
		{
			_lastCameraPosition = cameraTransform.position;
		}

		private void UpdateBackgrounds()
		{
			if (!cameraTransform) return;

			var delta = new Vector2(
				cameraTransform.position.x - _lastCameraPosition.x,
				cameraTransform.position.y - _lastCameraPosition.y);

			foreach (var bg in backgrounds.Where(bg => bg.isInitialized))
			{
				bg.Transform.position += new Vector3(delta.x * bg.speed.x, delta.y * bg.speed.y, 0);
			}
		}
	}
}

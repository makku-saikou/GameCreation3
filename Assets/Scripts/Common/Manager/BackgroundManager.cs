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
			public float scaleFactor = 1.0f;
			[VerticalGroup("Info")] [ReadOnly] public bool isInitialized = false;

			[HideInInspector] public Vector2 textureUnitSize;
			[HideInInspector] public Vector2 initialSpriteSize;

			public Transform Transform => background.transform;

			public void Init()
			{
				if (!background || !background.sprite || !background.sprite.texture) return;

				textureUnitSize = new Vector2(
					background.sprite.texture.width / background.sprite.pixelsPerUnit * Transform.localScale.x,
					background.sprite.texture.height / background.sprite.pixelsPerUnit * Transform.localScale.y
				);
				var size = background.sprite.bounds.size;
				initialSpriteSize = new Vector2(size.x / Transform.localScale.x, size.y / Transform.localScale.y);
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

			var cameraHeight = mainCamera.orthographicSize * 2;
			var cameraWidth = cameraHeight * mainCamera.aspect;

			foreach (var bg in backgrounds.Where(bg => bg.isInitialized))
			{
				bg.Transform.position += new Vector3(delta.x * bg.speed.x, delta.y * bg.speed.y, 0);

				float scaleRatioX = cameraWidth / bg.initialSpriteSize.x;
				float scaleRatioY = cameraHeight / bg.initialSpriteSize.y;
				float targetScale = Mathf.Max(scaleRatioX, scaleRatioY) * bg.scaleFactor;
				bg.Transform.localScale = new Vector3(targetScale, targetScale, 1);
			}


		}
	}
}

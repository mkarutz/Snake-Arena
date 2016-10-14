using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace AppAdvisory.SharingSystem
{
	public class ScreenshotElement : MonoBehaviour
	{
		public RectTransform m_rect;
		public RectTransform rect
		{
			get
			{
				return GetComponent<RectTransform>();
			}
		}

		public CanvasGroup parent;
		public Button button;
		public Image image;
		public Text text;

		public Vector2 defaultPosition;
		public Quaternion defaultRotation;
		public Vector2 defaultSizeDelta;


		void Awake()
		{
			defaultRotation = rotation;
			defaultPosition = localPosition;
			defaultSizeDelta = sizeDelta;
		}

		public Quaternion rotation
		{
			get
			{
				return rect.rotation;
			}

			set
			{
				rect.rotation = value;
			}
		}


		public Vector2 localPosition
		{
			get
			{
				return rect.localPosition;
			}

			set
			{
				rect.localPosition = value;
			}
		}


		public Vector2 sizeDelta
		{
			get
			{
				return rect.sizeDelta;
			}

			set
			{
				rect.sizeDelta = value;
			}
		}

		public Vector2 localScale
		{
			get
			{
				return rect.localScale;
			}

			set
			{
				rect.localScale = value;
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

using App.Math;
using App.Utils;

using UniRx;

namespace App.View.Transmitter
{
	public class ControlCircle : MonoBehaviour,
		IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
	{
		public bool AutoCenterX;
		public bool AutoCenterY;
		public GameObject Knob;
		public float Radius;
		public Vector3 Pid;

		public Vector2 Output;

		private void Awake()
		{
			_rc = GetComponent<RectTransform>();
			_rcKnob = Knob.GetComponent<RectTransform>();
			_knobStart = _rcKnob.anchoredPosition;
		}

		private void Start()
		{
		}

		private void Update()
		{
			var k = _rcKnob.anchoredPosition;
			var c = _rc.anchoredPosition;
			var d = k - c;
			Output = d;//new Vector2(-.5f, -0.5f) + d/Radius*2.0f;
		}

		private void FixedUpdate()
		{
			if (!_dragging)
				RecenterSticks();
		}

		void RecenterSticks()
		{
			_pidLeftRight.SetPid(Pid);
			_pidUpDown.SetPid(Pid);

			var dt = Time.fixedDeltaTime;
			var ap = _rcKnob.anchoredPosition;

			var deltaLR = _pidLeftRight.Calculate(0, ap.x, dt);
			var deltaUD = AutoCenterY ? _pidUpDown.Calculate(0, ap.y, dt) : 0;

			var delta = new Vector2(deltaLR, deltaUD);
			_rcKnob.anchoredPosition += delta;
		}

		Vector2 GetCenteredPosition(PointerEventData eventData)//, Vector2 pos)
		{
			var sp = eventData.pointerCurrentRaycast.screenPosition;

			return Vector2.zero;
		}
        
		Camera _cam;
        public void OnPointerDown(PointerEventData eventData)
		{
			_cam = eventData.pressEventCamera;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_dragging = true;
		}

		public void OnDrag(PointerEventData eventData)
		{
			// these could be stored in Awake()
			Vector2 center = _cam.WorldToScreenPoint(transform.position);
			var width = _rc.rect.width;
			var height = _rc.rect.height;

			Vector2 sp = eventData.pointerCurrentRaycast.screenPosition;
			var delta = sp - center;
			var scaled = new Vector2(delta.x/width, delta.y/height);
			var len = Mathf.Clamp(delta.magnitude, 0, Radius);
			_rcKnob.anchoredPosition = _knobStart + delta.normalized*len;
		}
        
		public void OnEndDrag(PointerEventData eventData)
		{
			_dragging = false;
		}

		private static Vector2 getScreenPosition(Vector2 screen)
		{
			// Get current position on screen
			Vector3 p = Camera.main.ScreenToWorldPoint(screen);
			return new Vector2(p.x, p.y);
		}

		private RectTransform _rc;
		private RectTransform _rcKnob;
		private Vector2 _knobStart;

		private PidScalarController _pidLeftRight = new PidScalarController();
		private PidScalarController _pidUpDown = new PidScalarController();

		private bool _dragging;
	}
}


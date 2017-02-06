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

namespace App.UI.Transmitter
{
	public class ControlCircle : MonoBehaviour,
		IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
	{
		public bool AutoCenters;
		public GameObject Knob;
		public float Radius;

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
		}

		private void FixedUpdate()
		{
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
			// var rel = GetCenteredPosition(eventData);
			// Debug.Log("BeginDrag at " + rel);
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
			var len = Mathf.Clamp(delta.magnitude, 0, 400);
			_rcKnob.anchoredPosition = _knobStart + delta.normalized*len;
		}
        
		public void OnEndDrag(PointerEventData eventData)
		{
			// var rel = eventData.position - _parentRc.anchoredPosition;
			// Debug.Log("OnEndDrag at " + rel);
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
	}
}


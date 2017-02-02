using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

using App.Math;
using App.Utils;

using UniRx;

namespace App.FixedWing
{
	public class Transmitter : MonoBehaviour 
	{
		public enum EChanel
		{
			THR,
			AIL,
			ELE,
			RUD,
		};

		// all in range 0..1
		public float THR;
		public float AIL;
		public float ELE;
		public float RUD;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
			var dt = Time.deltaTime;
			var scale = 0.1f;
			var delta = 0.0f;
			if (Input.GetKey(KeyCode.W))
				delta += scale*dt;
			if (Input.GetKey(KeyCode.S))
				delta -= scale*dt;

			THR += delta;
			THR = Mathf.Clamp01(THR);
		}

		private void FixedUpdate()
		{
		}
	}
}


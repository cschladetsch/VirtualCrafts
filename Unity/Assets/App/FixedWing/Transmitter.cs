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

			ReadTHR(dt);
			ReadELE(dt);	
		}

		private void ReadTHR(float dt)
		{
			var scale = 0.1f;	// TODO: expo curves
			var delta = 0.0f;
			if (Input.GetKey(KeyCode.W))
				delta += scale*dt;
			if (Input.GetKey(KeyCode.S))
				delta -= scale*dt;

			THR += delta;
			THR = Mathf.Clamp01(THR);
		}

		private void ReadELE(float dt)
		{
			var scale = 0.1f;	// TODO: expo curves
			var delta = 0.0f;
			if (Input.GetKey(KeyCode.I))
				delta += scale*dt;
			if (Input.GetKey(KeyCode.K))
				delta -= scale*dt;

			ELE += delta;
			ELE = Mathf.Clamp01(ELE);
		}

		private void FixedUpdate()
		{
		}
	}
}


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

		/*
		W/S = THR
		I/K = ELE
		J/L = AIR
		Q/E = RUD
		*/
		private void Update()
		{
			var dt = Time.deltaTime;

			ReadTHR(dt);
			ReadELE(dt);	
			ReadAIL(dt);	
			ReadRUD(dt);	
		}

		private void ReadRUD(float dt)
		{
			var scale = 0.1f;	// TODO: expo curves
			var delta = 0.0f;
			if (Input.GetKey(KeyCode.Q))
				delta += scale*dt;
			if (Input.GetKey(KeyCode.E))
				delta -= scale*dt;

			RUD += delta;
			RUD = Mathf.Clamp01(RUD);
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
			THR = Mathf.Clamp(THR, 0, 9000);
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

		// left/right on right stick of mode-2 Tx
		private void ReadAIL(float dt)
		{
			var scale = 0.1f;	// TODO: expo curves
			var delta = 0.0f;
			if (Input.GetKey(KeyCode.J))
				delta -= scale*dt;
			if (Input.GetKey(KeyCode.L))
				delta += scale*dt;

			AIL += delta;
			AIL = Mathf.Clamp01(AIL);
		}

		private void FixedUpdate()
		{
		}
	}
}


﻿using System;
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
	public enum EChannel
	{
		THR,
		AIL,
		ELE,
		RUD,
	};

	public class Transmitter : MonoBehaviour 
	{
		// all in range 0..1
		public float THR;
		public float AIL;
		public float ELE;
		public float RUD;

		public AnimationCurve[] Expos = new AnimationCurve[4];

		public int TraceLevel;

		private void Awake()
		{
			TraceLevel = 0;
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

			if (TraceLevel > 1) DrawGraphs();
		}

		void DrawGraphs()
		{
			DebugGraph.Log("THR", (double)THR, Color.red);
			string r = "RUD";
			DebugGraph.Log(r, (double)RUD, Color.green);
			DebugGraph.Log("ELE", (double)ELE, Color.blue);
			DebugGraph.Log("AIL", (double)AIL, Color.yellow);
		}

		private void ReadRUD(float dt)
		{
			var scale = GetScale(EChannel.RUD, RUD);
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
			var scale = GetScale(EChannel.THR, THR);
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
			var scale = GetScale(EChannel.ELE, ELE);
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
			var scale = GetScale(EChannel.AIL, AIL);
			var delta = 0.0f;
			if (Input.GetKey(KeyCode.J))
				delta -= scale*dt;
			if (Input.GetKey(KeyCode.L))
				delta += scale*dt;

			AIL += delta;
			AIL = Mathf.Clamp01(AIL);
		}

		private float GetScale(EChannel channel, float cur)
		{
			var curve = Expos[(int)channel];
			return curve.Evaluate(cur);
		}

		private void FixedUpdate()
		{
		}
	}
}


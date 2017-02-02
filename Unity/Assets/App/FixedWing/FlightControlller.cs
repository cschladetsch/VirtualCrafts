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
	public class FlightControlller : MonoBehaviour 
	{
		public Transmitter Transmitter;

		public Motor Motor;
		public ControlSurface LeftAileron;
		public ControlSurface RightAileron;
		public ControlSurface Rudder;
		public ControlSurface LeftElevator;
		public ControlSurface RightElevator;

		public float MaxThrottleRpm = 2000;
		
		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
			Motor.DesiredRpm = Mathf.Clamp01(Transmitter.THR)*MaxThrottleRpm;

			UpdateElevators();
		}

		void UpdateElevators()
		{
			var raw = Mathf.Clamp01(Transmitter.ELE);	// input from Tx in [0..1] where 0.5 means centered
			var scaled = raw - 0.5f;

			// val of 0 means no throw on ELE
			// val of -0.5 means full pitch down (nose drops)
			// val of 0.5 means full pitch up (nose rises)

			// for now, we treat the elevators as a unit.
			// we are not modelling a jet fighter.
			var maxThrow = LeftElevator.MaxThrow;
			var val = scaled*2*maxThrow;		// *2 because we can be - or + max

			Assert.IsTrue(val >= -maxThrow && val <= maxThrow);

			LeftElevator.DesiredAngle = val;
			RightElevator.DesiredAngle = val;
		}

		private void FixedUpdate()
		{
		}
	}
}

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
	public class FlightController : MonoBehaviour 
	{
		public Transmitter Transmitter;

		public Motor Motor;
		public ControlSurface LeftAileron;
		public ControlSurface RightAileron;
		public ControlSurface Rudder;
		public ControlSurface LeftElevator;
		public ControlSurface RightElevator;

		public int TraceLevel;

		private void Awake()
		{
			TraceLevel = 1;
		}

		private void Start()
		{
		}

		private void Update()
		{
			UpdateThrottle();
			UpdateElevators();
			UpdateAirlerons();
			UpdateRudder();

			if (TraceLevel > 1) DrawGraphs();
		}

		void DrawGraphs()
		{
			DebugGraph.Log("Rpm", Motor.CurrentRpm);
			DebugGraph.Log("LeftAil", LeftAileron.Angle);
			DebugGraph.Log("RightAil", RightAileron.Angle);
			DebugGraph.Log("Rudder", Rudder.Angle);
			DebugGraph.Log("Elevator", LeftElevator.Angle);
		}

		private void UpdateRudder()
		{
			var raw = Mathf.Clamp01(Transmitter.RUD);	// [0..1]
			var scaled = raw - 0.5f;					// [-0.5..0.5]
			Rudder.DesiredAngle = scaled*2*Rudder.MaxThrow;
		}

		private void UpdateAirlerons()
		{
			var raw = Mathf.Clamp01(Transmitter.AIL);	// input from Tx in [0..1] where 0.5 means centered
			var scaled = raw - 0.5f;

			// val of 0 means no throw on AIL
			// val of -0.5 means full roll left
			// val of 0.5 means full roll right

			// share throws for both AIL
			var maxThrow = RightAileron.MaxThrow;
			var val = scaled*2*maxThrow;		// *2 because we can be - or + max

			Assert.IsTrue(val >= -maxThrow && val <= maxThrow);

			LeftAileron.DesiredAngle = val;
			RightAileron.DesiredAngle = -val;
		}

		private void UpdateThrottle()
		{
			var max = Motor.MaxThrottleRpm;
			Motor.DesiredRpm = Mathf.Clamp(Transmitter.THR, 0, max);
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


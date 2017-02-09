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
		public Body Body;
		public Motor Motor;

		public Vector3[] Pids = new Vector3[4];
		public ControlSurface LeftAileron;
		public ControlSurface RightAileron;
		public ControlSurface Rudder;
		public ControlSurface Elevator;

		public int TraceLevel;

		private void Awake()
		{
			TraceLevel = 1;
		}

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			float dt = Time.fixedDeltaTime;

			WritePids();

			UpdateInput();

			Stabilise(dt);

			if (TraceLevel > 1) DrawGraphs();
		}

		void WritePids()
		{
			for (var n = 0; n < 4; ++n)
			{
				_pidControlllers[n].SetPid(Pids[n]);
			}
		}

		void Stabilise(float dt)
		{
			var euler = Body.transform.rotation.eulerAngles;

			StabiliseAilerons(euler, dt);
		}

		void StabiliseAilerons(Vector3 euler, float dt)
		{
			var desired = LeftAileron.Angle;
			var actual = euler.z;

			var delta = _pidControlllers[(int)EChannel.AIL].Calculate(desired, actual, dt);

			Debug.LogFormat("Desired: {0}, Delta:{1}", desired, actual);

			LeftAileron.DesiredAngle += delta/2.0f;
			RightAileron.DesiredAngle -= delta/2.0f;
		}

		void UpdateInput()
		{
			UpdateThrottleInput();
			UpdateElevatorInput();
			UpdateAirleronsInput();
			UpdateRudderInput();
		}

		void DrawGraphs()
		{
			DebugGraph.Log("Rpm", Motor.CurrentRpm);
			DebugGraph.Log("LeftAil", LeftAileron.Angle);
			DebugGraph.Log("RightAil", RightAileron.Angle);
			DebugGraph.Log("Rudder", Rudder.Angle);
			DebugGraph.Log("Elevator", Elevator.Angle);
		}

		private void UpdateRudderInput()
		{
			var raw = Mathf.Clamp01(Transmitter.RUD);	// [0..1]
			var scaled = raw - 0.5f;					// [-0.5..0.5]
			Rudder.DesiredAngle = scaled*2*Rudder.MaxThrow;
		}

		private void UpdateAirleronsInput()
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

		private void UpdateThrottleInput()
		{
			var max = Motor.MaxThrottleRpm;
			Motor.DesiredRpm = Mathf.Clamp(Transmitter.THR, 0, max);
		}

		void UpdateElevatorInput()
		{
			var raw = Mathf.Clamp01(Transmitter.ELE);	// input from Tx in [0..1] where 0.5 means centered
			var scaled = raw - 0.5f;

			// val of 0 means no throw on ELE
			// val of -0.5 means full pitch down (nose drops)
			// val of 0.5 means full pitch up (nose rises)

			// for now, we treat the elevators as a unit.
			// we are not modelling a jet fighter.
			var maxThrow = Elevator.MaxThrow;
			var val = scaled*2*maxThrow;		// *2 because we can be - or + max

			Assert.IsTrue(val >= -maxThrow && val <= maxThrow);

			Elevator.DesiredAngle = val;
			Elevator.DesiredAngle = val;
		}

		private PidScalarController[] _pidControlllers = new PidScalarController[4];
	}
}


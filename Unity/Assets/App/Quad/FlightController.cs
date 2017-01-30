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

namespace App.Quad
{
	public class FlightController : MonoBehaviour 
	{
		public enum EControl
		{
			THR = 0,
			RUD,
			AIR,
			ELE,
		}

		[Flags]
		public enum EMode
		{
			None = 0,
			Grounded = 1,
			AltitudeHold = 2,
			AttitudeHold = 4,
			ReturnToHome = 8,
			Follow = 16,
			Hover = AltitudeHold | AttitudeHold,
		}

		public Body Body;

		public Motor FL, FR, RL, RR;
		public Motor[] Motors { get { return _motors; } }

		public EMode Mode;

		// 0 = THR	Throttle, or overall rpm gain for blades
		// 1 = RUD	Rudder. Control around vertical axis - YAW
		// 2 = ELE	Pitch forward and move forward, or pitch back and move back
		// 3 = AIR	Tilt left and move left, or tilt right and move right
		public float[] Inputs = new float[4];
		public float[] Trims = new float[4];
		public float[] MotorRpms = new float[4];

		public float ForceGizmoScale = 5;

		private static int TraceLevel;

		private void Awake()
		{
			TraceLevel = 2;

			_motors = Body.GetComponentsInChildren<Motor>();
			_rigidBody = Body.GetComponent<Rigidbody>();
			Assert.AreEqual(4, _motors.Length);

			// _orientation = GetComponent<Sensor.OrientationSensor>();
			// if (_orientation != null)
			// {
			// 	 _orientation.Euler
			// 	 	.Subscribe(r => Debug.Log("change: " + r))
			// 		.AddTo(this);
			// }
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		// TODO: this could all be cleaner and clearer with 
		// a single Flow.Channel<AppliedForce> given to a
		// number of coros.
		private void FixedUpdate()
		{
			ApplyForces(GatherForces());
		}

		List<AppliedForce> GatherForces()
		{
			var forces = new List<AppliedForce>();

			forces.AddRange(ApplyForcesFromBlades());

			forces.AddRange(GetWindForces());

			forces.AddRange(GetRainForces());

			return forces;
		}

		void ApplyForces(IList<AppliedForce> forces)
		{
			foreach (var force in forces)
			{
				_rigidBody.AddForceAtPosition(force.Force, force.Where, ForceMode.Impulse);
			}
			
			if (TraceLevel > 1) DrawTotalForceVector(forces);
		}

		IEnumerable<AppliedForce> GetWindForces()
		{
			yield break;
		}

		IEnumerable<AppliedForce> GetRainForces()
		{
			yield break;
		}

		List<AppliedForce> ApplyForcesFromBlades()
		{
			var forces = new List<AppliedForce>();
			foreach (var m in _motors)
			{
				forces.Add(new AppliedForce(m.WorldForce, m.transform.position));
			}
			return forces;
		}

		void DrawTotalForceVector(IEnumerable<AppliedForce> forces)
		{
			var forceTotal = Vector3.zero;
			var posTotal = Vector3.zero;
			var count = 0;
			foreach (var f in forces)
			{
				forceTotal += f.Force;
				posTotal += f.Where;
				++count;
			}

			float numForces = count;
			var center = posTotal/numForces;
			var force = forceTotal/numForces;

			Debug.DrawLine(center, center + force*ForceGizmoScale, Color.yellow, 0, false);
		}

		struct AppliedForce
		{
			public Vector3 Force;
			public Vector3 Where;

			public AppliedForce(Vector3 f, Vector3 w)
			{
				Force = f;
				Where = w;
			}

			// public static AppliedForce operator /(AppliedForce left, float scale)
			// {
			// 	return new AppliedForce(left.Force/scale, )
			// }
			// public static AppliedForce operator +(AppliedForce left, AppliedForce right)
			// {
			// 	return new AppliedForce(left.Force + right.Force, left.Where + (right.Where - left.Where));
			// }
		}

		private Motor[] _motors;
		private Rigidbody _rigidBody;

		private Sensor.OrientationSensor _orientation;
	}
}


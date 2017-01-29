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

namespace App
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

		public Motor FL, FR, RL, RR;

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

			_rigidBody = GetComponent<Rigidbody>();
			_motors = GetComponentsInChildren<Motor>();
			Assert.AreEqual(4, _motors.Length);
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void FixedUpdate()
		{
			var forces = new AppliedForce[4];
			var n = 0;
			foreach (var m in _motors)
			{
				_rigidBody.AddForceAtPosition(m.WorldForce, m.transform.position, ForceMode.Impulse);

				forces[n++] = new AppliedForce(m.WorldForce, m.transform.position);
			}

			if (TraceLevel > 1)
			{
				var forceTotal = Vector3.zero;
				var posTotal = Vector3.zero;
				foreach (var f in forces)
				{
					forceTotal += f.Force;
					posTotal += f.Where;
				}

				var center = posTotal/4.0f;
				var force = forceTotal/4.0f;
				Debug.DrawLine(center, center + force*ForceGizmoScale, Color.yellow, 0, false);
			}
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
		}

		private Motor[] _motors;
		private Rigidbody _rigidBody;
	}
}


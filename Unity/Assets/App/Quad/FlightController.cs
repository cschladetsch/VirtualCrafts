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

namespace App.Quad
{
	public class FlightController : MonoBehaviour 
	{
		public Body Body;
		public Transmitter Transmitter;

		public Motor FL, FR, RL, RR;

		public float[] MotorRpms = new float[4];

		public float ForceGizmoScale = 5;

		private static int TraceLevel;

		[Flags]
		public enum EConstraints
		{
			None = 0,
			Attitude = 1,
			Height = 2,
			Speed = 4,
		}
		public EConstraints[] Constraints = new EConstraints[4];

		public enum EMode
		{
			None,
			ReturnToHome,
			Hover,
			Follow,
			CircleAround,
			FollowPath,
		}
		public EMode Mode;

		public float DesiredHeight;

		public Vector3 DefaultMotorPidValues = new Vector3(0.8f, 0.5f, 0.1f);
		public Motor[] Motors;

		public Vector3 QuaternionControllerPID = new Vector3(.4f, .2f, .01f);
		public PidQuaternionController PidQuaternionController;

		public Sensor.GyroscopeSensor Gyro;

		private void Awake()
		{
			TraceLevel = 5;

			_motors = Body.GetComponentsInChildren<Motor>();
			Assert.AreEqual(_motors.Count, 4);

			_rigidBody = Body.GetComponent<Rigidbody>();
			Assert.IsNotNull(_rigidBody);

			PidQuaternionController = new PidQuaternionController(QuaternionControllerPID.x, QuaternionControllerPID.y, QuaternionControllerPID.z);

			var pid = DefaultMotorPidValues;
			for (int n = 0; n < 4; ++n)
			{
				Motors[n].PidController = new PidController(pid.x, pid.y, pid.z);
			}

			Gryo
				.Pitch
				.DistinctUntilChanged()
				.Subscribe(p => DealWithNewPitch(p))
				.AddTo(this);
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (TraceLevel < 5) return;

			foreach (var m in _motors)
			{
				DebugGraph.Log(m.gameObject.name, m.RevsPerMinute);
			}
		}

		// TODO: this could all be cleaner and clearer with 
		// a single Flow.Channel<AppliedForce> given to a
		// number of coros.
		private void FixedUpdate()
		{
			CalcMotorRpms();

			ApplyForces(GatherForces());
		}

		void CalcMotorRpms()
		{
			if (Mode == EMode.Hover)
			{
				// var dh = transform.position.y - DesiredHeight;
				// var error = dh;
				// var delta = _errors[0] - 
				
				// float res = PidControllers[0].ComputeOutput(error, delta, Time.fixedDeltaTime);
			}
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

				_rigidBody.AddTorque(force.Torque, ForceMode.Impulse);
			}
			
			if (TraceLevel > 1) DrawTotalForces(forces);
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
				forces.Add(new AppliedForce(m.WorldForce, m.transform.position, m.WorldTorque));
			}
			return forces;
		}

		void DrawTotalForces(IEnumerable<AppliedForce> forces)
		{
			var forceTotal = Vector3.zero;
			var torqueTotal = Vector3.zero;
			var posTotal = Vector3.zero;
			var count = 0;
			foreach (var f in forces)
			{
				forceTotal += f.Force;
				torqueTotal += f.Force;
				posTotal += f.Where;
				++count;
			}

			float numForces = count;
			var center = posTotal/numForces;
			var force = forceTotal/numForces;
			var torque = torqueTotal/numForces;

			Debug.DrawLine(center, center + force*ForceGizmoScale, Color.red, 0, false);
			Debug.DrawLine(center, center + torque*ForceGizmoScale, Color.blue, 0, false);
		}

		struct AppliedForce
		{
			public Vector3 Force;
			public Vector3 Torque;
			public Vector3 Where;

			public AppliedForce(Vector3 f, Vector3 w, Vector3 t)
			{
				Force = f;
				Where = w;
				Torque = t;
			}
		}

		private Rigidbody _rigidBody;

		private Sensor.AccellerometerSensor _accellerometer;
		private Sensor.AltimeterSensor _altimeter;
		private Sensor.GyroscopeSensor _gyro;
		private Sensor.OrientationSensor _orientation;
	}
}


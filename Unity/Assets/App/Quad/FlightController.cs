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
		public Body Body;
		public Transmitter Transmitter;

		public Motor FL, FR, RL, RR;
		public Motor[] Motors { get { return _motors; } }

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

		public Vector3[] PidControllerPIDs = new Vector3[4];
		public PidController[] PidControllers = new PidController[4];

		public Vector3 QuaternionControllerPID = new Vector3(.4f, .2f, .01f);
		public PidQuaternionController PidQuaternionController;

		private void Awake()
		{
			TraceLevel = 2;

			_motors = Body.GetComponentsInChildren<Motor>();
			_rigidBody = Body.GetComponent<Rigidbody>();

			Assert.AreEqual(4, _motors.Length);
			Assert.IsNotNull(_rigidBody);

			PidQuaternionController = new PidQuaternionController(QuaternionControllerPID.x, QuaternionControllerPID.y, QuaternionControllerPID.z);

			for (int n = 0; n < 4; ++n)
			{
				var pid = PidControllerPIDs[n];
				PidControllers[n] = new PidController(pid.x, pid.y, pid.z);
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
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
		}

		private Motor[] _motors;
		private Rigidbody _rigidBody;

		private Sensor.AccellerometerSensor _accellerometer;
		private Sensor.AltimeterSensor _altimeter;
		private Sensor.GyroscopeSensor _gyro;
		private Sensor.OrientationSensor _orientation;
	}
}


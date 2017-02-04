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
		public float[] MotorRpms = new float[4];
		public int TraceLevel;

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
		public Vector3 DesiredEuler = Vector3.zero;

		public Vector3 DefaultMotorPidValues = new Vector3(0.8f, 0.5f, 0.1f);
		public Motor[] Motors;

		// public Vector3 QuaternionControllerPID = new Vector3(.4f, .2f, .01f);
		// public PidQuaternionController PidQuaternionController;

		// public ReactiveProperty<Vector3> Accellerometer = new ReactiveProperty<Vector3>();
		// public ReactiveProperty<float> Height = new ReactiveProperty<float>();
		// public ReactiveProperty<Quaternion> Rotation = new ReactiveProperty<Quaternion>();
		// public ReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();

		private void Awake()
		{
			TraceLevel = 10;

			_motors = Body.GetComponentsInChildren<Motor>();
			Assert.AreEqual(_motors.Length, 4);

			PrepareMotors();
		}

		private void PrepareMotors()
		{
			var pid = DefaultMotorPidValues;
			for (int n = 0; n < 4; ++n)
			{
				Motors[n].PidController = new PidScalarController(pid.x, pid.y, pid.z);
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (TraceLevel < 3) return;
		}

		private void FixedUpdate()
		{
			RunFlightController();
		}

		public float AngleScale = 1;
		public float HeightScale = 2;

		void RunFlightController()
		{
			// var frame = new Frame(transform);
			// var delta = frame - _thisFrame;
			// var deltaDelta = _thisFrame - _lastFrame;

			// find world vectors for each motor position
			var vFL = FL.transform.position - Body.CenterOfMass.transform.position;
			var vFR = FR.transform.position - Body.CenterOfMass.transform.position;
			var vRL = RL.transform.position - Body.CenterOfMass.transform.position;
			var vRR = RR.transform.position - Body.CenterOfMass.transform.position;

			// get the absolute height displacement for each motor relative to center of mass
			var aFL = Mathf.Abs(vFL.y);
			var aFR = Mathf.Abs(vFR.y);
			var aRL = Mathf.Abs(vRL.y);
			var aRR = Mathf.Abs(vRR.y);

			// get the signs
			var sFL = Mathf.Sign(vFL.y);
			var sFR = Mathf.Sign(vFR.y);
			var sRL = Mathf.Sign(vRL.y);
			var sRR = Mathf.Sign(vRR.y);

			// assume FL motor is most incorrect
			var max = 0;
			var dist = aFL;

			// test FR
			if (aFR > dist)
			{
				max = 1;
				dist = aFR;
			}

			// test rear left
			if (aRL > dist)
			{
				max = 2;
				dist = aRL;
			}

			// test rear right
			if (aRR > dist)
			{
				max = 3;
				dist = aRR;
			}

			var dt = Time.fixedDeltaTime;

			// speed up or slow down according to which motor is most distant from vertical
			// do the opposite to the opposite motor
			//
			// eventually we'd like to change them all a little...
			float s = dist/2*AngleScale*dt;
			switch (max)
			{
				// front left motor is dipping the most: speed it up and slow down opposite motor
				case 0:
					if (sFL < 0)
						FL.DesiredRpm += s;
					// else
					// 	RR.DesiredRpm += s;
					break;
				
				// etc...
				case 1:
					if (sFR < 0)
						FR.DesiredRpm += s;
					// else
					// 	RL.DesiredRpm += s;
					break;
				case 2:
					if (sRL < 0)
						RL.DesiredRpm += s;
					// else
					// 	FR.DesiredRpm += s;
					break;
				case 3:
					if (sRR < 0)
						RR.DesiredRpm += s;
					// else
					// 	FL.DesiredRpm += s;
					break;
			}

			// account for height
			var bh = Body.transform.position.y;
			var dh = DesiredHeight - bh;
			var heightScale = dh*HeightScale*dt;
			foreach (var m in _motors)
			{
				m.DesiredRpm += heightScale;
			}
		}

		public float AngleToRpmCorrection = 1;

		void DrawBalanceBalance(Vector3 dir, float angle)
		{
			DebugGraph.Log("balx", dir.x);
			DebugGraph.Log("baly", dir.y);
			DebugGraph.Log("balz", dir.z);
			DebugGraph.Log("angle", angle);
		}

		struct Angles
		{
			public Vector3 Axes;

			public Angles(Transform tr)
			{
				Axes = tr.rotation.eulerAngles;
			}
		}

		void SampleTransmitter()
		{
		}

		Frame _lastFrame;
		// Frame _thisFrame;
		FrameDelta _deltaFrame;

		private Motor[] _motors;
	}
}


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
	public class ControlSurface : ForceProviderBehaviour
	{
		public Vector3 Axis;
		public float MaxThrow = 30;
		public Vector3 Pid = new Vector3(0.8f, 0.5f, 0.01f);
		public PidScalarController Controller = new PidScalarController();
		public float DesiredAngle;
		public float Angle;
		public Transform ForceApplication;

		public AppliedForce AppliedForce;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
			DrawForce();
			transform.localRotation = Quaternion.AngleAxis(Angle, Axis);
		}

		private void FixedUpdate()
		{
			Controller.P = Pid.x;
			Controller.I = Pid.y;
			Controller.D = Pid.z;

			float dt = Time.fixedDeltaTime;
			var delta = Controller.Calculate(DesiredAngle, Angle, dt);
			Angle += delta*dt;

			Angle = Mathf.Clamp(Angle, -MaxThrow, MaxThrow);
			var rot = Quaternion.AngleAxis(Angle, Axis);
			var motor = _flightController.Motor;
			if (motor.Rpm <= 0)
				return;

			var mag = RpmRelative.Evaluate(Mathf.Clamp01(motor.Rpm/motor.MaxThrottleRpm)); 
			AppliedForce = new AppliedForce(
				rot.eulerAngles*mag,
				ForceApplication.position);
		}
	}
}


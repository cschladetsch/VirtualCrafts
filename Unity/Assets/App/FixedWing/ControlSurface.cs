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

using ChaosCult.SceneLabels;

namespace App.FixedWing
{
	// a general control surface on the model
	public class ControlSurface : MonoBehaviour
	{
		// the axis that it rotates around
		public Vector3 Axis;

		// maximum angle in degrees
		public float MaxThrow = 30;

		// controller
		public Vector3 Pid = new Vector3(0.8f, 0.5f, 0.01f);
		public PidScalarController Controller = new PidScalarController();
		public float DesiredAngle;
		public float Angle;

		// how the force provided by this surface relates to the motor rpm
		public AnimationCurve RpmRelative = new AnimationCurve();
		public float RpmCurveScale = 1;
		public ForceProvider ForceProvider;

		public float ForceDrawScale = 2;
		public int TraceLevel;

		void Awake()
		{
			TraceLevel = 1;
		}

		public void Construct(Body body)
		{
			_body = body;
			ForceProvider = GetComponentInChildren<ForceProvider>();
		}

		private void Update()
		{
			transform.localRotation = Quaternion.AngleAxis(Angle, Axis);

			if (TraceLevel > 1) 
				Debug.DrawLine(
					ForceProvider.Where, 
					ForceProvider.Where + ForceProvider.Force*ForceDrawScale, 
					Color.blue, 0, false);
		}

		private void FixedUpdate()
		{
			ChangeAngle();

			ChangeMagnitude();
		}

		void ChangeAngle()
		{
			Controller.P = Pid.x;
			Controller.I = Pid.y;
			Controller.D = Pid.z;

			float dt = Time.fixedDeltaTime;
			var delta = Controller.Calculate(DesiredAngle, Angle, dt);
			Angle += delta;
			Angle = Mathf.Clamp(Angle, -MaxThrow, MaxThrow);
			ForceProvider.transform.localRotation = Quaternion.AngleAxis(Angle, Axis);
		}

		private void ChangeMagnitude()
		{
			var motor = _body.FlightController.Motor;
			var mag = RpmCurveScale*RpmRelative.Evaluate(Mathf.Clamp01(motor.Rpm/motor.MaxThrottleRpm)); 
			ForceProvider.Magnitude = mag;
		}

		/// <summary>
		/// Callback to draw gizmos that are pickable and always drawn.
		/// </summary>
		void OnDrawGizmos()
		{
			var fp = ForceProvider;
			// LabelsAccess.DrawLabel(fp.Where, fp.Where + ForceProvider.Force.ToString(), null);
			LabelsAccess.DrawLabel(gameObject, Angle.ToString(), null);
		}

		private Body _body;
	}
}


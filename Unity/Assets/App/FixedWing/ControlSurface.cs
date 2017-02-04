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
		// the axis that the surface rotates around
		public Vector3 Axis;

		// maximum angle in degrees
		public float MaxThrow = 30;

		// controller
		public Vector3 Pid = new Vector3(0.8f, 0.5f, 0.01f);
		public PidScalarController Controller = new PidScalarController();

		// the angle we want to be at
		public float DesiredAngle;

		// the current angl
		public float Angle;

		// where the force is provided, and the amount of torque
		public ForceProvider ForceProvider;

		public float ForceDrawScale = 2;
		public int TraceLevel;

		public void Construct(Body body)
		{
			_body = body;
			ForceProvider = GetComponentInChildren<ForceProvider>();
		}

		private void Update()
		{
			transform.localRotation = Quaternion.AngleAxis(Angle, Axis);

			if (TraceLevel > 1) 
			{
				// draw torque
				Debug.DrawLine(
					ForceProvider.Where, 
					ForceProvider.Where + ForceProvider.Torque*ForceDrawScale,
					Color.magenta, 0, false);

				// draw force
				Debug.DrawLine(
					ForceProvider.Where, 
					ForceProvider.Where + ForceProvider.Position*ForceDrawScale, 
					Color.blue, 0, false);
			}
		}

		/// <summary>
		/// Callback to draw gizmos that are pickable and always drawn.
		/// </summary>
		void OnDrawGizmos()
		{
			if (TraceLevel > 1)
			{
				var fp = ForceProvider;
				LabelsAccess.DrawLabel(fp.Where, fp.Where + ForceProvider.Force.ToString(), null);
				LabelsAccess.DrawLabel(gameObject, Angle.ToString(), null);
			}
		}

		private void FixedUpdate()
		{
			float dt = Time.fixedDeltaTime;
			var motor = _body.FlightController.Motor;
			var thrust = Mathf.Clamp01(motor.Rpm/motor.MaxThrottleRpm);		// normalise thrust

			ChangeAngle(dt);

			ChangeMagnitude(dt, thrust);

			ChangeTorque(dt, thrust);
		}

		// some control surfaces require specialised angle changes
		virtual protected void ChangeAngle(float dt)
		{
			UpdatePid();

			var delta = Controller.Calculate(DesiredAngle, Angle, dt);
			Angle += delta;
			Angle = Mathf.Clamp(Angle, -MaxThrow, MaxThrow);
			ForceProvider.transform.localRotation = Quaternion.AngleAxis(Angle, Axis);
		}

		protected void UpdatePid()
		{
			Controller.P = Pid.x;
			Controller.I = Pid.y;
			Controller.D = Pid.z;
		}

		private void ChangeMagnitude(float dt, float thrust)
		{
			var fp = ForceProvider;
			fp.Torque = fp.transform.forward*dt*fp.ThrustRelativeTorque.Evaluate(thrust);
		}

		void ChangeTorque(float dt, float thrust)
		{
			var fp = ForceProvider;
			var toCenter = _body.CenterOfMass.position - fp.transform.position;
			var tau = Vector3.Cross(toCenter, fp.transform.forward);
			fp.Torque = dt*fp.ThrustRelativeForce.Evaluate(thrust)*tau;
		}

		private Body _body;
	}
}


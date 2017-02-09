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

using Util.Extenstion;

namespace App.FixedWing
{
	// a general control surface on the model
	public class ControlSurface : MonoBehaviour
	{
		public EChannel Channel;

		// the axis that the surface rotates around
		public Vector3 RotationAxis;

		// maximum angle in degrees
		public float MaxThrow = 30;

		// controller
		public Vector3 Pid = new Vector3(0.8f, 0.5f, 0.01f);
		public PidScalarController Controller = new PidScalarController();

		// the angle we want to be at
		public float DesiredAngle;

		// the angle to correct from the flight-controller
		public float CorrectionAngle;

		// the current angle
		public float Angle;

		// where the force is provided, and the amount of torque
		public ForceProvider ForceProvider;

		public float ForceDrawScale = 2;
		public int TraceLevel;

		public void Construct(Body body)
		{
			_body = body;
		}

		/// <summary>
		/// Callback to draw gizmos that are pickable and always drawn.
		/// </summary>
		void OnDrawGizmos()
		{
			// if (TraceLevel > 1)
			// {
			// 	var fp = ForceProvider;
			// 	LabelsAccess.DrawLabel(fp.Where, fp.Where + ForceProvider.Force.ToString(), null);
			// 	LabelsAccess.DrawLabel(gameObject, Angle.ToString(), null);
			// }
		}

		public void Step(float dt)
		{
			var motor = _body.FlightController.Motor;
			ChangeAngle(dt);
			ChangeForce(dt, motor);
		}

		private void ChangeForce(float dt, Motor motor)
		{
			ForceProvider.Step(dt, motor.Thrust);
			DebugGraph.Log(gameObject.name + ": force=", ForceProvider.Force);
		}

		virtual protected void ChangeAngle(float dt)
		{
			// UpdatePid();

			// include the input from controller (DesiredAnle), and transmitter (CorrectionAngle)
			var delta = Controller.Calculate(DesiredAngle + CorrectionAngle, Angle, dt)*dt;
			Angle += delta;
			Angle = Mathf.Clamp(Angle, -MaxThrow, MaxThrow);
			transform.localRotation = Quaternion.AngleAxis(Angle, RotationAxis);
		}

		protected void UpdatePid()
		{
			Controller.P = Pid.x;
			Controller.I = Pid.y;
			Controller.D = Pid.z;
		}

		void ChangeTorque(float dt, float thrust)
		{
			var fp = ForceProvider;
			var toCenter = transform.forward - _body.CenterOfMass.position;
			var tau = Vector3.Cross(toCenter, fp.Force);
			fp.Torque = fp.TorqueScale*fp.ThrustRelativeForce.Evaluate(thrust)*tau;
		}

		private Body _body;
	}
}

namespace Util.Extenstion
{
	public static class StringExtension
	{
		public static string Format = "F3";

		public static string str(this Vector2 vec)
		{
			return vec.ToString(Format);
		}

		public static string str(this Vector3 vec)
		{
			return vec.ToString(Format);
		}

		public static string str(this Vector4 vec)
		{
			return vec.ToString(Format);
		}

		public static string str(this Quaternion quat)
		{
			return quat.ToString(Format);
		}
	}
}


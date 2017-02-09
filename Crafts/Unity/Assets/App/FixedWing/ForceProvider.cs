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
	// a behaviour that applies both a linear and turning force
	public class ForceProvider : MonoBehaviour 
	{
		// how the force provided by this surface relates to the overall thrust
		public AnimationCurve ThrustRelativeForce = new AnimationCurve();
		public float ForceScale;
		public Vector3 Force;	// readonly: TODO: custom insector

		// how the torque provided by this surface relates to the overall thrust
		public AnimationCurve ThrustRelativeTorque = new AnimationCurve();
		public float TorqueScale;
		// torque is always around the craft's center of mass
		public Vector3 Torque;	// readonly: TODO: custom inspector

		// the world-space position where the force is applied
		public Vector3 Position { get { return transform.position; } }

		#region Debug

		public Color ColorForce = Color.magenta;
		public Color ColorTorque = Color.yellow;
		public float GizmodMagnitudeTorque;
		public float GizmodMagnitudeForce;
		public int TraceLevel = 1;

		#endregion

		private void Awake()
		{
			TraceLevel = 2;
		}

		public void Step(float dt, Vector3 thrust)
		{
			UpdateForce(dt, thrust);
			UpdateTorque(dt, thrust);

			if (TraceLevel > 1) DrawForceAndTorque();
		}

		private void UpdateForce(float dt, Vector3 velocity)
		{
			var speed = velocity.magnitude;	
			Force = transform.forward
				*ForceScale					
				*ThrustRelativeForce.Evaluate(speed);

			Debug.LogFormat("{2}: fwd={0}, force={1}", transform.forward, Force, name);
		}

		private void UpdateTorque(float dt, Vector3 thrust)
		{
			// NFI yet
			// var fullTorque = transform.rotation*Torque;	// rotate a Vector3
			// Torque = ... how to scale/re-orient with thrust/speed?
			Torque = Vector3.zero;
		}

		private void DrawForceAndTorque()
		{
			Debug.DrawLine(
				transform.position, 
				transform.position + Force*GizmodMagnitudeForce, 
				ColorForce, 0);
			
			Debug.DrawLine(
				transform.position, 
				transform.position + Torque*GizmodMagnitudeTorque, 
				ColorTorque, 0);
		}
	}
}


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
		public Vector3 ForceDir;
		public Vector3 Force;

		// how the torque provided by this surface relates to the overall thrust
		public AnimationCurve ThrustRelativeTorque = new AnimationCurve();
		public float TorqueScale;
		public Vector3 Torque;

		public Vector3 Position { get { return transform.position; } }

		public Color ColorForce = Color.magenta;
		public Color ColorTorque = Color.yellow;
		public float GizmodMagnitudeTorque;
		public float GizmodMagnitudeForce;
		public int TraceLevel = 1;

		private void FixedUpdate()
		{
			Force = ForceDir*ForceScale*Time.fixedDeltaTime;
		}

		// private void Update()
		// {
		// 	if (TraceLevel > 0) 
		// 	{
		// 		Debug.DrawLine(
		// 			transform.position, 
		// 			transform.position + Force*GizmodMagnitudeForce, 
		// 			ColorForce, 0);
				
		// 		Debug.DrawLine(
		// 			transform.position, 
		// 			transform.position + Torque*GizmodMagnitudeTorque, 
		// 			ColorTorque, 0);
		// 	}
		// }
	}
}


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
	public class ForceProvider : MonoBehaviour 
	{
		public Color Color = Color.magenta;
		public float Magnitude;
		public float GizmodMagnitude;
		public Vector3 Torque;

		public Vector3 Where { get { return transform.position; } }
		public Vector3 Position { get { return transform.forward*Magnitude; } }

		public int TraceLevel = 1;

		private void Update()
		{
			if (TraceLevel > 0) 
			{
				Debug.DrawLine(
					transform.position, 
					transform.position + transform.forward*Magnitude*GizmodMagnitude, 
					Color, 0);
				DebugGraph.Log("RUD", Position*Magnitude);
			}

		}

		private void FixedUpdate()
		{
		}
	}
}


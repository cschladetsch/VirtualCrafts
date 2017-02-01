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
	public class Body : MonoBehaviour 
	{
		public FlightController FlightController;
		public GameObject CenterOfMass;

		int TraceLevel = 2;

		void Awake()
		{
			FlightController = GetComponentInChildren<FlightController>();
			Assert.IsNotNull(FlightController);
		}

		private void Update()
		{
			if (TraceLevel < 1) return;

			// var euler = FlightController.GyroSensor.Orientation.eulerAngles;
			// DebugGraph.Log("pitch", euler.x);
			// DebugGraph.Log("yaw", euler.y);
			// DebugGraph.Log("roll", euler.z);
		}
	}
}


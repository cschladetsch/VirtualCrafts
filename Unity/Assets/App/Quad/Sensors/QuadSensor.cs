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

namespace App.Quad.Sensor
{
	public class QuadSensor : MonoBehaviour 
	{
		private void Awake()
		{
			_flightController = GetComponentInParent<FlightController>();
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void FixedUpdate()
		{
		}

		protected FlightController _flightController;
	}
}


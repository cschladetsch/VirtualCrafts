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
	public class GyroscopeSensor : QuadSensor
	{
		public Quaternion Orientation;

		private void FixedUpdate()
		{
			Orientation = transform.rotation;
		}
	}
}


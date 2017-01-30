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
	public class OrientationSensor : QuadSensor
	{
		public IObservable<Vector3> Euler = new ReactiveProperty<Vector3>();
		// public IObservable<Quaternion> Orientation = new ReactiveProperty<Quaternion>();

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
			Euler.Publish(new Vector3(Time.time, 0, 0));
		}

		private void FixedUpdate()
		{
		}
	}
}


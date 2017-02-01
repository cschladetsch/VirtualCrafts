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
		public Quaternion Rotation { get { return _rotation.Value; } }

		public IObservable<Vector3> EulerAngles { get { return _rotation.Select( rot => rot.eulerAngles ); } }

		public IObservable<float> Pitch { get { return _rotation.Select( rot => rot.eulerAngles.x ); } }
		public IObservable<float> Yaw { get { return _rotation.Select( rot => rot.eulerAngles.y ); } }
		public IObservable<float> Roll { get { return _rotation.Select( rot => rot.eulerAngles.z ); } }

		private void FixedUpdate()
		{
			_rotation.Value = transform.rotation;
		}

		private QuaternionReactiveProperty _rotation = new QuaternionReactiveProperty();	
	}
}

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
	public class AccellerometerSensor : QuadSensor
	{
		public FrameDelta Delta0;
		public FrameDelta Delta1;
		public List<Frame> Frames = new List<Frame>();

		public Vector3 Velocity { get { return Delta0.Velocity; } }
		public Vector3 Accceleration { get { return Delta1.Velocity - Delta0.Velocity; } }

		public Quaternion AngularVelocity { get { return Delta0.Rotation; } }
		public Quaternion AngularAccceleration 
		{ 
			get { return Delta0.Rotation*Quaternion.Inverse(Delta1.Rotation); }
		}

		private void FixedUpdate()
		{
			Frames.Add(new Frame(transform));
			if (Frames.Count > 3)
				Frames.RemoveAt(0);

			switch (Frames.Count)
			{
				case 1: 
					Delta0 = Frames[0] - Frame.Identity;
					return;
				case 2: 
					Delta0 = Frames[0] - Frames[1];
					return;
				case 3: 
					Delta0 = Frames[0] - Frames[1];
					Delta1 = Frames[1] - Frames[2];
					return;
			}
		}
	}
}


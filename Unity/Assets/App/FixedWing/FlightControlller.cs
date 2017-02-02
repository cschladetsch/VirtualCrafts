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
	public class FlightControlller : MonoBehaviour 
	{
		public Transmitter Transmitter;

		public Motor Motor;
		public ControlSurface LeftAileron;
		public ControlSurface RightAileron;
		public ControlSurface Rudder;
		public ControlSurface LeftElevator;
		public ControlSurface RightElevator;

		public float MaxThrottleRpm = 2000;
		
		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
			Motor.DesiredRpm = Mathf.Clamp01(Transmitter.THR)*MaxThrottleRpm;
		}

		private void FixedUpdate()
		{
		}
	}
}


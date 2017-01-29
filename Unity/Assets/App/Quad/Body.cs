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

namespace App
{
	[RequireComponent(typeof(FlightController))]
	public class Body : MonoBehaviour 
	{
		public float LeftMotorRpm;
		public float RightMotorRpm;
		public float UpMotorRpm;
		public float DownMotorRpm;

		public bool OverrideUpDown;
		public bool OverrideLeftRight;

		private void Awake()
		{
			_fc = GetComponent<FlightController>();
		}

		private void Start()
		{
		}

		private void Update()
		{
			foreach (var m in _fc.Motors)
			{
				if (OverrideUpDown)
				{
					if (m.SpinDirection == Motor.ESpin.CW)
						m.RevsPerMinute = UpMotorRpm;
					else
						m.RevsPerMinute = DownMotorRpm;
				}

				// if (OverrideLeftRight)
				// {
				// 	_fc.RL.RevsPerMinute = RightMotorRpm;
				// }
			}
		}

		private void FixedUpdate()
		{
		}

		private FlightController _fc;
	}
}


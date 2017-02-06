using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

using App.Math;
using App.Utils;

using TMPro;

using UniRx;

namespace App.View.Transmitter
{
	public enum EChannel
	{
		THR,
		RUD,
		ELE,
		AIL,
		BIND,
		POWER,
		STOP,
		RTH,
		HOVER,
	}

	public enum EAxis
	{
		X, Y
	}

	public class ControlOutput : MonoBehaviour 
	{
		public ControlCircle StickBay;
		public EChannel Channel;
		public EAxis Axis;
		public TextMeshProUGUI ValueText;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			float val = 0;
			switch (Axis)
			{
				case EAxis.X:
					val = StickBay.Output.x;
					break;
				case EAxis.Y:
					val = StickBay.Output.y;
					break;
			}

			ValueText.text = string.Format("{0}", val.ToString("F1"));
		}
	}
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

using UniRx;

namespace App.Math
{
	public class PidScalarControllerBehaviour : MonoBehaviour
	{
		public float SetPoint = 0;
		public float P, I, D;

		private void Awake()
		{
			 _controller = new PidScalarController();
		}

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			_controller.P = P;
			_controller.I = I;
			_controller.D = D;
			
			var offset = _controller.Calculate(SetPoint, transform.position.x, Time.fixedDeltaTime);
			var p = transform.position;
			p.x += offset*Time.fixedDeltaTime;
			transform.position = p;

			DebugGraph.Log("val", p.x);
			DebugGraph.Log("offset", offset);
		}

		private PidScalarController _controller;
	}
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

using App.Math;

namespace App.Math
{
	public class PidVector3ControllerBehaviour : MonoBehaviour
	{
		public Vector3 SetPoint;

		// TODO: may want different co-efficients for the different axes
		public float P = 0.1f;
		public float I = 0.5f;
		public float D = 0.01f;

		private void Awake()
		{
			_controller = new PidVector3Controller(P, I, D);
			StartCoroutine(Run());
		}

		IEnumerator Run()
		{
			yield return new WaitForSeconds(3);
			Debug.Log("Startig");
			while (true) 
			{
				var offset = _controller.Calculate(SetPoint, transform.position, 0.1f);
				var p = transform.position;
				transform.position = p + offset;

				Debug.Log(offset);
				DebugGraph.Log(transform.position.x);
				DebugGraph.Log(transform.position.y);
				DebugGraph.Log(transform.position.z);

				yield return 0;
			}
		}

		private PidVector3Controller _controller;
	}
}


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

namespace App.Quad
{
	public class Body : MonoBehaviour 
	{
		public GameObject CenterOfMass;

		private void Update()
		{
			DebugGraph.Log("height", transform.position.y);
		}
	}
}


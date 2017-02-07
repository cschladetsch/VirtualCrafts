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
	public class PlaceMarkers : MonoBehaviour 
	{
		public GameObject MarkerPrefab;
		public Vector3 Displacement;		// distance between markers
		public Vector3 Counts;				// number in each axis

		private void Awake()
		{
			Vector3 start = new Vector3(-Counts.x/2*Displacement.x, 0, -Counts.z/2*Displacement.z);
			for (int n = 0; n < Counts.y; ++n)
			{
				for (int m = 0; m < Counts.z; ++m)
				{
					// draw along x
					PlaceLine(start
						, new Vector3(Displacement.x, 0, 0)
						, (int)Counts.x);

					start.z += Displacement.z;
				}

				start.z = 0;
				start.y += Displacement.y;
			}
		}

		void PlaceLine(Vector3 start, Vector3 delta, int num)
		{
			for (int n = 0; n < num; ++n)
			{
				var go = Instantiate(MarkerPrefab);
				go.transform.SetParent(transform);
				go.transform.position = start;
				start += delta;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void FixedUpdate()
		{
		}
	}
}


using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

using ChaosCult.SceneLabels;
using App.Math;
using App.Utils;

using UniRx;

namespace App
{
	public class Motor : MonoBehaviour 
	{
		public enum ESpin
		{
			CW, CCW,
		}

		public float RevsPerMinute;		// revolutions per minute
		public float LiftFactor;			// how much lift motor generates per rev/min
		public ESpin Spin;
		public Vector3 Force;

		public float LiftForce
		{
			get
			{
				return ForceDir*RevsPerMinute*LiftFactor*ForceDir;
			}
		}

		public Vector3 ForceDir
		{
			get
			{
				
			}
		}

		float ForceDir()
		{
			return Spin == ESpin.CW ? 1 : -1;
		}

		private void Awake()
		{

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

		private void OnDrawGizmos()
		{
			LabelsAccess.DrawLabel(transform.position, gameObject.name, null);
		}
	}
}


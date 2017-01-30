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

		public float RevsPerMinute;			// revolutions per minute
		public float LiftFactor;			// how much lift motor generates per rev/min
		public ESpin SpinDirection;

		public float ForceGizmoScale = 5;
		public float RotGizmoScale = 0.1f;

		public Vector3 WorldForce 
		{ 
			get 
			{ 
				// return transform.TransformVector(transform.up * LocalForceMag - transform.localPosition);
Debug.Log(transform.up);
				return transform.TransformVector(transform.localRotation*transform.up*LocalForceMag);// - transform.localPosition);
			} 
		}

		public float LocalForceMag 
		{ 
			get { return RevsPerMinute*LiftFactor; } 
		}

		public float SpinDir { get { return SpinDirection == ESpin.CW ? 1 : -1; } }

		private static int TraceLevel = 1;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
			SpinBlade(Time.deltaTime);

			if (TraceLevel > 0) DrawForceVector();
		}

		private void SpinBlade(float dt)
		{
			_rot += RevsPerMinute*SpinDir*dt*RotGizmoScale;
			transform.localRotation = Quaternion.AngleAxis(_rot, Vector3.forward);
		}

		private void DrawForceVector()
		{
			Debug.DrawLine(transform.position, transform.position + WorldForce*ForceGizmoScale, Color.magenta, 0, false);
		}

		private void FixedUpdate()
		{
		}

		private void OnDrawGizmos()
		{
			LabelsAccess.DrawLabel(transform.position, gameObject.name, null);
		}

		float _rot;
	}
}


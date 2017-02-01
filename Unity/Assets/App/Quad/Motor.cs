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

namespace App.Quad
{
	[RequireComponent(typeof(ElectronicSpeedController))]
	public class Motor : MonoBehaviour 
	{
		public PidScalarController PidController;
		public float DesiredRpm;
		public float Rpm;

		public enum ESpin
		{
			CW,		// clockwise
			CCW,	// counter clockwise
		}

		public float RevsPerMinute;				// revolutions per minute
		public float ForceMultiPlier;			// How much extra force is added per revolution
		public ESpin SpinDirection;				// the direction of the motor. changes the torque direction

		public float TorqueGizmoScale = 5;		// just used for render force gizmo lines
		public float ForceGizmoScale = 5;		// just used for render force gizmo lines
		public float RotScale = 6;				// 360/60 = 6. this means shows real rotational speed

		public static int TraceLevel = 3;

		// world-space torque turning force
		public Vector3 WorldTorque
		{
			get
			{
				// https://en.wikipedia.org/wiki/Torque
				var com = _body.CenterOfMass.transform.position;
				var motorPos = transform.position;
				var fromCenter = com - motorPos;
				var cross = Vector3.Cross(fromCenter, WorldForce);
				var torque = cross*SpinDir;
				return torque;
			}
		}

		// the world-space thrust supplied by the rotor attached to the motor
		public Vector3 WorldForce { get { return transform.up*RevsPerMinute*ForceMultiPlier; } }

		// direction of the motor - clockwise or counter-clockwise
		public float SpinDir { get { return SpinDirection == ESpin.CW ? -1 : 1; } }

		private void Awake()
		{
			_body = GetComponentInParent<Body>();
			Esc = GetComponent<ElectronicSpeedController>();
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
			UpdateRotation(dt);
			
			transform.localRotation = Quaternion.AngleAxis((float)_rot, Vector3.up);
		}

		void UpdateRotate(float dt)
		{
			_rot += RevsPerMinute*SpinDir*dt*RotScale;
			while (_rot > 360) _rot -= 360;
			while (_rot < 360) _rot += 360;
		}

		private void DrawForceVector()
		{
			Debug.DrawLine(transform.position, transform.position + WorldForce*ForceGizmoScale, Color.yellow, 0, false);
			Debug.DrawLine(transform.position, transform.position + WorldTorque*TorqueGizmoScale, Color.magenta, 0, false);
		
			DebugGraph.Log("torque", WorldTorque.y);
		}

		private void FixedUpdate()
		{
		}

		private void OnDrawGizmos()
		{
			LabelsAccess.DrawLabel(transform.position, gameObject.name, null);
		}

		private float _rot;
		private Body _body;
	}
}


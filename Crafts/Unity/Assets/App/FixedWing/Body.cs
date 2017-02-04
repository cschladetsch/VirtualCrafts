﻿using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

using UniRx;

namespace App.FixedWing
{
	[RequireComponent(typeof(Rigidbody))]
    public class Body : MonoBehaviour 
	{
		public Transform CenterOfMass;
		public FlightController FlightController;
		public float ForceGizmoScale = 2;

		public int TraceLevel;

		private void Awake()
		{
			TraceLevel = 1;

			FlightController = transform.parent.GetComponentInChildren<FlightController>();
			_motors = GetComponentsInChildren<Motor>().ToList();
			_controlSurfaces = GetComponentsInChildren<ControlSurface>().ToList();
			_rigidBody = GetComponent<Rigidbody>();

			//Assert.IsTrue(_controlSurfaces.Count > 0);
			Assert.IsNotNull(FlightController);
			Assert.IsNotNull(_rigidBody);

			Debug.Log("There are " + _controlSurfaces.Count + " force providers");


			foreach (var f in _controlSurfaces)
			{
				f.Construct(this);
			}
		}

		private void FixedUpdate()
		{
			var forces = UpdateForces();

			if (TraceLevel > 0) DrawForces(forces);
		}

		IList<ForceProvider> UpdateForces()
		{
			var forces = MotorForces().Union(ControlSurfaces()).ToList();
			foreach (var f in forces)
			{
				_rigidBody.AddForceAtPosition(f.Force, f.Position, ForceMode.Impulse);
				_rigidBody.AddTorque(f.Torque, ForceMode.Impulse);
			}

			return forces;
		}

		IEnumerable<ForceProvider> MotorForces()
		{
			return _motors.Select(m => m.ForceProvider);
		}

		IEnumerable<ForceProvider> ControlSurfaces()
		{
			return _controlSurfaces.Select(cp => cp.ForceProvider);
		}

		void DrawForces(IList<ForceProvider> fp)
		{
			foreach (var f in fp)
			{
				Debug.DrawLine(f.Position, f.Position + f.Force, Color.red, 0, false);
			}
		}

		private Rigidbody _rigidBody;
		private List<ControlSurface> _controlSurfaces = new List<ControlSurface>();
		private List<Motor> _motors = new List<Motor>();
	}
}

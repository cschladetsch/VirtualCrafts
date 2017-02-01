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
	[RequireComponent(typeof(Rigidbody))]
	public class Body : MonoBehaviour 
	{
		public FlightController FlightController;
		public GameObject CenterOfMass;
		public Motor[] Motors { get { return FlightController.Motors; } }

		public float ForceGizmoScale = 5;
		static int TraceLevel = 2;

		void Awake()
		{
			TraceLevel = 0;

			FlightController = GetComponentInChildren<FlightController>();
			Assert.IsNotNull(FlightController);

			_rigidBody = GetComponent<Rigidbody>();
		}

		private void Update()
		{
			if (TraceLevel < 1) return;

			var euler = transform.rotation.eulerAngles;
			DebugGraph.Log("Pitch", euler.x);
			DebugGraph.Log("Yaw", euler.y);
			DebugGraph.Log("Roll", euler.z);
		}

		/// <summary>
		/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
		/// </summary>
		void FixedUpdate()
		{
			ApplyForces(GatherForces());
		}

		List<AppliedForce> GatherForces()
		{
			var forces = new List<AppliedForce>();

			forces.AddRange(ApplyForcesFromBlades());

			forces.AddRange(GetWindForces());

			forces.AddRange(GetRainForces());

			return forces;
		}

		void ApplyForces(IList<AppliedForce> forces)
		{
			foreach (var force in forces)
			{
				_rigidBody.AddForceAtPosition(force.Force, force.Where, ForceMode.Impulse);

				_rigidBody.AddTorque(force.Torque, ForceMode.Impulse);
			}
			
			if (TraceLevel > 1) DrawTotalForces(forces);
		}

		IEnumerable<AppliedForce> GetWindForces()
		{
			yield break;
		}

		IEnumerable<AppliedForce> GetRainForces()
		{
			yield break;
		}

		List<AppliedForce> ApplyForcesFromBlades()
		{
			var forces = new List<AppliedForce>();
			foreach (var m in Motors)
			{
				forces.Add(new AppliedForce(m.WorldForce, m.transform.position, m.WorldTorque));
			}

			return forces;
		}

		void DrawTotalForces(IEnumerable<AppliedForce> forces)
		{
			var forceTotal = Vector3.zero;
			var torqueTotal = Vector3.zero;
			var posTotal = Vector3.zero;
			var count = 0;
			foreach (var f in forces)
			{
				forceTotal += f.Force;
				torqueTotal += f.Force;
				posTotal += f.Where;
				++count;
			}

			float numForces = count;
			var center = posTotal/numForces;
			var force = forceTotal/numForces;
			var torque = torqueTotal/numForces;

			Debug.DrawLine(center, center + force*ForceGizmoScale, Color.red, 0, false);
			Debug.DrawLine(center, center + torque*ForceGizmoScale, Color.blue, 0, false);
		}

		struct AppliedForce
		{
			public Vector3 Force;
			public Vector3 Torque;
			public Vector3 Where;

			public AppliedForce(Vector3 f, Vector3 w, Vector3 t)
			{
				Force = f;
				Where = w;
				Torque = t;
			}
		}

		private Rigidbody _rigidBody;
	}
}


using System.Linq;
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
			float dt = Time.fixedDeltaTime;

			UpdateMotors(dt);

			UpdateSurfaces(dt);
		}

		void UpdateMotors(float dt)
		{
			foreach (var m in _motors)
			{
				m.Update(dt);
				_rigidBody.AddForceAtPosition(m.Thrust, m.Position, ForceMode.Impulse);
			}
		}

		void UpdateSurfaces(float dt)
		{
			foreach (var surface in _controlSurfaces)
			{
				surface.UpdateForce(dt);

				var f = surface.ForceProvider;

				_rigidBody.AddForceAtPosition(f.Force, f.Position, ForceMode.Impulse);
				_rigidBody.AddTorque(f.Torque*f.TorqueScale, ForceMode.Impulse);

				// Debug.LogFormat("force: {0} at {1}", f.Force, f.Position);
			}
		}

		// IEnumerable<ForceProvider> MotorForces()
		// {
		// 	return _motors.Select(m => m.ForceProvider);
		// }

		// IEnumerable<ForceProvider> ControlSurfaces()
		// {
		// 	return _controlSurfaces.Select(cp => cp.ForceProvider);
		// }

		// void DrawForces(IList<ForceProvider> fp)
		// {
		// 	foreach (var f in fp)
		// 	{
		// 		Debug.DrawLine(f.Position, f.Position + f.Force, Color.red, 0, false);
		// 	}
		// }

		private Rigidbody _rigidBody;
		private List<Motor> _motors = new List<Motor>();
		private List<ControlSurface> _controlSurfaces = new List<ControlSurface>();
	}
}


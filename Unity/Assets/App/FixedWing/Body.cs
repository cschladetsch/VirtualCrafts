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

			_controlSurfaces = GetComponentsInChildren<ControlSurface>().ToList();
			_rigidBody = GetComponent<Rigidbody>();
			FlightController = transform.parent.GetComponentInChildren<FlightController>();

			Assert.IsTrue(_controlSurfaces.Count > 0);
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
			UpdateForces();

			if (TraceLevel > 0) DrawForces();
		}

		void UpdateForces()
		{
			Vector3 forceSum = Vector3.zero;
			Vector3 whereSum = Vector3.zero;

			foreach (var f in _controlSurfaces)
			{
				var fp = f.ForceProvider;
				_rigidBody.AddForceAtPosition(fp.Force, fp.Where);

				forceSum += fp.Force;
				whereSum += fp.Where;
			}

			var count = (float)_controlSurfaces.Count;
			forceSum /= count;
			whereSum /= count;

			// var com = CenterOfMass.position;
			Debug.DrawLine(whereSum, whereSum + forceSum*ForceGizmoScale, Color.magenta, 0);
		}

		void DrawForces()
		{
		}

		private Rigidbody _rigidBody;
		private List<ControlSurface> _controlSurfaces = new List<ControlSurface>();
	}
}


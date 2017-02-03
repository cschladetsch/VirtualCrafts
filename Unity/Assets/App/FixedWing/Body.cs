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
		public float ForceGizmoScale = 2;


		public int TraceLevel;

		private void Awake()
		{
			TraceLevel = 1;

			_forces = GetComponentsInChildren<ForceProviderBehaviour>().ToList();
			_rigidBody = GetComponent<Rigidbody>();

			foreach (var f in _forces)
			{
				f.Construct(this);
			}

			Assert.IsTrue(_forces.Count > 0);
			Assert.IsNotNull(_rigidBody);

			Debug.Log("There are " + _forces.Count + " force providers");
		}

		private void Start()
		{
		}

		private void Update()
		{
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

			foreach (var f in _forces)
			{
				_rigidBody.AddForceAtPosition(f.Force.Force, f.Force.Where);

				forceSum += f.Force.Force;
				whereSum += f.Force.Where;
			}

			var count = (float)_forces.Count;
			forceSum /= count;
			whereSum /= count;

			// var com = CenterOfMass.position;
			Debug.DrawLine(whereSum, whereSum + forceSum*ForceGizmoScale, Color.magenta, 0);
		}

		void DrawForces()
		{

		}

		private Rigidbody _rigidBody;
		private List<ForceProviderBehaviour> _forces = new List<ForceProviderBehaviour>();
	}
}


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
		private void Awake()
		{
			_forces = GetComponentsInChildren<ForceProviderBehaviour>().ToList();
			_rigidBody = GetComponent<Rigidbody>();

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

			DrawForces();
		}

		void DrawForces()
		{

		}


		private Rigidbody _rigidBody;
		private List<ForceProviderBehaviour> _forces = new List<ForceProviderBehaviour>();
	}
}


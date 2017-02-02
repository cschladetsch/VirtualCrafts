
using UnityEngine;

namespace App
{
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
}



using UnityEngine;

namespace App.FixedWing
{
	// something that applies force to the body
    public abstract class ForceProviderBehaviour : MonoBehaviour 
	{
		// the world-space force to apply
		public virtual AppliedForce Force { get; }

		// how it scales with Rpm of motor
		public AnimationCurve RpmRelative;

		public float GizmoScale = 1;

		public int TraceLevel;

		private void Awake()
		{
			TraceLevel = 1;
		}

		public void Construct(Body body)
		{
			_body = body;
			_flightController = body.transform.parent.GetComponentInChildren<FlightController>();
		}

		protected void DrawForce()
		{
			if (TraceLevel > 0) Debug.DrawLine(transform.position, transform.position + Force.Where*GizmoScale, Color.yellow, 0);
		}

		protected Body _body;
		protected FlightController _flightController;
	}
}


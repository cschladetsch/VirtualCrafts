
using UnityEngine;

using App.Math;

namespace App.FixedWing
{
	// A Motor has a ForceProvider that has a thrust proportional
	// to the rpm of the motor and the scale factor of the props
    public class Motor : MonoBehaviour
	{
		public float DesiredRpm;
		public float MaxThrottleRpm;
		public float CurrentRpm;

		public Vector3 Thrust;
		public float RpmThrustScale;

		public float NormalisedThrustMagnitude 
		{
			 get { return CurrentRpm/MaxThrottleRpm; } 
		} 

		public Vector3 Position { get { return transform.position; } }

		public PidScalarController PidController = new PidScalarController();
		public Vector3 PID = new Vector3(0.8f, 0.5f, 0.01f);

		// just for visuals. using real speeds is hard to see, so
		// slow it down artificially
		public float PropRotationRenderScale = 0.5f;

		private void Update()
		{
			// rotate the visuals only. 
			// 6.0 means 1 full revolution in 60 seconds: 6*60 = 360
			_rot += Time.deltaTime*6.0f*CurrentRpm*PropRotationRenderScale;
			transform.localRotation = Quaternion.AngleAxis(_rot, -Vector3.forward);
		}

		public void UpdateRpm(float dt)
		{
			PidController.SetPid(PID);

			// progress towards desired Rpm
			var change = PidController.Calculate(
				DesiredRpm*MaxThrottleRpm, CurrentRpm, dt);
			CurrentRpm += change;
			CurrentRpm = Mathf.Clamp(CurrentRpm, 0, MaxThrottleRpm);

			DebugGraph.Log("Rpm", CurrentRpm);
		}

		public void Step(float dt)
		{
			UpdateRpm(dt);

			Thrust = transform.forward
				*CurrentRpm
				*RpmThrustScale
				*dt;
		}

		private float _rot;
	}
}



using UnityEngine;

using App.Math;

namespace App.FixedWing
{
    public class Motor : MonoBehaviour
	{
		public float DesiredRpm;
		public float MaxThrottleRpm;
		public float Rpm;

		public ForceProvider ForceProvider;

		public PidScalarController PidController = new PidScalarController();
		public Vector3 PID = new Vector3(0.8f, 0.5f, 0.01f);
		public float RotGizmoScale = 0.5f;

		public float RpmScale = 1;

		private void Update()
		{
			_rot += Time.deltaTime*6.0f*Rpm*RotGizmoScale;
			transform.localRotation = Quaternion.AngleAxis(_rot, -Vector3.forward);
		}

		private void FixedUpdate()
		{
			PidController.P = PID.x;
			PidController.I = PID.y;
			PidController.D = PID.z;

			var dt = Time.fixedDeltaTime;
			var change = PidController.Calculate(DesiredRpm*MaxThrottleRpm, Rpm, dt);
			Rpm += change;//*dt;

			Rpm = Mathf.Clamp(Rpm, 0, MaxThrottleRpm);

			DebugGraph.Log("Rpm", Rpm);

			ForceProvider.Force = transform.forward*Rpm*RpmScale;
		}

		private float _rot;
	}
}


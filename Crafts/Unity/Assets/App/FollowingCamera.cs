
using UnityEngine;

using App.Math;

namespace App
{
	/// <summary>
	/// A system that follows a target from behind and at a distance up.
	/// TODO: need to sync the low-pass and PID systems outside of either
	/// Update() or FixedUpdate()
	/// </summary>
    public class FollowingCamera : MonoBehaviour 
	{
		public Transform Target;	// what to keep on screen
		public float LookAhead;		// how far to look ahead of object
		public float LagBehind;		// how far to stay behind it
		public float Height;		// how high above it
		public Transform Controlled;

		public Vector3 MovePidFactors;
		public PidVector3Controller MoveController = new PidVector3Controller();

		public ButterworthFilteredVector3 FilteredPosition;
		public int FilterSampleRate = 60;
		public int FilterCutoffFrequency = 100;
		private float _filterTimer;

		// not used yet: maybe later will smooth the camera orientation as well
		// public Vector4 OrientationPidFactors;
		// public PidQuaternionController OrientationController;

		private void Awake()
		{
			ResetFilterTimer();
			FilteredPosition = new ButterworthFilteredVector3(FilterCutoffFrequency, 
				FilterSampleRate, EPassType.Lowpass, 0.8f);
		}

		private void ResetFilterTimer()
		{
			_filterTimer = 1.0f/FilterSampleRate;
		}

		private void FixedUpdate()
		{
			float dt = Time.fixedDeltaTime;
			_filterTimer -= dt;
			if (_filterTimer < 0)
			{
				// TODO: need to tune the low-pass filter
				//ApplyFilter(dt);
			}

			MoveCamera(dt);
		}

		void ApplyFilter(float dt)
		{
			ResetFilterTimer();
			FilteredPosition.Target = Target.transform.position;
			Controlled.position = FilteredPosition.Filter(dt);
		}

		void MoveCamera(float dt)
		{
			MoveController.SetPid(MovePidFactors);

			var tp = Target.position; 
			var tf = Target.forward;
			var lookAt = tp + tf*LookAhead;
			var belowCam = tp - tf*LagBehind;
			var desired = belowCam + Vector3.up*Height;

			var delta = MoveController.Calculate(desired, Controlled.position, dt);
			Controlled.position += delta;

			Controlled.LookAt(lookAt, Vector3.up);	// TODO: PidQuaternionController
		}
	}
}


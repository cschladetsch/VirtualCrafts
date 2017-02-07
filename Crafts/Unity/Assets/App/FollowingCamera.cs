
using UnityEngine;

using App.Math;

namespace App
{
	/// <summary>
	/// A system that follows a target from behind and at a distance up 
	/// </summary>
    public class FollowingCamera : MonoBehaviour 
	{
		public GameObject Target;	// what to keep on screen
		public float LookAhead;		// how far to look ahead of object
		public float LagBehind;		// how far to stay behind it
		public float Height;		// how high above it
		public Transform Controlled;

		public Vector3 MovePidFactors;
		public PidVector3Controller MoveController = new PidVector3Controller();

		// not used yet: maybe later will smooth the camera orientation as well
		// public Vector4 OrientationPidFactors;
		// public PidQuaternionController OrientationController;

		// not used yet: maybe later will pass 
		// public ButterworthFilter FilteredPosition;

		private void Update()
		{
			MoveController.SetPid(MovePidFactors);

			var dt = Time.deltaTime;
			var tp = Target.transform.position; 
			var tf = Target.transform.forward;
			var lookAt = tp + tf*LookAhead;
			var belowCam = tp - tf*LagBehind;
			var desired = belowCam + Vector3.up*Height;

			var delta = MoveController.Calculate(desired, Controlled.position, dt);
			Controlled.position += delta;

			Controlled.LookAt(lookAt, Vector3.up);	// TODO: PidQuaternionController
		}
	}
}


using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

using App.Math;
using App.Utils;

using UniRx;

namespace App
{
	public class ForceApplier : MonoBehaviour 
	{
		public Rigidbody Rod;
		public Vector3 PositionPid;
		public Vector3 YawPid;

		public float ForceUpToRod;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void FixedUpdate()
		{
			var dt = Time.fixedDeltaTime;

			WritePids();

			ProcessHit(TrackRod(), dt);
		}

		RaycastHit? TrackRod()
		{
			var mask = LayersMask.Make("Rod");
			var hits = Physics.RaycastAll(transform.position, Vector3.up, 10000, mask);
			if (hits.Length != 1) return null;
			return hits[0];
		}

		void WritePids()
		{
			_position.SetPid(PositionPid);
		}

		void ProcessHit(RaycastHit? hit, float dt)
		{
			if (!hit.HasValue) return;

			var roll = Rod.transform.rotation.eulerAngles.z;
			if (Mathf.Abs(roll) < 0.1f)
				return;

			// mag of torque is abs(pos)*abs(force)*sin(yaw))
			// m = abs(p)*abs(f)*sin(yaw)
			// => p = m/abs(f)*sin(yaw)

			var desiredPos = ForceUpToRod/Mathf.Sin(roll);
			var delta = _position.Calculate(desiredPos, transform.position.x, dt)*dt;
			var newX = transform.position.x + delta;
			Debug.LogFormat("desired: {0}, delta: {1}, newX: {2}", desiredPos, delta, newX);
			var newPos = new Vector3(newX, 0, 0);
			MoveRodVertically(hit.Value.point);
			
			// delta is the difference in yaw we want to induce
			Rod.AddForceAtPosition(new Vector3(newX, ForceUpToRod, 0), newPos, ForceMode.Impulse);
		}

		void MoveRodVertically(Vector3 p)
		{
			var top = transform.localScale.y;
			var hitPt = p;
			var pt = transform.position;
			pt.y = hitPt.y - top;
			transform.position = pt;
		}

		private PidScalarController _position = new PidScalarController();
	}

}


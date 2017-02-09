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
	public class Sphere : MonoBehaviour 
	{
		public Vector3 PID;
		public float MinDirChangeTime = 1;
		public float MaxDirChangeTime = 3;
		public float Speed = 1;

		private void Awake()
		{
			Random.InitState((int)System.DateTime.Now.Ticks);

			_nextDirChangeTime = GetNextDirectionChangeTime();
			_dir = Random.Range(0, 10) < 5 ? 1 : -1;
		}

		float GetNextDirectionChangeTime()
		{
			return Random.Range(MinDirChangeTime, MaxDirChangeTime);
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		float _pause = 2;

		private void FixedUpdate()
		{
			_controller.SetPid(PID);

			float dt = Time.fixedDeltaTime;
			_pause -= dt;
			if (_pause > 0)
				return;

			FixToXY();

			UpdatePosition(dt);
		}

		void UpdatePosition(float dt)
		{
			CheckForDirectionChange(dt);

			_desiredX += _dir*Speed*dt;
			var pt = transform.position;
			var delta = _controller.Calculate(_desiredX, pt.x, dt);
			pt.x += delta;
			transform.position = pt;
		}

		void CheckForDirectionChange(float dt)
		{
			_nextDirChangeTime -= dt;
			if (_nextDirChangeTime < 0)
			{
				_dir *= -1;
				_nextDirChangeTime = GetNextDirectionChangeTime();	
			}
		}

		void FixToXY()
		{
			// fix sphere to always be on XY axis
			var pt = transform.position;
			pt.z = 0;
			transform.position = pt;
		}

		private PidScalarController _controller = new PidScalarController();
		private float _nextDirChangeTime;
		private float _dir;
		private float _desiredX;

	}
}


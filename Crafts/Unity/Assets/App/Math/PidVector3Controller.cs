using UnityEngine;

namespace App.Math
{
    public class PidVector3Controller
	{
		// target location
		public Vector3 Target;

		// pid co-efficients
		public float P = 0.5f;
		public float I = 0.05f;
		public float D = 0.1f;

		public PidVector3Controller()
		{
			CreateControllers();
		}

		public PidVector3Controller(float p, float i, float d)
		{
			P = p;
			I = i;
			D = d;

			CreateControllers();
		}

		/// <summary>
		/// Calculate the absolute delta to add to the current control signal
		/// given current control signal and a delta-time since last call
		/// <param name="current">current value</param>
		/// <param name="dt">delta time</param>
		/// <returns>an absolute change to add to the current value passed in. does not require dt scaling</returns>
		public Vector3 Calculate(Vector3 current, float dt)
		{
			return Calculate(Target, current, dt);
		}

		/// <summary>
		/// PID controller for a Vector3.
		/// The result is a delta to be added to the process value.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="current">the current position</param>
		/// <param name="dt"></param>
		/// <returns></returns>
		public Vector3 Calculate(Vector3 target, Vector3 current, float dt)
		{
			float x = _controllers[0].Calculate(target.x, current.x, dt);
			float y = _controllers[1].Calculate(target.y, current.y, dt);
			float z = _controllers[2].Calculate(target.z, current.z, dt);

			return new Vector3(x,y,z);
		}

		/// <summary>
		/// Set all the PID factors for all three controllers at once.
		/// Use SetPid[X,Y,Z]() to set the pid factors for each axes separately.
		/// </summary>
		/// <param name="pid">the pid factors to use for each of x,y,z axes</param>
		public void SetPid(Vector3 pid)
		{
			for (var n = 0; n < 3; ++n)
			{
				var c = _controllers[n];
				c.P = pid.x;
				c.I = pid.y;
				c.D = pid.z;
			}
		}

		public void SetPidX(Vector3 pid)
		{
			_controllers[0].SetPid(pid);
		}

		public void SetPidY(Vector3 pid)
		{
			_controllers[1].SetPid(pid);
		}

		public void SetPidZ(Vector3 pid)
		{
			_controllers[2].SetPid(pid);
		}

		void CreateControllers()
		{
			_controllers = new[] {
				new PidScalarController(P, I, D),
				new PidScalarController(P, I, D),
				new PidScalarController(P, I, D)
			};
		}

		private PidScalarController[] _controllers;	
	}
}

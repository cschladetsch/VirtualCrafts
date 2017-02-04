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

namespace App.FixedWing
{
	public class RudderControlSurface : ControlSurface
	{
		override protected void ChangeAngle(float dt)
		{
			base.UpdatePid();

			var delta = Controller.Calculate(DesiredAngle, Angle, dt);
			Angle += delta;
			Angle = Mathf.Clamp(Angle, -MaxThrow, MaxThrow);
			ForceProvider.transform.localRotation = Quaternion.AngleAxis(Angle, Axis);
		}
	}
}


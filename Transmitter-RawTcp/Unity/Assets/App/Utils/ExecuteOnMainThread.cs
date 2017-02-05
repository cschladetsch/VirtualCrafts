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

namespace App.Utils
{
	public class ExecuteOnMainThread : MonoBehaviour 
	{
		private void Update()
		{
			foreach (var act in _actions)
			{
				act();
			}

			_actions.Clear();
		}

		public void NextUpdate(Action act)
		{
			_actions.Add(act);
		}

		private List<Action> _actions = new List<Action>();
	}
}


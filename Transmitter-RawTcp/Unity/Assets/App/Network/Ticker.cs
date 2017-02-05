using UnityEngine;

namespace App
{
    public class Ticker : MonoBehaviour 
	{
		public float Interval = 0.100f;

		private float _timer;

		void Awake()
		{
			_timer = Interval;
		}

		private void Update()
		{
			_timer -= Time.deltaTime;
			if (_timer < 0)
			{
				Debug.Log("Tick: " + Time.time);
				_timer = Interval;
			}
		}
	}
}


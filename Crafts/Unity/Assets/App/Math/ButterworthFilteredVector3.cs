
using UnityEngine;

namespace App.Math
{
    public class ButterworthFilteredVector3
	{
		public Vector3 Target;
		public Vector3 Filtered;

		public float CutoffFrequency = 200;
		public EPassType PassType = EPassType.Lowpass;
		public int SampleRate = 60;
		public float Resonance = 0.8f;
		public bool Change = false;

		public ButterworthFilteredVector3()
		{
			ResetFilters();
			ResetTimer();
		}

		public ButterworthFilteredVector3(float frequency, int sampleRate, EPassType passType, float resonance)
		{
			CutoffFrequency = frequency;
			SampleRate = sampleRate;
			PassType = passType;
			Resonance = resonance;
			ResetFilters();
			ResetTimer();
		}

		public void Reset(bool history = false)
		{
			ResetFilters(history);
			ResetTimer();
		}

		private void ResetTimer()
		{
			_timer = 1.0f/SampleRate;
		}

		private void ResetFilters()
		{
			foreach (var v in _filters)
			{
				v.Construct(CutoffFrequency, SampleRate, PassType, Resonance);
			}
		}

		public Vector3 Filter(float dt)
		{
			if (Change)
			{
				ResetFilters();
				Change = false;
			}

			_timer -= Time.deltaTime;
			if (_timer < 0)
			{
				Filtered.x = _filters[0].Update(Target.x);
				Filtered.y = _filters[1].Update(Target.y);
				Filtered.z = _filters[2].Update(Target.z);
			}

			return Filtered;
		}

		private void ResetFilters(bool history)
		{
			foreach (var f in _filters)
			{
				f.Construct(CutoffFrequency, SampleRate, PassType, Resonance);
				if (history)
				{
					f.Trim();
				}
			}
			Change = false;
		}

		private ButterworthFilter[] _filters = new ButterworthFilter[3]
		{
			new ButterworthFilter(),
			new ButterworthFilter(),
			new ButterworthFilter() 
		};

		private float _timer;
	}
}


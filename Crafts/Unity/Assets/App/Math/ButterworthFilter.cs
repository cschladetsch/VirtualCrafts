using UnityEngine;

namespace App.Math
{
	public enum EPassType
	{
		Highpass,
		Lowpass,
	}

	public class ButterworthFilter
	{
		public float Value
		{
			get { return _outputHistory[0]; }
		}

		public ButterworthFilter()
		{
		}

		// resonance amount, from sqrt(2) to ~ 0.1
		/// <summary>
		/// Construct a butterworth filter 
		/// </summary>
		/// <param name="frequency">the cut-off band frequency</param>
		/// <param name="sampleRate">the sample rate between updates</param>
		/// <param name="passType">high or low-pass</param>
		/// <param name="resonance"></param>
		public ButterworthFilter(float frequency, int sampleRate, EPassType passType, float resonance)
		{
			Construct(frequency, sampleRate, passType, resonance);
		}

		public void Construct(float frequency, int sampleRate, EPassType passType, float resonance)
		{
			switch (passType)
			{
				case EPassType.Lowpass:
					c = 1.0f / (float)Mathf.Tan(Mathf.PI * frequency / sampleRate);
					a1 = 1.0f / (1.0f + resonance * c + c * c);
					a2 = 2f * a1;
					a3 = a1;
					b1 = 2.0f * (1.0f - c * c) * a1;
					b2 = (1.0f - resonance * c + c * c) * a1;
					break;
				case EPassType.Highpass:
					c = (float)Mathf.Tan(Mathf.PI * frequency / sampleRate);
					a1 = 1.0f / (1.0f + resonance * c + c * c);
					a2 = -2f * a1;
					a3 = a1;
					b1 = 2.0f * (c * c - 1.0f) * a1;
					b2 = (1.0f - resonance * c + c * c) * a1;
					break;
			}
		}

		public float Update(float newInput)
		{
			float newOutput = a1 * newInput 
				+ a2*_inputHistory[0] 
				+ a3*_inputHistory[1] 
				- b1*_outputHistory[0] 
				- b2*_outputHistory[1];

			_inputHistory[1] = _inputHistory[0];
			_inputHistory[0] = newInput;

			_outputHistory[2] = _outputHistory[1];
			_outputHistory[1] = _outputHistory[0];
			_outputHistory[0] = newOutput;

			return Value;
		}

		public void Trim()
		{
			_inputHistory[0] = _inputHistory[1] = _outputHistory[0];
			_outputHistory[2] = _outputHistory[1] = _outputHistory[2] = _outputHistory[0];
		}

		/// <summary>
		/// rez amount, from sqrt(2) to ~ 0.1
		/// </summary>
		private float c, a1, a2, a3, b1, b2;

		/// <summary>
		/// Array of input values, latest are in front
		/// </summary>
		private float[] _inputHistory = new float[2];

		/// <summary>
		/// Array of output values, latest are in front
		/// </summary>
		private float[] _outputHistory = new float[3];
	}
}

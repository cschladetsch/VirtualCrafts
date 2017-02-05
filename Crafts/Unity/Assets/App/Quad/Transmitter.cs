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
using Flow;

using ChaosCult.SceneLabels;

namespace App.Quad
{
	public class Transmitter : MonoBehaviour 
	{
		public enum  EChannelType
		{
			// Thrust - RPM for motors
			THR = 0,

			// Rudder: yaw
			RUD,

			// Airleron: roll
			AIR,

			// Elevator: pitch
			ELE,

			// Button: return to home
			RTH,

			// Button: stop all motors
			HLT,

			// Phree-position switch
			RATES1,		// low limits on angles
			RATES2,		// higher limites on angles
			AERO,		// no limits - 3d "aerobatic" mode"
		}

		public struct Channel
		{
			public IChannel<float> Values;	// 0..1
			public float Trim;
			public AnimationCurve Expo;

			public Channel(IChannel<float> channel, AnimationCurve expo)
			{
				Values = channel;
				Trim = 0;
				Expo = expo;
			}
		}

		public AnimationCurve[] Expos = new AnimationCurve[4];

		public Dictionary<EChannelType, Channel> Channels;

		private void Construct(IFactory factory)
		{
			Channels = new Dictionary<EChannelType, Channel>();
			Channels[EChannelType.THR] = new Channel(NewFloatChannel(factory), Expos[0]);
			Channels[EChannelType.RUD] = new Channel(NewFloatChannel(factory), Expos[1]);
			Channels[EChannelType.AIR] = new Channel(NewFloatChannel(factory), Expos[2]);
			Channels[EChannelType.ELE] = new Channel(NewFloatChannel(factory), Expos[3]);
		}

		IChannel<float> NewFloatChannel(IFactory fact)
		{
			return fact.NewChannel<float>();
		}


		private void Start()
		{
		}

		private void Update()
		{
		}

		private void FixedUpdate()
		{
		}

		void OnDrawGizmos()
		{
			LabelsAccess.DrawLabel(transform.position, "Transmitter", null);
		}

	}
}


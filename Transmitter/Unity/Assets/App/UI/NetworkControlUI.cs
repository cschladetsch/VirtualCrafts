using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

using App.Math;
using App.Utils;


using App.Network;

using UniRx;

namespace App.UI
{
	public class NetworkControlUI : MonoBehaviour 
	{
		public Transmitter Transmitter;
		public Receiver Receiver;

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
		}

		public void StartServer()
		{
			Transmitter.PowerOn();
		}

		public void JoinSession()
		{
			Receiver.PowerOn();
		}

		public void HostSend()
		{
			Transmitter.SendText("Hello");
		}

		public void ClientSend()
		{
			Receiver.SendText("World");
		}
	}
}


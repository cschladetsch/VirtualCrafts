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

		public void ToServer()
		{
		}

		public void ToClient()
		{
		}

		public void StartServer()
		{
			Debug.Log("StartServer");
		}

		public void JoinSession()
		{
			Debug.Log("JoinSession");
		}

		public void HostSend()
		{
			Debug.Log("HostSend");
		}

		public void ClientSend()
		{
			Debug.Log("ClientSend");
		}
	}
}


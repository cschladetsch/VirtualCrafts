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

namespace App.Network
{
	public class NetworControl : MonoBehaviour 
	{
		public AsyncClient Client;
		public AsyncListener Listener;

		public void ToClient()
		{
			Listener.SendToAll("To Client");
		}

		public void ToServer()
		{
			Assert.IsNotNull(Client);
			Assert.IsNotNull(Client.Connection);
			Client.Connection.Send("To Server");
		}

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
	}
}


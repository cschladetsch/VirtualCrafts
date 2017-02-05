using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

using App.Math;
using App.Utils;

namespace App
{
	public class Transmitter : MonoBehaviour 
	{
		public int Port = 11111;

		private void Awake()
		{
			NetworkTransport.Init();

			var config = new ConnectionConfig();
			_reiliableChannelId  = config.AddChannel(QosType.Reliable);
			_unreliableChannelId = config.AddChannel(QosType.Unreliable);

			var topology = new HostTopology(config, 1);

			_hostId = NetworkTransport.AddHost(topology, Port);

			Debug.Log(_hostId);

			var ip = Net.GetMyAddress();
			Debug.LogFormat("My ip={0}, port={1}", ip.ToString(), Port);

			_connectionId = NetworkTransport.Connect(_hostId, ip.ToString(), Port, 0, out _error);
			NetworkTransport.Disconnect(_hostId, _connectionId, out _error);

			// NetworkTransport.Receive(
			// 	out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
			// NetworkTransport.ReceiveFromHost(
			// 	recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
		}

		void SendText()
		{
			var buffer = Encoding.ASCII.GetBytes("Hello World");
			TestResult(NetworkTransport.Send(_hostId, _connectionId, 
				_reiliableChannelId, buffer, buffer.Length, out _error), "Send"
				);
		}

		void TestResult(bool result, string what = "")
		{
			if (result) return;

			Debug.LogErrorFormat("Failure: {0} with error {1}", what, _error);
		}

		void Disconnect()
		{
			NetworkTransport.Disconnect(_hostId, _connectionId, out _error);
			Debug.LogFormat("Disconnect: error={0}", _error);
		}

		private void Start()
		{
		}

		private void Update()
		{
			int recHostId; 
			int connectionId; 
			int channelId; 
			byte[] recBuffer = new byte[1024]; 
			int bufferSize = 1024;
			int dataSize;
			NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out _error);
			switch (recData)
			{
				//1	nothing interesting happened	
				case NetworkEventType.Nothing:         
					break;

				//2	Connection event come in
				case NetworkEventType.ConnectEvent:
					break;

				//3	 Data received. In this case recHostId will define host, connectionId will define connection, channelId will define channel; dataSize will define size of the received data. If recBuffer is big enough to contain data, data will be copied in the buffer. If not, error will contain MessageToLong error and you will need reallocate buffer and call this function again.
				case NetworkEventType.DataEvent:       
					break;

				//4 Disconnection signal come in. It can be signal that established connection has been disconnected or that your connect request is failed.
				case NetworkEventType.DisconnectEvent: 
					if (_connectionId == connectionId) {
						//cannot connect by some reason see error
					} else {
						//one of the established connection has been disconnected
					}
					
					break;
			}
		}

		private void FixedUpdate()
		{
		}

		private byte _error;
		private int _connectionId;
		private int _reiliableChannelId;
		private int _unreliableChannelId;
		private int _hostId;

	}
}


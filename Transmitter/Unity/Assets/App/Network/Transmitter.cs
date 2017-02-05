using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace App.Network
{
    public class Transmitter : NetworkPeerCommon 
	{
		public int Port;
		public string IpAddress { get { return _ipAddress;  } }

		public void PowerOn()
		{
			Configure();
		}

		public void Send(byte[] data)
		{
			Debug.LogFormat("_connectionId={0}", _connectionId);
			TestResult(NetworkTransport.Send(_hostId, _connectionId, 
				_reiliableChannelId, data, data.Length, out _error), "Send"
				);
		}

		public void Send(string text = "Hello World")
		{
			Send(ToBytes(text));
		}

		private void Awake()
		{
			 var def = GetComponentInParent<DefaultSession>();
			 Port = Port == 0 ? def.Port : Port;
			 _ipAddress = Utils.Net.GetMyAddress().ToString();
		}

		private void Update()
		{
			if (_connectionId == 0)
				return;

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
					Debug.Log("NothingEvent");
					break;

				//2	Connection event come in
				case NetworkEventType.ConnectEvent:
					Debug.Log("ConnectEvent");
					break;

				//3	 Data received. In this case recHostId will define host, connectionId will define connection, channelId will define channel; dataSize will define size of the received data. If recBuffer is big enough to contain data, data will be copied in the buffer. If not, error will contain MessageToLong error and you will need reallocate buffer and call this function again.
				case NetworkEventType.DataEvent:       
					Debug.LogFormat("DataEvent: {0}={1}", dataSize, ToString(recBuffer));
					break;

				//4 Disconnection signal come in. It can be signal that established connection has been disconnected or that your connect request is failed.
				case NetworkEventType.DisconnectEvent: 
					Debug.LogFormat("Disconnect: {0}, {1}", _connectionId, connectionId);;
					if (_connectionId == connectionId) {
						//cannot connect by some reason see error
						Debug.LogFormat("Cannot connect. error: {0}", _error);
					} else {
						//one of the established connection has been disconnected
						Debug.LogFormat("Disconnected. error: {0}", _error);
					}
					
					break;
			}
		}

		private void FixedUpdate()
		{
		}

		private void OnDestroy()
		{
			Disconnect();
		}

		private void OnApplicationQuit()
		{
			Disconnect();
		}

		private void Configure()
		{
			if (!NetworkTransport.IsStarted)
				NetworkTransport.Init();

			var config = new ConnectionConfig();
			_reiliableChannelId  = config.AddChannel(QosType.Reliable);
			_unreliableChannelId = config.AddChannel(QosType.Unreliable);

			var topology = new HostTopology(config, 10);
			_hostId = NetworkTransport.AddHost(topology, Port);

			Debug.LogFormat("Ip={0}, Port={1}. HostId={2}. ConnectionId={3}, Unreliable={4}, Reliable={5}", 
				_ipAddress, Port, _hostId, _connectionId, _unreliableChannelId, _reiliableChannelId);
		}

		private void Disconnect()
		{
			if (_hostId == -1)
				return;

			TestResult(NetworkTransport.Disconnect(_hostId, _connectionId, out _error), "Disconnect");

			NetworkManager.Shutdown();

			_hostId = -1;
		}

		private int _connectionId;
		private int _reiliableChannelId;
		private int _unreliableChannelId;
		private int _hostId;
		private string _ipAddress;
		private int _port;
	}
}


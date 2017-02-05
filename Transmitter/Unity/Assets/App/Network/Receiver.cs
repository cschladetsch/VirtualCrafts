using System.Net;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace App.Network
{
	// a peer in the network that is not a host
    public class Receiver : NetworkPeerCommon
	{
		public string TransmitterIp;
		public int Port;

		private void Awake()
		{
			var def = GetComponentInParent<DefaultSession>();
			TransmitterIp = string.IsNullOrEmpty(TransmitterIp) ? def.HostIp : TransmitterIp;
			Port = Port == 0 ? def.Port : Port;
		}

		private void Start()
		{
			if (!NetworkTransport.IsStarted)
				NetworkTransport.Init();
		}

		private void Update()
		{
			byte[] buffer = new byte[1024]; 
			int bufferSize = 1024;
			int receiveSize;
			byte error;

			NetworkEventType evnt = NetworkTransport.Receive(
				out _hostId, out _connectionId, out _channelId, buffer, bufferSize
				, out receiveSize, out error);

			switch (evnt)
			{
				case NetworkEventType.Nothing:
					break;

				case NetworkEventType.ConnectEvent:
					if ((NetworkError)error == NetworkError.Ok)
					{
						Debug.Log("Connected");
					}
					break;

				case NetworkEventType.DisconnectEvent:
					Debug.Log("Disconnected");
					// if (outHostId == hostId && 
					// outConnectionId == connectionId)
					// {
					// 	Debug.Log("Connected, error:" + error.ToString());
					// }
					break;

				case NetworkEventType.DataEvent:
					Debug.LogFormat("DataEvent: {0} {1}", receiveSize, ToString(buffer));
					break;

			}
		}

		public void PowerOn()
		{
			if (!NetworkTransport.IsStarted)
				NetworkTransport.Init();
			
			var ip = string.IsNullOrEmpty(TransmitterIp) ? Utils.Net.GetMyAddress() : IPAddress.Parse(TransmitterIp);
			_connectionId = NetworkTransport.Connect(_hostId, ip.ToString(), Port, 0, out _error);

			Debug.LogFormat("HostId={0}, ChannelId={1}, ConnectionId={2}", _hostId, _channelId, _connectionId);
		}

		public void Send(string text)
		{
			byte[] buffer = ToBytes(text);
			TestResult(NetworkTransport.Send(_hostId, _connectionId, _channelId, buffer, buffer.Length, out _error), "Send");
		}

		int _connectionId;
		int _channelId;
		int _hostId;
	}
}


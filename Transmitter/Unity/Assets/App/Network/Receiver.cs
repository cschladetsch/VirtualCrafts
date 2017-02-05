using System.Net;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace App.Network
{
    public class Receiver : MonoBehaviour 
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

			NetworkEventType evnt = NetworkTransport.Receive(out _hostId, out _connectionId, out _channelId, buffer, bufferSize, out receiveSize, out error);
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
					Debug.LogFormat("DataEvent: {0} {1}", receiveSize, Encoding.ASCII.GetString(buffer));
					break;

			}
		}

		private void FixedUpdate()
		{
		}

		public void PowerOn()
		{
			if (!NetworkTransport.IsStarted)
				NetworkTransport.Init();
			
			// ConnectionConfig config = new ConnectionConfig();
			// _channelId = config.AddChannel(QosType.Reliable);

			// HostTopology topology = new HostTopology(config, 1);

			// _hostId = NetworkTransport.AddHost(topology, Port);

			var ip = string.IsNullOrEmpty(TransmitterIp) ? Utils.Net.GetMyAddress() : IPAddress.Parse(TransmitterIp);
			_connectionId = NetworkTransport.Connect(_hostId, ip.ToString(), Port, 0, out _error);

			Debug.LogFormat("HostId={0}, ChannelId={1}, ConnectionId={2}", _hostId, _channelId, _connectionId);
		}

		public void SendText(string text)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(text);
			TestResult(NetworkTransport.Send(_hostId, _connectionId, _channelId, buffer, buffer.Length, out _error), "Send");
		}

		private void TestResult(bool result, string what = "")
		{
			if (result) return;

			Debug.LogErrorFormat("Failure: {0} with error {1}", what, _error);
		}

		int _connectionId;
		int _channelId;
		int _hostId;
		byte _error;
	}
}


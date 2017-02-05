using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using UnityEngine;
using UnityEngine.Assertions;

namespace App.Network
{
	public class AsyncClient : MonoBehaviour
	{
		public int Port = 11008;
		public string IpAddress = "192.168.0.2";
		public Connection Connection { get { return _connection; } }

		bool first = true;
		private void Update()
		{
			if (!first) return;
			if (Time.time < 1) return;

			first = false;

			var ip = Utils.Network.GetAddress(IpAddress);

			var socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_client = new TcpClient();
			Debug.Log("BeginConnectServer");
			_client.BeginConnect(Dns.GetHostAddresses(IpAddress), Port, new AsyncCallback(Connected), socket);
		}

        private void OnDestroy()
        {
			Close();
        }

		void Close()
		{
            if (_connection != null) _connection.Close();
            if (_client != null) _client.Close();

			_connection = null;
			_client = null;
		}

        private void OnApplicationQuit()
        {
			Close();
        }

		void Connected(IAsyncResult ar)
		{
			var socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);

			// Debug.LogFormat("Socket connected to {0}", socket.RemoteEndPoint.ToString());

			_connection = new Connection(socket);
			_connection.Receive();

			// _server.Send("World");
		}

		private TcpClient _client;
		private Connection _connection;
	}
}


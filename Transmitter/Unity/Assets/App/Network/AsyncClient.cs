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

		private void Start()
		{
			var ip = Utils.Network.GetAddress(IpAddress);

			var socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_client = new TcpClient();
			_client.BeginConnect(ip, Port, new AsyncCallback(Connected), socket);
		}

        private void OnDestroy()
        {
			Close();
        }

		void Close()
		{
            if (_server != null) _server.Close();
            if (_client != null) _client.Close();

			_server = null;
			_client = null;
		}

        private void OnApplicationQuit()
        {
			Close();
        }

		void Connected(IAsyncResult result)
		{
			var socket = (Socket)result;
			socket.EndConnect(result);

			_server = new Connection(socket);
		}

		private TcpClient _client;
		private Connection _server;
	}
}


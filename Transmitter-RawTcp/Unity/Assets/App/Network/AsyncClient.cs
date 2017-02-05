using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;
using UnityEngine.Assertions;

using App.Utils;

namespace App.Network
{	
	[RequireComponent(typeof(ExecuteOnMainThread))]
	public class AsyncClient : MonoBehaviour
	{
		public int Port = 11008;
		public string IpAddress = "192.168.0.2";
		public Connection Connection;
        // private ManualResetEvent connectionDone = new ManualResetEvent(false);

		void Awake()
		{
			_onMain = GetComponent<ExecuteOnMainThread>();
		}

		private void Start()
		{
			var ip = Utils.Network.GetAddress(IpAddress);

			var socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_client = new TcpClient();
			Debug.Log("BeginConnectServer");
			_client.BeginConnect(Dns.GetHostAddresses(IpAddress), Port, new AsyncCallback(Connected), socket);

			// connectionDone.WaitOne();
		}

		private void Update()
		{
			foreach (var act in _nextUpdateActions)
			{
				act();
			}
			_nextUpdateActions.Clear();
		}

        private void OnDestroy()
        {
			Close();
        }

		void Close()
		{
            if (Connection != null) Connection.Close();
            if (_client != null) _client.Close();

			Connection = null;
			_client = null;
		}

        private void OnApplicationQuit()
        {
			Close();
        }

		public void MainConnected(Socket socket)
		{
			Debug.Log("MainConnected: " + socket);
			Assert.IsNotNull(socket);
			Assert.IsTrue(socket.Connected);

			Connection = new Connection(socket);
		}

		void Connected(IAsyncResult ar)
		{
			var socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);
			Assert.IsNotNull(socket);

			Debug.Log("Scheduling MainConnected");
			_onMain.NextUpdate(() => MainConnected(socket));
		}

		private List<Action> _nextUpdateActions = new List<Action>();
		private TcpClient _client;
		private ExecuteOnMainThread _onMain;
	}
}

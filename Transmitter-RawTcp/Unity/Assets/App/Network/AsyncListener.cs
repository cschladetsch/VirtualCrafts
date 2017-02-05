using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

using App.Utils;

namespace App.Network
{
    [RequireComponent(typeof(ExecuteOnMainThread))]
    public class AsyncListener : MonoBehaviour
    {
        public int ListenPort = 11008;

        private void Awake()
        {
            _onMain = GetComponent<ExecuteOnMainThread>();
            StartListening();
        }

        public void StartListening()
        {
            var local = Utils.Network.GetMyAddress();
            if (local == null)
            {
                Debug.LogError("Not able to find an IP4 local address");
                return;
            }

            _listener = new TcpListener(local, ListenPort);
            _listener.Start();

            NextClient();
        }

        void NextClient()
        {
            // set event to not-signalled
            // _readyToConnect.Reset();
            try 
            {
                Debug.Log("Waiting for a client");

                _listener.BeginAcceptSocket(new AsyncCallback(AcceptCallback), _listener);

                // wait till next connection is finished
                // _readyToConnect.WaitOne();
            }
            catch (Exception e)
            {
                Debug.LogException(e, gameObject);
                DestroyImmediate(gameObject);
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Debug.Log("Client found! Do rest on main thread");
            TcpListener listener = (TcpListener) ar.AsyncState;
            var socket = listener.EndAcceptSocket(ar);
            _onMain.NextUpdate(() =>
            {
                var client = new Connection(socket);
                _clients[client.IPAddress] = client;
                NextClient();

                // signal whatever is waiting on this to continue
                //_readyToConnect.Set();
            });
        }

        private void Send(Connection client, String text)
        {
            client.Send(text);
        }

        public void SendToAll(string text)
        {
            foreach (var kv in _clients)
            {
                kv.Value.Send(text);
            }
        }

        private void OnDestroy()
        {
            Close();
        }

        private void OnApplicationQuit()
        {
            Close();
        }

        private void Close()
        {
            foreach (var kv in _clients)
            {
                kv.Value.Close();
            }
            _clients.Clear();

            if (_listener != null) _listener.Stop();
            _listener = null;

        }
        private TcpListener _listener;
        private Dictionary<IPAddress, Connection> _clients = new Dictionary<IPAddress, Connection>();
		private ExecuteOnMainThread _onMain;
        // private ManualResetEvent _readyToConnect = new ManualResetEvent(false);
    }
}

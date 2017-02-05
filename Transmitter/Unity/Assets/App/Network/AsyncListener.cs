using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using UnityEngine;

namespace App.Network
{
    public class AsyncListener : MonoBehaviour
    {
        public int ListenPort = 11008;

        private void Start()
        {
            StartListening();
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
        public void StartListening()
        {
            var local = Utils.Network.GetMyAddress();
            if (local == null)
            {
                Debug.LogError("Not about to find an IP4 local address");
                return;
            }

            _listener = new TcpListener(local, ListenPort);
            _listener.Start();

            NextClient();
        }

        void NextClient()
        {
            try 
            {
                Debug.Log("BeginAcceptSocket");
                _listener.BeginAcceptSocket(new AsyncCallback(AcceptCallback), _listener);
            }
            catch (Exception e)
            {
                Debug.LogException(e, gameObject);
                DestroyImmediate(gameObject);
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            var socket = _listener.EndAcceptSocket(ar);
            Connection client = new Connection(socket);
            _clients[client.IPAddress] = client;
            // client.Send("Hello");
            NextClient();
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

        private TcpListener _listener;
        private Dictionary<IPAddress, Connection> _clients = new Dictionary<IPAddress, Connection>();
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;

namespace App.Network
{
    public class AsyncNetworkListener : MonoBehaviour
    {
        public int ListenPort = 11002;

        public ManualResetEvent clientFound = new ManualResetEvent(false);

        private Socket _listener;

        private void Awake()
        {
            StartListening();
        }

        void OnApplicationQuit()
        {
            _listener.Shutdown(SocketShutdown.Both);
            _listener.Close();
        }

        IPAddress GetLocalAddress()
        {
            var hosts = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var address in hosts.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork) 
                {
                    return address;
                }
            }

            return null;
        }

        public void StartListening()
        {
            var local = GetLocalAddress();
            if (local == null)
            {
                Debug.LogError("Not about to find an IP4 local address");
                clientFound.Close();
                return;
            }

            IPEndPoint localEndPoint = new IPEndPoint(local, ListenPort);

            // Create a TCP/IP socket.
            _listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp );

            // Bind the socket to the local endpoint and listen for incoming connections.
            try 
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);

                while (true) 
                {
                    // Start an asynchronous socket to listen for connections.
                    Debug.LogFormat("Waiting for a connection...");

                    // Set the event to nonsignaled state.
                    clientFound.Reset();

                    var asyncResult = _listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        _listener);

                    // Wait until a connection is made before continuing.
                    clientFound.WaitOne();
                }

            } catch (Exception e) {
                Debug.LogException(e, null);
            }

            Debug.Log("Tcp Listener ends");
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            clientFound.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer,0,bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1) {
                    // All the data has been read from the
                    // client. Display it on the console.
                    Debug.LogFormat("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content );
                    // Echo the data back to the client.
                    Send(handler, content);
                } else {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try {
                // Retrieve the socket from the state object.
                Socket handler = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Debug.LogFormat("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            } catch (Exception e) {
				Debug.LogException(e, null);
            }
        }

    }

    // State object for reading client data asynchronously
    public class StateObject
    {
        // Size of receive buffer.
        public const int BufferSize = 1024;

        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client  socket.
        public Socket workSocket = null;
    }
}

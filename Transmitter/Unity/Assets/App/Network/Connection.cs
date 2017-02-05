using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using UnityEngine;
using UnityEngine.Assertions;

using UniRx;

namespace App.Network
{
    public class Connection
    {
		public StringReactiveProperty MessageSent = new StringReactiveProperty();
		public StringReactiveProperty MessageReceived = new StringReactiveProperty();

		public IPAddress IPAddress 
		{ 
			get 
			{ 
				var ep = _socket.LocalEndPoint as IPEndPoint;
				return ep == null ? null : ep.Address;
			}
		}

		public Connection(Socket socket)
		{
			_socket = socket;
		}

		public IAsyncResult Send(string text)
		{
			text = String.Format("{0} {1}", text.Length, text);
			_sendBuffer = Encoding.ASCII.GetBytes(text);
			_sendOffset = 0;
			return SendAsync();
		}

		private IAsyncResult SendAsync()
		{
            try
            {
				return _socket.BeginSend(
						_sendBuffer, _sendOffset, _sendBuffer.Length - _sendOffset, SocketFlags.None,
						new AsyncCallback(SendCallback), this);
			}
			catch (Exception e)
			{
				Debug.LogException(e, null);
				Close();
			}

			return null;
		}

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var state = (Connection)ar.AsyncState;
                _sendOffset += _socket.EndSend(ar);
                Debug.LogFormat("{0} bytes sent", _sendOffset);
				if (_sendOffset == _sendBuffer.Length)
				{
					// strip leading size string
					var text = Encoding.ASCII.GetString(_sendBuffer, 0, _sendBuffer.Length);
					var index = text.IndexOf(' ');
					MessageSent.Value = text.Substring(0, index);
					EndSend();
					return;
				}

				SendAsync();
            } 
            catch (Exception e) 
            {
				Debug.LogException(e, null);
            }
        }

		private void EndSend()
		{
			_sendBuffer = null;
			_sendOffset = 0;
		}

		public IAsyncResult Receive()
		{
			_recvBuffer = new byte[BufferSize];
			_recvOffset = 0;
			return ReceiveAsync();
		}

		IAsyncResult ReceiveAsync()
		{
            return _socket.BeginReceive(_recvBuffer, _recvOffset, BufferSize - _recvOffset, SocketFlags.None,
                new AsyncCallback(ReadCallback), this);
		}

        public void ReadCallback(IAsyncResult ar)
        {
            var state = (Connection) ar.AsyncState;
            var socket = state._socket;

            state._recvOffset += socket.EndReceive(ar);
			var text = Encoding.ASCII.GetString(
				state._recvBuffer, state._recvOffset, state._recvBuffer.Length - state._recvOffset);

			if (state._recvSize == 0)
			{
				var spaceIndex = text.IndexOf(' ');
				if (spaceIndex > 0)
				{
					state._recvSize = int.Parse(text.Substring(0, spaceIndex));
					Debug.Log("Expecting payload byte count of " + state._recvSize);
					state._recvBuffer = new Byte[state._recvSize];
					state._recvOffset = 0;
				}
			}

            if (state._recvOffset == state._sendBuffer.Length)
            {
                MessageReceived.Value = Encoding.ASCII.GetString(state._recvBuffer, 0, _recvSize);
            }

			ReceiveAsync();
        }

		public void Close()
		{
			EndSend();
			if (_socket == null)
				return;

			_sendBuffer = null;
			_socket.Close(100);
			_socket = null;
		}

		private void EndReceive()
		{
			_recvBuffer = null;
			_recvOffset = 0;
		}

        private const int BufferSize = 1024;

        private byte[] _sendBuffer = new byte[BufferSize];
		private int _sendOffset = 0;
		
        private byte[] _recvBuffer = new byte[BufferSize];
		private int _recvOffset = 0;
		private int _recvSize = 0;

        private Socket _socket = null;
    }
}


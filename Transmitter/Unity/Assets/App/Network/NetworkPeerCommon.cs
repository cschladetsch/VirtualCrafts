using System.Text;

using UnityEngine;

namespace App.Network
{
    /// <summary>
    /// Common for both hosts and clients 
    /// </summary>
    public abstract class NetworkPeerCommon : MonoBehaviour 
	{
		// TODO: these could/should be made concrete methods
		// public abstract void Send(byte[] data);
		// public abstract void Send(string text);

		public string GetErrorString(byte error)
		{
			return Error.GetString(error);
		}

		protected void TestResult(bool result, string what = "")
		{
			if (result) return;

			Debug.LogErrorFormat("Failure: {0} with error {1}", what, GetErrorString(_error));
		}

		protected string ToString(byte[] bytes)
		{
			return Encoding.ASCII.GetString(bytes);
		}

		protected byte[] ToBytes(string text)
		{
			return Encoding.ASCII.GetBytes(text);
		}

		protected string LastErrorString()
		{
			return GetErrorString(_error);
		}

		protected byte _error;
	}
}


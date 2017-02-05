
using UnityEngine;

namespace App.Network
{
    public class DefaultSession : MonoBehaviour 
	{
		public int Port = 1111;
		public string HostIp = "192.168.0.2";

		private void Awake()
		{
			if (!string.IsNullOrEmpty(HostIp))
				return;

			HostIp = Utils.Net.GetMyAddress().ToString();
		}
	}
}


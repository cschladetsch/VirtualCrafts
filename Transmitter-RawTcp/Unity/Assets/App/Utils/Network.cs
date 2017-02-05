using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace App.Utils
{
    public static class Network
	{
		public static IPAddress GetMyAddress(AddressFamily family = AddressFamily.InterNetwork)
		{
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(a => a.AddressFamily == family);
		}

		// note: this is synchronous use Dns.BeginGetHosAddresses for async
		public static IPAddress GetAddress(string ipaddres, AddressFamily family = AddressFamily.InterNetwork)
		{
			return Dns.GetHostAddresses(ipaddres).FirstOrDefault(a => a.AddressFamily == family);
		}
	}
}


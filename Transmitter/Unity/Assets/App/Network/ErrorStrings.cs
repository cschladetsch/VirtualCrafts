
using UnityEngine.Networking;

namespace App.Network
{
    public static class Error
	{
		public static string GetString(byte error)
		{
			switch ((NetworkError)error)
			{
			// 0
			case NetworkError.Ok: 
				return "The operation completed successfully";
			// 1
			case NetworkError.WrongHost: 
				return "The specified host not available.";
			// 2
        	case NetworkError.WrongConnection: 
				return "The specified connectionId doesn't exist.";
			// 3
			case NetworkError.WrongChannel: 
				return "The specified channel doesn't exist";
			// 4
        	case NetworkError.NoResources: 
				return "Not enough resources are available to process this request.";
			// 5
        	case NetworkError.BadMessage: 
				return "Not a data messagw.";
			// 6
			case NetworkError.Timeout:
				return "Connection timed out";
			// 7
			case NetworkError.MessageToLong:
				return "The message is too long to fit the buffer.";
			// 8
			case NetworkError.WrongOperation:
				return "Operation is not supported.";
			// 9
			case NetworkError.VersionMismatch:
				return "The protocol versions are not compatible. Check your library versions.";
			// 10
			case NetworkError.CRCMismatch:
				return "The Networking.ConnectionConfig does not match the other endpoint";
			// 11
			case NetworkError.DNSFailure:
				return "The address supplied to connect to was invalid or could not be resolved.";
			}

			return "Uknown error " + error;
		}
	}
}


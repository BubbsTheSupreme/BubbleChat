using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BubbleChat;
class Program
{
	static void Main(string[] args)
	{
		byte[] bytes = new byte[1024];

		try
		{
			IPHostEntry host = Dns.GetHostEntry("localhost");
			IPAddress ipAddress = host.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, 12345);

			Socket sender = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);
			try
			{
				sender.Connect(remoteEP);

				byte[] msg = Encoding.ASCII.GetBytes("This is a test[END]");

				int bytesSent = sender.Send(msg);

				int bytesRec = sender.Receive(bytes);
				Console.WriteLine("Echoed test = {0}",
					Encoding.ASCII.GetString(bytes, 0, bytesRec));

				sender.Shutdown(SocketShutdown.Both);
				sender.Close();

			}
			catch (ArgumentNullException ane)
			{
				Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
			}
			catch (SocketException se)
			{
				Console.WriteLine("SocketException : {0}", se.ToString());
			}
			catch (Exception e)
			{
				Console.WriteLine("Unexpected exception : {0}", e.ToString());
			}

		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}
}

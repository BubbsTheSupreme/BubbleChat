using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BubbleChat.Packets.Handler;

namespace BubbleChat.Client;

public class BubbleChatClient
{
	private Socket Socket;
	private string Username;
	private Thread RecvLoop;

	private IPAddress ServerIp;
	private ushort ServerPort;

	public BubbleChatClient(ushort port, string ip)
	{
		ServerPort = port;
		ServerIp = IPAddress.Parse(ip);
		Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

	public void Connect()
	{
		try
		{
			RecvLoop = new Thread(Receive);
			Socket.Connect(ServerIp, ServerPort);
            Console.WriteLine($"Connected to: {ServerIp}");
			RecvLoop.Start();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			Socket.Close();
			Socket.Dispose();
		}
	}

	public void Send(byte[] buffer)
	{
		try
		{
			Socket.Send(buffer);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			Socket.Close();
			Socket.Dispose();
		}
	}

	public void Receive()
	{
		try
		{
			while(Socket.Connected)
			{
				byte[] buffer = new byte[2];
				int count = Socket.Receive(buffer);
				if (count == 0) break;
				ushort size = BitConverter.ToUInt16(buffer, 0);
				buffer = new byte[size];
				count = Socket.Receive(buffer);
				PacketRouter.Process(Socket, buffer);
			}
		}
		catch(Exception e)
		{
			Console.WriteLine(e);
			Socket.Close();
			Socket.Dispose();
		}
	}
}
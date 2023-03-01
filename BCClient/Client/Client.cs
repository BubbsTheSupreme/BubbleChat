using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BubbleChat.Packets.Handler;

namespace BubbleChat.Client;

public class BubbleChatClient // NEEDS ONDISCONNECT EVENT
{
	private Socket Socket;
	// private string Username;
	private Thread RecvLoop;
	public static event Action<string> OnDisconnect;

	public BubbleChatClient()
	{
		Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

	public bool IsConnected()
	{
		if(Socket.Connected == true)
			return true;
		else 
			return false;
	}

	public void Connect(ushort port, string ip)
	{
		try
		{
			RecvLoop = new Thread(Receive);
			Socket.Connect(ip, port);
            Console.WriteLine($"Connected to: {ip}");
			RecvLoop.Start();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			OnDisconnect?.Invoke("Could not connect..");
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
			OnDisconnect?.Invoke("Connection has been lost");
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
			OnDisconnect?.Invoke("Connection has been lost");
			Socket.Close();
			Socket.Dispose();
		}
	}

	public void Disconnect()
	{
		Socket.Close();
	}
}
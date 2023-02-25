using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using BubbleChat.Packets.Handler;

namespace BubbleChat.Server;

public class ClientConnection
{
	public Socket Client;
	public string Username;

	public ClientConnection(Socket client)
	{
		Client = client;
	}

	public void Send(byte[] buffer)
	{
		try
		{
			Client.Send(buffer);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			Client.Close();
			Client.Dispose();
		}
	}

	public void Close()
	{
		try
		{
			Client.Close();
			Client.Dispose();
		}
		catch(Exception e)
		{
			Console.WriteLine(e);
		}
	}
}

public class BubbleChatServer
{
	private Socket Socket;
	private Thread RecvLoop;
	private IPAddress ServerIp;
	private IPEndPoint ServerEndPoint;
	private List<ClientConnection> Connections = new();

	public BubbleChatServer(ushort port)
	{
		try 
		{
			// GET IP
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			ServerIp = host.AddressList[1];
			ServerEndPoint = new(ServerIp, port);
			// CREATE SOCKET WITH IP
			Socket = new(ServerIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			Socket.Bind(ServerEndPoint);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			Socket.Close();
			Socket.Dispose();
		}
	}

	public void StartListening()
	{
		try
		{
			RecvLoop = new(Receive);
			Console.WriteLine("Listening for connections...");
			Socket.Listen();
			Socket client = Socket.Accept();
			ClientConnection clientCon = new(client);
			Connections.Add(clientCon);
			RecvLoop.Start(clientCon);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			Socket.Close();
			Socket.Dispose();
		}
	}

	public void Receive(object args)
	{
		ClientConnection client = (ClientConnection)args;
		try
		{
			while(client.Client.Connected)
			{
				byte[] buffer = new byte[2];
				int count = client.Client.Receive(buffer);
				if (count == 0) break;
				ushort size = BitConverter.ToUInt16(buffer, 0);
				buffer = new byte[size];
				count = client.Client.Receive(buffer);
				PacketRouter.Process(client, buffer);
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
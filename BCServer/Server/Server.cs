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
	public BubbleChatServer Server;
	public string Username;
	public byte UserId;
	public IPAddress ClientIp;
	private Thread RecvLoop;
	public static event Action<string> OnDisconnect;

	public ClientConnection(Socket client, BubbleChatServer server)
	{
		Client = client;
		Server = server;
		ClientIp = IPAddress.Parse(((IPEndPoint)Client.RemoteEndPoint).Address.ToString()); //Gets Ip
		RecvLoop = new(Receive);
		RecvLoop.Start(this);
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
			OnDisconnect?.Invoke(Username);
			Client.Close();
			Client.Dispose();
		}
	}

	public void Close()
	{
		try
		{
			OnDisconnect?.Invoke(Username);
			Client.Close();
			Client.Dispose();
		}
		catch(Exception e)
		{
			Console.WriteLine(e);
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
			OnDisconnect?.Invoke(client.Username);
			Console.WriteLine(e);
			Client.Close();
			Client.Dispose();
		}
	}
}

public class BubbleChatServer
{
	private Socket Socket;
	private IPAddress ServerIp;
	private IPEndPoint ServerEndPoint;
	public List<ClientConnection> Connections = new(); //keeps track of all connections

	public BubbleChatServer(ushort port)
	{
		try 
		{
			ClientConnection.OnDisconnect += RemoveClientConnection;
			// GET IP
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			ServerIp = host.AddressList[1];
			ServerEndPoint = new(ServerIp, port);
			// CREATE SOCKET WITH IP
			Socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
			byte latestId = 0;
			Console.WriteLine("Listening for connections...");
			while(true)
			{
				Socket.Listen();
				Socket client = Socket.Accept(); // Get client connection socket 
				ClientConnection clientCon = new(client, this); //create new client connection
				clientCon.UserId = latestId;
				Connections.Add(clientCon); // add client connection to list of connections
				if (latestId == 255) latestId = 0;
				else latestId++;
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			Socket.Close();
			Socket.Dispose();
		}
	}

	public void RemoveClientConnection(string username)
	{
		foreach (ClientConnection connection in Connections.ToArray())
		{
			if (connection.Username == username)
				Connections.Remove(connection);
		}
	}

}
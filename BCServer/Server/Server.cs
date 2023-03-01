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
	public IPAddress ClientIp;
	private Thread RecvLoop;
	public static event Action<string> OnDisconnect;

	public ClientConnection(Socket client)
	{
		Client = client;
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
	private List<ClientConnection> Connections = new(); //keeps track of all connections

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
			Console.WriteLine("Listening for connections...");
			while(true)
			{
				Socket.Listen();
				Socket client = Socket.Accept(); // Get client connection socket 
				ClientConnection clientCon = new(client); //create new client connection 
				Connections.Add(clientCon); // add client connection to list of connections
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			Socket.Close();
			Socket.Dispose();
		}
	}

}
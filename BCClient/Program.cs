using System;
using System.Threading;
using BubbleChat.UI;
using BubbleChat.Client;
using BubbleChat.Packets;
using BubbleChat.Logging;

namespace BubbleChat;

class Program
{
	static void Main(string[] args)
	{
		Logger.Init();
		BubbleChatClient client = new();
		// UserInterface ui = new();
		Console.Write("Username: ");
		string username = Console.ReadLine();
		Console.Write("Password: ");
		string password = Console.ReadLine();

		client.Connect(12345, "192.168.1.114");

		Packet usernamePacket = new(0);
		usernamePacket.WriteSecondId(0).WriteContent(username);
		client.Send(usernamePacket.Finalize());

		Thread.Sleep(500);

		Packet passwordPacket = new(0);
		passwordPacket.WriteSecondId(1).WriteContent(password);
		client.Send(passwordPacket.Finalize());

		Packet messagePacket;
		string message;

		while (client.IsConnected() == true)
		{
			message = Console.ReadLine();
			messagePacket = new(1);
			messagePacket.WriteSecondId(client.UserId).WriteContent(message);
			client.Send(messagePacket.Finalize());
		}
	}
}

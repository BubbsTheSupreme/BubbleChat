using System;
using BubbleChat.Server;

namespace BubbleChat;
class Program
{
	static void Main(string[] args)
	{
		BubbleChatServer server = new((ushort)12345);
		server.StartListening();
	}
}

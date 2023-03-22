using System;
using BubbleChat.Server;
using BubbleChat.Logging;
using BubbleChat.Config;

namespace BubbleChat;
class Program
{
	static void Main(string[] args)
	{
		Logger.Init();
		ServerConfig.InitConfig("config.xml");
		BubbleChatServer server = new((ushort)12345);
		server.StartListening();
	}
}

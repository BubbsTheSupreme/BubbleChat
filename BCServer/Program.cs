using System;
using BubbleChat.Server;
using BubbleChat.Logging;
using BubbleChat.Config;
using BubbleChat.Packets.Filtering;

namespace BubbleChat;
class Program
{
	static void Main(string[] args)
	{
		Logger.Init();
		ServerFilter.InitFilter("filter.xml");
		ServerConfig.InitConfig("config.xml");
		BubbleChatServer server = new((ushort)12345);
		server.StartListening();
	}
}

using BubbleChat.UI;
using BubbleChat.Logging;

namespace BubbleChat;

class Program
{
	static void Main(string[] args)
	{
		Logger.Init();
		UserInterface ui = new();
	}
}

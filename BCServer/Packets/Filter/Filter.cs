using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BubbleChat.Logging;

namespace BubbleChat.Packets.Filtering;

public static class ServerFilter
{
	public static string FilterStatus;
	public static string CensorCharacter;
	public static Dictionary<string, string> BannedWords = new();
	public static XmlDocument XmlFilter = new();

	public static void InitFilter(string path)
	{
		Logger.Info("Loading filter.xml data..");
		Console.WriteLine("Loading filter.xml data..");
		XmlFilter.Load(path);

		var censorNode = XmlFilter.SelectSingleNode("//filter/censor");
		var wordsNode = XmlFilter.SelectSingleNode("//filter/bannedwords");

		FilterStatus = censorNode.Attributes["status"].InnerText;
		CensorCharacter = censorNode.InnerText;
		foreach (XmlNode word in wordsNode.ChildNodes) BannedWords.Add(word.InnerText, word.Attributes["bannable"].InnerText);
	}

	public static Tuple<string, bool, string> Filter(string message)
	{  
		bool banCheck = false;
		string filteredMessage = message;
		foreach (var (word, bannable) in BannedWords)
		{
			string replacement = string.Concat(Enumerable.Repeat(CensorCharacter, word.Length));
			Match banMatch = Regex.Match(filteredMessage, word);
			if (bannable == "yes") banCheck = banMatch.Success;
			filteredMessage = Regex.Replace(filteredMessage, word, replacement, RegexOptions.IgnoreCase);
		}
	
		return Tuple.Create(filteredMessage, banCheck, message);
	}
}
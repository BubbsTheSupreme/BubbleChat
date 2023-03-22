using NLog;

namespace BubbleChat.Logging;

public static class Logger
{
	private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
	public static void Init()
	{
		var config = new NLog.Config.LoggingConfiguration();
		var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "BubbleChat.log" };
		config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
		config.AddRule(LogLevel.Error, LogLevel.Fatal, logfile);
		NLog.LogManager.Configuration = config;

	}

	public static void Info(string message)
	{
		_log.Info(message);
	}

	public static void Error(string message)
	{
		_log.Error(message);
	}
}
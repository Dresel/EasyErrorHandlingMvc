namespace EasyErrorHandlingMvc.Samples.Mvc5Extended
{
	using NLog;
	using NLog.Config;
	using NLog.Targets;

	public static class LoggingConfig
	{
		public static void ConfigureLogging()
		{
			// Step 1. Create configuration object
			LoggingConfiguration config = new LoggingConfiguration();

			// Step 2. Create targets and add them to the configuration
			MemoryTarget memoryTarget = new MemoryTarget() { Name = "memory" };
			config.AddTarget("memory", memoryTarget);

			// Step 3. Set target properties
			memoryTarget.Layout = @"${date:format=HH\\:MM\\:ss} ${logger} ${message}";

			// Step 4. Define rules
			LoggingRule rule1 = new LoggingRule("*", LogLevel.Debug, memoryTarget);
			config.LoggingRules.Add(rule1);

			// Step 5. Activate the configuration
			LogManager.Configuration = config;
		}
	}
}
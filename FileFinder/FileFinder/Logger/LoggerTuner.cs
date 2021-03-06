﻿using Serilog;


namespace FileFinder.Logger
{
    public static class LoggerTuner
    {
        public static ILogger TuneLoge()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs\\logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}

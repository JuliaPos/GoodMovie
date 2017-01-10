using GoodMovie.Contracts;
using GoodMovie.Logger.Overrides;
using MetroLog;
using MetroLog.Internal;

namespace GoodMovie.Logger
{
    public class AppLogger : IAppLogger
    {
        private ILogger _log;

        public AppLogger()
        {
            var logFileConfig = new StreamingFileTargetWithFolder { RetainDays = 7 };
            logFileConfig.SetLogFolderName("Foreground Logs");

            LogManagerFactory.DefaultConfiguration = new LogConfiguratorBase().CreateDefaultSettings();
#if DEBUG
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, logFileConfig);
#else
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Error, LogLevel.Error, logFileConfig);
#endif
        }

        public void Info(string msg)
        {
            _log = LogManagerFactory.DefaultLogManager.GetLogger(GetType());
            _log.Info(msg);
        }

        public void Error(string msg)
        {
            _log = LogManagerFactory.DefaultLogManager.GetLogger(GetType());
            _log.Error(msg);
        }

    }
}

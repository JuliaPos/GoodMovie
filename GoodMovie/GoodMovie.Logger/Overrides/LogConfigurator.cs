using MetroLog;
using MetroLog.Internal;

namespace GoodMovie.Logger.Overrides
{
    internal sealed class LogConfigurator : LogConfiguratorBase
    {
        public override LoggingConfiguration CreateDefaultSettings()
        {
            var def = base.CreateDefaultSettings();
            def.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTargetWithFolder());

            return def;
        }
    }
}
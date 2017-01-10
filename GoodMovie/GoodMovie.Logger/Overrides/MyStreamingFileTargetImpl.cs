using System.IO;
using System.Threading.Tasks;
using MetroLog.Layouts;
using MetroLog.Targets;

namespace GoodMovie.Logger.Overrides
{
    internal class MyStreamingFileTargetImpl : MyStreamingFileTarget
    {
        protected string FolderName = "Logs";

        public MyStreamingFileTargetImpl() : this(new SingleLineLayout())
        {
        }

        public MyStreamingFileTargetImpl(Layout layout) : base(layout)
        {
            FileNamingParameters.IncludeLevel = false;
            FileNamingParameters.IncludeLogger = false;
            FileNamingParameters.IncludeSequence = false;
            FileNamingParameters.IncludeSession = false;
            FileNamingParameters.IncludeTimestamp = FileTimestampMode.Date;
            FileNamingParameters.CreationMode = FileCreationMode.AppendIfExisting;
        }

        protected override string GetLogFolderName()
        {
            return FolderName;
        }

        protected override Task WriteTextToFileCore(StreamWriter file, string contents)
        {
            return file.WriteLineAsync(contents);
        }
    }
}

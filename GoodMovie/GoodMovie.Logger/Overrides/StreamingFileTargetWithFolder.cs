using MetroLog.Layouts;

namespace GoodMovie.Logger.Overrides
{
    internal class StreamingFileTargetWithFolder : MyStreamingFileTargetImpl
    {
        public StreamingFileTargetWithFolder() : this(new SingleLineLayout())
        {
        }

        public StreamingFileTargetWithFolder(Layout layout) : base(layout)
        {
        }

        public void SetLogFolderName(string newFolder)
        {
            if (!string.IsNullOrEmpty(newFolder))
            {
                FolderName = newFolder;
            }
        }
    }
}
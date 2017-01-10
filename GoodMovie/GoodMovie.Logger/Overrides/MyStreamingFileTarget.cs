using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using MetroLog;
using MetroLog.Layouts;
using MetroLog.Targets;

namespace GoodMovie.Logger.Overrides
{
    internal abstract class MyStreamingFileTarget : FileTargetBase
    {
        private static StorageFolder _logFolder;

        protected MyStreamingFileTarget(Layout layout) : base(layout)
        {
        }

        protected abstract string GetLogFolderName();

        public async Task<StorageFolder> EnsureInitializedAsync()
        {
            if (_logFolder == null)
            {
                var root = ApplicationData.Current.LocalFolder;
                _logFolder = await root.CreateFolderAsync(GetLogFolderName(), CreationCollisionOption.OpenIfExists);
            }
            return _logFolder;
        }

        protected override Task EnsureInitialized()
        {
            return EnsureInitializedAsync();
        }

        protected override async Task DoCleanup(Regex pattern, DateTime threshold)
        {
            var zipPattern = new Regex(@"^Log(.*).zip$");
            var toDelete = new List<StorageFile>();

            foreach (var file in await _logFolder.GetFilesAsync())
            {
                if (pattern.Match(file.Name).Success && file.DateCreated <= threshold)
                    toDelete.Add(file);

                if (zipPattern.Match(file.Name).Success)
                    toDelete.Add(file);
            }

            foreach (var file in toDelete)
            {
                try
                {
                    await file.DeleteAsync();
                }
                catch (Exception ex)
                {
                    InternalLogger.Current.Warn($"Failed to delete '{file.Path}'.", ex);
                }
            }
        }

        private static async Task<byte[]> ReadStorageFileToByteBuffer(IStorageFile storageFile)
        {
            IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
            byte[] content = null;

            using (var stream = accessStream.AsStreamForRead((int) accessStream.Size))
            {
                content = new byte[stream.Length];
                await stream.ReadAsync(content, 0, (int) stream.Length);
            }

            return content;
        }

        private async Task ZipFolderContents(StorageFolder sourceFolder, ZipArchive archive, string baseDirPath)
        {
            var files = await sourceFolder.GetFilesAsync();
            var pattern = FileNamingParameters.GetRegex();

            foreach (var file in files)
            {
                if (pattern.Match(file.Name).Success)
                {
                    var readmeEntry = archive.CreateEntry(file.Name);

                    var buffer = await ReadStorageFileToByteBuffer(file);

                    var entryStream = readmeEntry.Open();
                    await entryStream.WriteAsync(buffer, 0, buffer.Length);
                    await entryStream.FlushAsync();
                    entryStream.Dispose();
                }
            }
        }

        protected override async Task<Stream> GetCompressedLogsInternal()
        {
            var logFileName = "Logs-Dump.zip";

            // create log file and output stream 
            var zippedStorageFile =
                await _logFolder.CreateFileAsync(logFileName, CreationCollisionOption.ReplaceExisting);
            var logoutputStream = await zippedStorageFile.OpenStreamForWriteAsync();

            // archive  
            var zipArchive = new ZipArchive(logoutputStream, ZipArchiveMode.Create, false);
            await ZipFolderContents(_logFolder, zipArchive, logFileName);

            // release outfile stream 
            await logoutputStream.FlushAsync();
            zipArchive.Dispose();
            logoutputStream.Dispose();


            // get inputstream for reading 
            var loginputStream = await _logFolder.OpenStreamForReadAsync(logFileName);
            return loginputStream;
        }

        protected override async Task<LogWriteOperation> DoWriteAsync(StreamWriter streamWriter, string contents,
            LogEventInfo entry)
        {
            // Write contents 
            await WriteTextToFileCore(streamWriter, contents).ConfigureAwait(false);

            // return... 
            return new LogWriteOperation(this, entry, true);
        }

        protected abstract Task WriteTextToFileCore(StreamWriter file, string contents);

        protected override async Task<Stream> GetWritableStreamForFile(string fileName)
        {
            var file =
                await
                    _logFolder.CreateFileAsync(fileName,
                        FileNamingParameters.CreationMode == FileCreationMode.AppendIfExisting
                            ? CreationCollisionOption.OpenIfExists
                            : CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenStreamForWriteAsync();
            if (FileNamingParameters.CreationMode == FileCreationMode.AppendIfExisting)
            {
                // Make sure we're at the end of the stream for appending
                stream.Seek(0, SeekOrigin.End);
            }
            return stream;
        }
    }
}
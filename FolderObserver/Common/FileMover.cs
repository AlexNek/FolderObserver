using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using log4net;

namespace FolderObserver.Common
{
    internal class FileMover
    {
        private const int FileStreamDefaultBufferSize = 4096;
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task<bool> MoveAsync(string sourceFilePath, string destFilePath)
        {
            sourceFilePath.AssertArgumentHasText(nameof(sourceFilePath));
            destFilePath.AssertArgumentHasText(nameof(destFilePath));

            if (!File.Exists(sourceFilePath))
            {
                return await Task.FromResult(false);
            }

            await Task.Run(() => { MoveFile(sourceFilePath, destFilePath); });

            return await Task.FromResult(true);
        }

        public static bool Move(string sourceFilePath, string destFilePath, bool overrideDestination=true)
        {
            sourceFilePath.AssertArgumentHasText(nameof(sourceFilePath));
            destFilePath.AssertArgumentHasText(nameof(destFilePath));
            _log.Debug($"Move file {sourceFilePath} to {destFilePath}");
            if (overrideDestination)
            {
                if (File.Exists(destFilePath))
                {
                    File.Delete(destFilePath);
                }
            }

            if (!File.Exists(sourceFilePath))
            {
                _log.Debug($"File not exist {sourceFilePath}");
                return false;
            }

            MoveFile(sourceFilePath, destFilePath);

            return true;
        }

        public static async Task MoveAsync2(string sourceFilePath, string destFilePath)
        {
            sourceFilePath.AssertArgumentHasText(nameof(sourceFilePath));
            destFilePath.AssertArgumentHasText(nameof(destFilePath));

            if (IsUncPath(sourceFilePath) || HasNetworkDrive(sourceFilePath) || IsUncPath(destFilePath) || HasNetworkDrive(destFilePath))
            {
                await InternalCopyToAsync(sourceFilePath, destFilePath, FileOptions.DeleteOnClose).ConfigureAwait(false);
                return;
            }

            FileInfo sourceFileInfo = new FileInfo(sourceFilePath);
            string sourceDrive = Path.GetPathRoot(sourceFileInfo.FullName);

            FileInfo destFileInfo = new FileInfo(destFilePath);
            string destDrive = Path.GetPathRoot(destFileInfo.FullName);

            if (sourceDrive == destDrive)
            {
                File.Move(sourceFilePath, destFilePath);
                return;
            }

            await Task.Run(() => File.Move(sourceFilePath, destFilePath)).ConfigureAwait(false);
        }

        private static bool HasNetworkDrive(string path)
        {
            try
            {
                return new DriveInfo(path).DriveType == DriveType.Network;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static async Task InternalCopyToAsync(
            string sourceFilePath,
            string destFilePath,
            FileOptions? sourceFileOptions = null,
            bool overwrite = false)
        {
            sourceFilePath.AssertArgumentHasText(nameof(sourceFilePath));
            destFilePath.AssertArgumentHasText(nameof(destFilePath));

            var sourceStreamFileOpt = (sourceFileOptions ?? FileOptions.SequentialScan) | FileOptions.Asynchronous;

            using (FileStream sourceStream = new FileStream(
                sourceFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                FileStreamDefaultBufferSize,
                sourceStreamFileOpt))
            using (FileStream destinationStream = new FileStream(
                destFilePath,
                overwrite ? FileMode.Create : FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                FileStreamDefaultBufferSize,
                true))
            {
                await sourceStream.CopyToAsync(destinationStream, FileStreamDefaultBufferSize).ConfigureAwait(false);
            }
        }

        private static bool IsUncPath(string path)
        {
            try
            {
                return new Uri(path).IsUnc;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void MoveFile(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }
    }
}
/*
 * https://blog.stephencleary.com/2013/11/taskrun-etiquette-examples-dont-use.html
 * Do not use Task.Run in the implementation of the method; instead, use Task.Run to call the method
 */

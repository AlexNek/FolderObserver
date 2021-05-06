using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;

using log4net;

namespace FolderObserver.Common
{
    internal class FileCompressor
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string Compress(string fullFileName)
        {
            _log.Debug($"Compress {fullFileName}");
            string archiveFileName = GetArchiveFileName(fullFileName);

            if (File.Exists(archiveFileName))
            {
                File.Delete(archiveFileName);
            }

            using (ZipArchive zip = ZipFile.Open(archiveFileName, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(fullFileName, Path.GetFileName(fullFileName), CompressionLevel.Optimal);
            }

            //zip.Dispose();
            return archiveFileName;
        }

        public static async Task CompressAsync(string fullFileName)
        {
            await Task.Run(() => Compress(fullFileName));
        }

        public static string GetArchiveFileName(string fullFileName)
        {
            string name = Path.GetFileNameWithoutExtension(fullFileName) + ".zip";
            return Path.Combine(Path.GetDirectoryName(fullFileName), name);
        }
    }
}

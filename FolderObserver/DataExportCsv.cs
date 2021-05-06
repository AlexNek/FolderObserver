using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FolderObserver.Model;

using log4net;

namespace FolderObserver
{
    internal class DataExportCsv : IDataExport
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _fileName;

        public DataExportCsv(string fileName)
        {
            _fileName = fileName;
        }

        public void Export(DataItems data)
        {
            throw new NotImplementedException();
        }

        public async Task ExportAsync(DataItems data)
        {
            StringBuilder sb = new StringBuilder();
            ExportHeader(sb);
            foreach (FileItem item in data)
            {
                ExportItem(item,sb);
            }
            await WriteFileAsync(_fileName, sb.ToString());
        }

        private void ExportItem(FileItem fileItem, StringBuilder sb)
        {
            sb.AppendFormat("{0:G},{1:G},{2}",fileItem.CopyDate,fileItem.TimeStamp,fileItem.Name);
            sb.AppendLine();
        }

        private void ExportHeader(StringBuilder sb)
        {
            sb.AppendLine("Copy time,File Time,Name");
        }

        private static async Task WriteFileAsync(string fullFileName, string content)
        {
            _log.Debug("Async Write File has started");
            using (StreamWriter outputFile = new StreamWriter(fullFileName))
            {
                await outputFile.WriteAsync(content);
            }

            _log.Debug("Async Write File has completed");
        }
    }
}

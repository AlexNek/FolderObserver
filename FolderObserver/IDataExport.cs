using System.Threading.Tasks;

using FolderObserver.Model;

namespace FolderObserver
{
    internal interface IDataExport
    {
        void Export(DataItems data);
        Task ExportAsync(DataItems data);
    }
}

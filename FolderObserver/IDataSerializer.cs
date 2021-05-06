using System.Threading.Tasks;

using FolderObserver.Model;

namespace FolderObserver
{
    public interface IDataSerializer
    {
        DataItems Load();

        Task<DataItems> LoadAsync();

        void Store(DataItems data);

        Task StoreAsync(DataItems data);
    }
}

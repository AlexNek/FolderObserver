using System;
using System.IO;
using System.Threading.Tasks;

using FolderObserver.Common;
using FolderObserver.Model;

namespace FolderObserver
{
    public class DataSerializer:IDataSerializer
    {
        private readonly string _fileName;

        public DataSerializer()
        {
            _fileName = Path.Combine(CommonDirectories.GetAllUsersApplicationData(), "ObserverStored.data");
        }

        public DataItems Load()
        {
            DataItems data;
            if (File.Exists(_fileName))
            {
                data = XmlSerializationUtil.Deserialize<DataItems>(_fileName);
            }
            else
            {
                data = new DataItems();
            }

            return data;
        }

        public async Task<DataItems> LoadAsync()
        {
            DataItems data;
            if (File.Exists(_fileName))
            {
                data = await XmlSerializationUtil.DeserializeAsync<DataItems>(_fileName);
            }
            else
            {
                data = new DataItems();
            }

            return await Task.FromResult(data);
        }

        public void Store(DataItems data)
        {
            try
            {
                XmlSerializationUtil.Serialize(data, _fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task StoreAsync(DataItems data)
        {
            try
            {
                await XmlSerializationUtil.SerializeAsync(data, _fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

#pragma warning disable 168

namespace FolderObserver.Common
{
    public static class XmlSerializationUtil
    {
        public static T Deserialize<T>(XmlDocument xmlDocument)
            where T : new()
        {
            T deserializedObject = new T();

            try
            {
                MemoryStream xmlDocStream = new MemoryStream();
                xmlDocument.Save(xmlDocStream);

                //ignore System.IO.FileNotFoundException, Ms problem not our
                //XmlSerializer serializer = new XmlSerializer(typeof(T));

                //here is the trick for removing FileNotFoundException
                XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(T) })[0];

                xmlDocStream.Seek(0, SeekOrigin.Begin);
                deserializedObject = (T)serializer.Deserialize(xmlDocStream);

                xmlDocStream.Dispose();
            }
            // TODO: Handle Specific Exception.
            catch (Exception generalException)
            {
                // Log Proper Exception Message.
            }

            return deserializedObject;
        }

        public static T Deserialize<T>(string xmlFileFullName)
            where T : new()
        {
            T deserializedObject = new T();

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlFileFullName);

                deserializedObject = Deserialize<T>(xmlDocument);
            }
            // TODO: Handle Specific Exception.
            catch (Exception generalException)
            {
                // Log Proper Exception Message.
            }

            return deserializedObject;
        }

        public static async Task<T> DeserializeAsync<T>(string xmlFileFullName)
            where T : new()
        {
            // as ms don't give us async methods - use simple wrapper
            await Task.Delay(0);
            var result = Deserialize<T>(xmlFileFullName);
            return await Task.FromResult(result);
        }

        public static void Serialize<T>(T details, string xmlFileFullName)
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(T));

            //here is the trick for removing FileNotFoundException
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(T) })[0];

            //remove unused namespaces
            XmlSerializerNamespaces xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);

            string directoryName = Path.GetDirectoryName(xmlFileFullName);
            if (!Directory.Exists(directoryName))
            {
                if (!String.IsNullOrEmpty(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }

            using (TextWriter writer = new StreamWriter(xmlFileFullName, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, details, xns);
            }
        }

        public static async Task SerializeAsync<T>(T details, string xmlFileFullName)
        {
            // as ms don't give us async methods - use simple wrapper
            await Task.Delay(0);
            Serialize(details, xmlFileFullName);
        }
    }
}

/*
 https://msdn.microsoft.com/en-us/library/system.runtime.serialization.datacontractserializer.aspx
 */

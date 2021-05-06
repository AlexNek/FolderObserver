using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FolderObserver.Common
{
    public static class CommonDirectories
    {
        
        /// <summary>
        /// Adds the company name version suffix.
        /// Company must be defined in assembly info
        /// </summary>
        /// <param name="commonApplicationData">The common application data.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static string AddCompanyNameVersionSuffix(string commonApplicationData, Assembly assembly)
        {
            //use exe assembly
            // Assembly assembly = Assembly.GetEntryAssembly(); //Assembly.GetExecutingAssembly();
            //string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = GetVersion(fvi);
            string productName = fvi.ProductName;
            string company = "";
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length > 0)
            {
                company = ((AssemblyCompanyAttribute)attributes[0]).Company;
            }

            string allUsersApplicationData = Path.Combine(commonApplicationData, company, productName, version);
            return allUsersApplicationData;
        }

        /// <summary>
        /// Gets all users application data path.
        /// Note:This function must be called direct from exe assembly for correct using from unit tests
        /// </summary>
        /// <param name="assembly">The assembly. Default it is exe assembly</param>
        /// <returns></returns>
        public static string GetAllUsersApplicationData(Assembly assembly = null)
        {
            string commonApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string allUsersApplicationData;

            //use exe assembly
            //Assembly assembly; //Assembly.GetExecutingAssembly();
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }
            //string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //correction for unit test calling
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
                string extension = Path.GetExtension(assembly.Location);
                //it is possible we use dll
                if (extension.ToLower() != ".exe") //MLHIDE
                {
                    //throw new ApplicationException("Function GetAllUsersApplicationData must be called from exe");
                }
            }
            
            allUsersApplicationData = AddCompanyNameVersionSuffix(commonApplicationData, assembly);
            if (!Directory.Exists(allUsersApplicationData))
            {
                Directory.CreateDirectory(allUsersApplicationData);
            }
            return allUsersApplicationData;
        }

        public static string GetCurrentUserApplicationData()
        {
            //_Path = "C:\\Users\\???\\AppData\\Roaming\\"
            //folderPath = "C:\\Users\\???\\AppData\\Roaming"
            //_User.Data = CreateObj(serverComputer.FileSystem.SpecialDirectories.CurrentUserApplicationData);
            //string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string currentUserApplicationData;

            //use exe assembly
            Assembly assembly = Assembly.GetEntryAssembly(); //Assembly.GetExecutingAssembly();
            //string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //correction for unit test calling
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }

            currentUserApplicationData = AddCompanyNameVersionSuffix(applicationData, assembly);

            if (!Directory.Exists(currentUserApplicationData))
            {
                Directory.CreateDirectory(currentUserApplicationData);
            }
            return currentUserApplicationData;
        }

        private static string GetVersion(FileVersionInfo fvi)
        {
            //FileMajorPart = 1
            //FileMinorPart = 0
            //FileBuildPart = 2
            //FilePrivatePart = 3
            //string version = fvi.FileVersion;
            string version = String.Format((("{0}.{1}")), fvi.ProductMajorPart, fvi.ProductMinorPart);
            return version;
        }
    }
}
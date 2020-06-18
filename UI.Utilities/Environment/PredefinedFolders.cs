using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UI.Utilities.Environment
{
    public static class PredefinedFolders
    {
        public static readonly string LocalApplicationFolder = System.IO.Path.Combine(
            System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.LocalApplicationData),
                                                                                      System.Diagnostics.Process.GetCurrentProcess().ProcessName,
                                                                                      System.Diagnostics.Process.GetCurrentProcess().MainModule.FileVersionInfo.ProductVersion);
        
        public static readonly string DynamicAssembliesFolder = "DynamicAssemblies";
        public static readonly string DynamicAssembliesPath = GetAndEnsureLocalPath(DynamicAssembliesFolder);

        public static string GetAndEnsureLocalPath(string folder)
        {
            var path = System.IO.Path.Combine(LocalApplicationFolder, folder);

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}

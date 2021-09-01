using System;
using System.IO;

namespace ShoppingWarehouse.Helpers
{
    public static class DirectoryHelper
    {
        public static void CreateExportedFilesDirectory()
        {
            string exporedFilesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exportedfiles");
            bool exists = Directory.Exists(exporedFilesDirectoryPath);
            if (!exists) Directory.CreateDirectory(exporedFilesDirectoryPath);
        }
    }
}

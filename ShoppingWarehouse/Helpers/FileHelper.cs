using System;
using System.IO;

namespace ShoppingWarehouse.Helpers
{
    public static class FileHelper
    {
        public static string GetFileExportPath(string filename)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exportedfiles", filename);
        }
    }
}

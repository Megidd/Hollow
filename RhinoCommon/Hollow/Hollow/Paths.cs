using System;
using System.IO;
using System.Reflection;

namespace Hollow
{
    internal class Paths
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        // Input object to be saved as STL.
        public static string stlIn = Path.GetTempPath() + "input.stl";
        public static string stlOut = Path.GetTempPath() + "output.stl";

        public static string logic = Path.Combine(AssemblyDirectory, "logic.exe");
    }
}

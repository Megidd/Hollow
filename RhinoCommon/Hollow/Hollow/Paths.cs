using System;
using System.IO;
using System.Reflection;

namespace Hollow
{
    internal class Paths
    {
        public Paths()
        {
            tmpDir = CreateTempSubdirectory() + Path.DirectorySeparatorChar;
        }

        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private string tmpDir;

        // https://stackoverflow.com/a/278457/3405291
        private string CreateTempSubdirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (File.Exists(tempDirectory))
            {
                return CreateTempSubdirectory();
            }
            else
            {
                Directory.CreateDirectory(tempDirectory);
                return tempDirectory;
            }
        }

        // Input object to be saved as STL.
        public string stlIn { get { return tmpDir + "input.stl"; } }
        public string stlOut { get { return tmpDir + "output.stl"; } }
        public string logic { get { return Path.Combine(AssemblyDirectory, "logic.exe"); } }
        public string logs { get { return tmpDir + "mesh-checks.txt"; } }
    }
}

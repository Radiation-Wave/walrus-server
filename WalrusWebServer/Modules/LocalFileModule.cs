using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WalrusWebServer.Modules
{
    public class LocalFileModule : IModule
    {
        public string Directory { get; private set; }

        public LocalFileModule(string directory)
        {
            Directory = directory;
        }

        public void Process(Stream output, HttpListenerContext context)
        {
            // This part could do with some changes.
            // Bashing paths like this isn't suitable for Mono.
            string localPath = Directory + '\\' + context.Request.Url.LocalPath.Replace('/', '\\');
            if (!File.Exists(localPath)) { return; }

            using (FileStream file = File.OpenRead(localPath))
            {
                file.CopyTo(output);
            }
        }

        public Dictionary<string, object> GetStatistics()
        {
            throw new NotImplementedException();
        }

        public object GetStatistics(string key)
        {
            throw new NotImplementedException();
        }
    }
}

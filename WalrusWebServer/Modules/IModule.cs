using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WalrusWebServer.Modules
{
    public interface IModule
    {
        void Process(Stream output, HttpListenerContext context);

        Dictionary<string, object> GetStatistics();

        object GetStatistics(string key);
    }
}

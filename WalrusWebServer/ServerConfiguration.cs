using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalrusWebServer.Mimes;

namespace WalrusWebServer
{
    public class ServerConfiguration
    {
        public string Address { get; private set; }
        public int Port { get; private set; }
        public string DefaultMime { get; private set; }
        public string Prefix { get { return string.Format("http://{0}:{1}/", Address, Port); } }

        public ServerConfiguration()
        {
            Address = "localhost";
            Port = 80;
            DefaultMime = MimeTypes.Get(string.Empty);
        }
        public ServerConfiguration(string address, int port, string defaultMime)
        {
            Address = address;
            Port = port;
            DefaultMime = defaultMime;
        }
    }
}

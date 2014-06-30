using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalrusWebServer;
using WalrusWebServer.Mimes;
using WalrusWebServer.Modules;

namespace WalrusWebServerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadMimeTypes();
            ServerConfiguration configuration = new ServerConfiguration();
            Server server = new Server(configuration);

            
            IModule fileStore = new LocalFileModule(@"C:\Users\Dylan\Documents\EVE\capture");
            IModule cache = new CompressedCache(fileStore);
            IModule limiter = new StreamLimiter(cache, 1024 * 64);

            server.Modules.Add(MimeTypes.Get("png"), limiter);

            Console.WriteLine("Server Running");
            while (Console.ReadLine() != "quit")
            {

            }
        }

        private static void LoadMimeTypes()
        {
            MimeTypes.Register("text/html", string.Empty, "htm", "html", "htmls", "htx");
            MimeTypes.Register("text/css", "css");
            MimeTypes.Register("image/png", "png");
            MimeTypes.Register("image/jpeg", "jpg", "jpeg");
            MimeTypes.Register("application/javascript", "js");
        }
    }
}

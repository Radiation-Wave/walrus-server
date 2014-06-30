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

            // Direct requests to local files
            string personal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            IModule fileStore = new LocalFileModule(string.Format("{0}\\walrus", personal));

            // Cache files into memory and pre-compress
            IModule cache = new CompressedCache(fileStore);

            // Restrict rate to 64 Kbytes
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

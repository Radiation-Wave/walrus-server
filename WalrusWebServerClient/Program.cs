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

			// Direct requests to locally stored files.
			string basePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			IModule contentModule = new LocalFileModule(string.Format("{0}\\walrus", basePath));

			// Cache files into memory and pre-compress, ideal for static content.
			IModule cacheModule = new CompressedCache(contentModule);

			// Restrict content to a rate of 64 Kbytes per second.
			IModule limiterModule = new StreamLimiter(cacheModule, 1024 * 64);

			ServerConfiguration configuration = new ServerConfiguration();
			Server server = new Server(configuration, cacheModule);

			// Apply the speed restriction to PNG and JPG files.
			server.Modules.Add(MimeTypes.Get("png"), limiterModule);
			server.Modules.Add(MimeTypes.Get("jpg"), limiterModule);

            Console.WriteLine("Server Running");
            while (Console.ReadLine() != "quit")
            {

            }
        }

        private static void LoadMimeTypes()
        {
			// By default a missing file extension is assumed to reference a HTML document. (string.Empty)
            MimeTypes.Register("text/html", string.Empty, "htm", "html", "htmls", "htx");
            MimeTypes.Register("text/css", "css");
            MimeTypes.Register("image/png", "png");
            MimeTypes.Register("image/jpeg", "jpg", "jpeg");
            MimeTypes.Register("application/javascript", "js");
        }
    }
}

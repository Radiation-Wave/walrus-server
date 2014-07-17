using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WalrusWebServer.Mimes;
using WalrusWebServer.Modules;

namespace WalrusWebServer
{
    public class Server
    {
        private HttpListener _listener;
        private Thread _worker;

        public ServerConfiguration Configuration { get; private set; }
        public ServerModules Modules { get; private set; }

        public Server(ServerConfiguration configuration, IModule defaultModule)
        {
            Configuration = configuration;
			Modules = new ServerModules(defaultModule);

            _listener = new HttpListener();
            _listener.Prefixes.Add(configuration.Prefix);
            _listener.Start();

            _worker = new Thread(AcceptRequests);
            _worker.Start();
        }

        private void AcceptRequests()
        {
            HttpListenerContext requestContext;
            while (true)
            {
                requestContext = _listener.GetContext();

				// Prepare the content mime types then use to locate a module.
                string contentType = GetContentType(requestContext.Request.Url);
                requestContext.Response.ContentType = contentType;

                IModule module = Modules.Get(contentType);
				if (module != null) {
					// Spool-up a mico-thread to handle the request.
					Task task = new Task(() => GenerateResponse(module, requestContext));
					task.Start();
				} else {
					// We don't know how to handle the request.
					requestContext.Response.StatusCode = 404;
					requestContext.Response.Close();
				}
            }
        }

        private void GenerateResponse(IModule module, HttpListenerContext context)
        {
			// Process the request then close the stream. All logic is delegated
			// to the supplied IModule implementer.
            module.Process(context.Response.OutputStream, context);
            context.Response.Close();
        }

        private string GetContentType(Uri url)
        {
			// Crudely extract the file extension used in the request.
            int position = url.AbsolutePath.LastIndexOf('.') + 1;
            if (position > 0)
            {
                string extension = url.AbsolutePath.Substring(position);
                string signature = MimeTypes.Get(url.AbsolutePath.Substring(position));
                if (signature != string.Empty)
                {
                    return signature;
                }
                else
                {
                    return Configuration.DefaultMime;
                }
            }
            return MimeTypes.Get(string.Empty);
        }
    }
}

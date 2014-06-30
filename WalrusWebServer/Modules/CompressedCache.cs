using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WalrusWebServer.Modules
{
    public class CompressedCache : IModule
    {
        private Dictionary<string, CompressedCacheEntry> _cacheDirectory;

        public IModule Destination { get; private set; }

        public CompressedCache(IModule destination)
        {
            _cacheDirectory = new Dictionary<string, CompressedCacheEntry>();

            Destination = destination;
        }

        public void Process(Stream output, HttpListenerContext context)
        {
            if (!_cacheDirectory.ContainsKey(context.Request.RawUrl))
            {
                _cacheDirectory.Add(context.Request.RawUrl, new CompressedCacheEntry(Destination, context));
            }

            CompressedCacheEntry entry = _cacheDirectory[context.Request.RawUrl];
            if (entry.IsDeflated && context.Request.Headers["Accept-Encoding"].Contains("deflate"))
            {
                context.Response.Headers.Add(HttpResponseHeader.ContentEncoding, "deflate");
                context.Response.ContentLength64 = entry.Deflated.LongLength;
                output.Write(entry.Deflated, 0, entry.Deflated.Length);
            }
            else
            {
                context.Response.ContentLength64 = entry.Content.LongLength;
                output.Write(entry.Content, 0, entry.Content.Length);
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

    class CompressedCacheEntry
    {
        public DateTime Created { get; private set; }
        public byte[] Content { get; private set; }
        public byte[] Deflated { get; private set; }
        public bool IsDeflated { get; private set; }

        public CompressedCacheEntry(IModule destination, HttpListenerContext context)
        {
            Created = DateTime.Now;
            using (MemoryStream memory = new MemoryStream())
            {
                // Store the raw document.
                destination.Process(memory, context);
                Content = memory.ToArray();

                // Attempt to deflate the document, store if size is reduced.
                using (MemoryStream deflateMemory = new MemoryStream())
                using (DeflateStream deflate = new DeflateStream(deflateMemory, CompressionLevel.Optimal))
                {
                    deflate.Write(Content, 0, Content.Length);
                    if (deflateMemory.Length < Content.Length)
                    {
                        Deflated = deflateMemory.ToArray();
                        IsDeflated = true;
                    }
                    else
                    {
                        IsDeflated = false;
                    }
                }
            }
        }
    }
}

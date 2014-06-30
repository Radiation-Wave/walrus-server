using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WalrusWebServer.Modules
{
    public class StreamLimiter : IModule
    {
        public int ByteRate { get; private set; }
        public IModule Destination { get; private set; }

        public StreamLimiter(IModule destination, int byteRate)
        {
            Destination = destination;
            ByteRate = byteRate;
        }

        public void Process(Stream output, HttpListenerContext context)
        {
            byte[] buffer = new byte[ByteRate];
            int bufferLength = 0;
            using (MemoryStream memory = new MemoryStream())
            {
                Destination.Process(memory, context);

                memory.Position = 0;
                while ((bufferLength = memory.Read(buffer, 0, ByteRate)) >= 0)
                {
                    output.Write(buffer, 0, bufferLength);
                    Thread.Sleep(1000);
                }
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

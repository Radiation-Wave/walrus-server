using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalrusWebServer.Mimes
{
    public class MimeTypes
    {
        private static Dictionary<string, string> _signatures = new Dictionary<string, string>();

        private MimeTypes() { }

        public static string Get(string extension)
        {
            if (_signatures.ContainsKey(extension))
            {
                return _signatures[extension];
            }
            return string.Empty;
        }

        public static void Register(string signature, params string[] extensions)
        {
            lock (_signatures)
            {
                foreach (string extension in extensions)
                {
                    _signatures.Add(extension, signature);
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalrusWebServer.Modules;

namespace WalrusWebServer
{
    public class ServerModules
    {
		private IModule _defaultModule;
        private Dictionary<string, IModule> _directory;

        public ServerModules(IModule defaultModule)
        {
			_defaultModule = defaultModule;
            _directory = new Dictionary<string, IModule>();
        }

        public IModule Get(string signature)
        {
            if (_directory.ContainsKey(signature))
            {
                return _directory[signature];
            }
			return _defaultModule;
        }

        public bool Add(string signature, IModule module)
        {
            lock (_directory)
            {
                if (!_directory.ContainsKey(signature))
                {
                    _directory.Add(signature, module);
                    return true;
                }
            }
            return false;
        }

        public bool Remove(string signature)
        {
            lock (_directory)
            {
                if (_directory.ContainsKey(signature))
                {
                    _directory.Remove(signature);
                    return true;
                }
            }
            return false;
        }
    }
}

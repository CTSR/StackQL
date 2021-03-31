using System.Collections.Generic;

namespace StackQL
{
    public class DbParams
    {
        private object _lock;
        private string _prefix;
        private int _index;
        private Dictionary<string, object> _params;
        
        public DbParams(string prefix)
        {
            _lock = new object();
            _prefix = prefix;
            _index = 0;
            _params = new Dictionary<string, object>();
        }

        public string Add(object value)
        {
            lock(_lock)
            {
                string key = _prefix + "_" + _index;
                _params.Add(key, value);
                _index += 1;
                return key;
            }
        }

        public Dictionary<string, object> DumpAll()
        {
            return _params;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinLib.Collections
{
    class DictionaryStack<K,V> :Dictionary<K,V>
    {

        public bool empty
        {
            get
            {
                return Count == 0;
            }
        }

        public KeyValuePair<K,V> pop
        {
            get
            {
                KeyValuePair<K, V> kvp = this.First();
                this.Remove(kvp.Key);
                return kvp;
            }
        }
    }
}

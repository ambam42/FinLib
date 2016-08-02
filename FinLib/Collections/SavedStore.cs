using FinLib.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FinLib
{
    public class SavedStore : Dictionary<string, string>
    {
        readonly string store;

        //private static Dictionary<string, string> storeInstance = null;
        //private static Dictionary<string, string> store
        //{
        //    get
        //    {
        //        //if (storeInstance == null)
        //        //{
        //        //    try {
        //        //        if (File.Exists(STORE))
        //        //        {
        //        //            storeInstance = File.ReadAllBytes(STORE).fromBytes().Unserialize();
        //        //        } 
        //        //    }catch(Exception e)
        //        //    {
        //        //        Log.f(e);
        //        //    }
        //        //    if(storeInstance == null)
        //        //    {
        //        //        storeInstance = new Dictionary<string, string>();
        //        //    }
        //        //}

        //        return storeInstance;
        //    }

        //}


        public SavedStore(string store) : base(File.Exists(store)?File.ReadAllBytes(store).fromBytes().Unserialize():new Dictionary<string, string>()) {
            this.store = store;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        public virtual string get(string key, string def= default(string))
        {
            return ContainsKey(key) ? this[key] : def;
        }

        public virtual V get<K, V>(K key, Func<K, string> fromKey, Func<string, V> toValue, V def = default(V)){
            try {
                return toValue(get(fromKey(key)));
            }catch(Exception e)
            {
                Log.f(e);
            }
            return def;
        }

        public virtual void set(string key, string value)
        {
            this[key] = value;
        }

        public virtual void set<K, V>(K key, Func<K, string> fromKey, V value, Func<V, string> fromValue)
        {
            set(fromKey(key), fromValue(value));
        }

        private void save()
        {
            try {
                File.Delete(store);
                File.WriteAllBytes(store, this.Serialize().toBytes());
            }catch(Exception e)
            {
                Log.f(e);
            }
        }

        void OnProcessExit(object sender, EventArgs e)
        {
            save();
        }
    }

    public class SavedObjectStore<K, V>: SavedStore
    {

        Func<K, string> fromKey;
        Func<string, K> toKey;
        Func<V, string> fromValue;
        Func<string, V> toValue;

        public SavedObjectStore(string store, Func<K, string> fromKey, Func<string, K> toKey, Func<V, string> fromValue, Func<string, V> toValue) :base(store){
            this.fromKey = fromKey;
            this.toKey = toKey;
            this.fromValue = fromValue;
            this.toValue = toValue;
        }

        public V this[K k]
        {
            get
            {
                return toValue(base[fromKey(k)]);
            }
            set
            {
                base[fromKey(k)] = fromValue(value);
            }
        }

        public V get(K key)
        {
            return this[key];
        }

        public void set(K key, V value)
        {
            this[key] = value;
        }


    }
}

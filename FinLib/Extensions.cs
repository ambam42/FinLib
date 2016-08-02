using FinLib.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Extensions
    {
        public static DateTime FromUnixTime(this string me)
        {
            long meL = default(long);
            if(long.TryParse(me, out meL))
            {
                return meL.FromUnixTime();
            }
            return default(DateTime);
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static StringBuilder Prepend<T>(this StringBuilder me, T value)
        {
            return me.Insert(0, value);
        }

        public static StringBuilder Prepend<T>(this StringBuilder me, params T[] values)
        {
            for (int i = values.Length - 1; i >= 0; i--)
            {
                me.Prepend(values[i]);
            }
            return me;
        }

        public static StringBuilder Prepend(this StringBuilder me, params object[] values)
        {
            return me.Prepend<object>(values);
        }

        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
           where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }

        public static bool IsEnumerable(this Type me)
        {
            return me.GetInterfaces().Any(
            i => i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static bool IsCompareable(this Type me)
        {
            return me.GetInterfaces().Any(
            i => i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IComparable<>));
        }

        public static bool IsEquateable(this Type me)
        {
            return me.GetInterfaces().Any(
            i => i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IEquatable<>)); 
        }

        public static String Collapse(this StringCollection me, String seperator = null, String preString= null, String postString = null, StringBuilder builder = null)
        {
            return ((IEnumerable)me).Collapse<string>(x => x, seperator, preString, postString, builder);
        }

        public static String Collapse(this String[] me, String seperator=null, String preString = null, String postString = null, StringBuilder builder = null)
        {
            int c = me.Count();
            return c <= 1? (c == 0 ? "" : me[0]):((IEnumerable)me).Collapse<string>(x => x, seperator, preString, postString, builder);
        }

        public static String Collapse(this List<String> me, String seperator = null, String preString = null, String postString = null, StringBuilder builder = null)
        {
            return ((IEnumerable)me).Collapse<string>(x => x, seperator, preString, postString, builder);
        }

        public static String Collapse<T>(this IEnumerable me, Func<T, string> transform, String seperator = ",", String preString = null, String postString = null, StringBuilder builder = null)
        {

            if (builder == null) { builder = new StringBuilder(); }
            try
            {
                long i = 0;
                foreach (T value in me)
                {
                    if (seperator != null && i > 0) builder.Append(seperator);
                    if (preString != null) { builder.Append(preString); }
                    builder.Append(transform(value));
                    if (postString != null) { builder.Append(postString); }
                    i++;
                }
            }
            catch (Exception e)
            {
                Level.Fault.log(e);
            }

            return builder.ToString();
        }

        public static bool pipeTo(this Stream me, Stream output, bool close = true, bool supressExceptions = false)
        {
            Optional<Exception> optException = Optional<Exception>.absent();
            try
            {
                byte[] buffer = new byte[32768];
                int read;
                while ((read = me.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                }
                output.Flush();
                return true;
            }
            catch (Exception e)
            {
                Log.f(e);
                if (supressExceptions)
                {
                    return false;
                }
                throw e;
            }
            finally
            {
                if (close)
                {
                    try
                    {

                        me.Close();
                    }
                    catch (Exception e) { Log.f(e); }
                    try
                    {

                        output.Close();
                    }
                    catch (Exception e) { Log.f(e); }
                }
            }
        }

        public static String Serialize(this Dictionary<String, String> me)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<String, String> entry in me)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" | ");
                }
                builder.Append(entry.Key.Replace(",", "\\,").Replace("|", "\\|")).Append(" , ").Append(entry.Value.Replace(",", "\\,").Replace("|", "\\|"));
            }
            return builder.ToString();
        }

        public static Dictionary<String, String> toDictionary(this String me, char rowSep, char kvSep)
        {
            Dictionary<String, String> dictionary = new Dictionary<String, String>();
            try
            {
                String[] kvps = me.Split(rowSep);
                for (int i = 0; i < kvps.Length; i++)
                {
                    String[] kvp = kvps[i].Split(kvSep);
                    if(kvp.Length >= 1)
                    {
                        dictionary.Add(kvp[0], kvp[1]);
                    }else if(kvps.Length > 0)
                    {
                        dictionary.Add(kvp[0], null);
                    }
                }
            }catch(Exception e)
            {
                Log.f(e);
            }
            return dictionary;
        }

        public static Dictionary<String, String> Unserialize(this String me)
        {
            Dictionary<String, String> dictionary = new Dictionary<String, String>();

            try
            {
                String[] entries = me.Split(new String[] { " | ", " , " }, StringSplitOptions.None);

                for (int i = 0; i + 1 < entries.Length; i += 2)
                {
                    String key = entries[i];
                    String value = entries[i + 1];
                    //key.Replace("\\,", ",").Replace("\\|", "|");
                    //value.Replace("\\,", ",").Replace("\\|", "|");
                    dictionary.Add(key.Replace("\\,", ",").Replace("\\|", "|"), value.Replace("\\,", ",").Replace("\\|", "|"));
                }
            }
            catch (Exception e)
            {
                Log.f(e);
            }

            return dictionary;
        }

        public static bool IsString(this object me)
        {
            return me is string || me is String;
        }

        public static bool IsBoolean(this object me)
        {
            return me is bool || me is Boolean;
        }

        public static bool IsException(this object me)
        {
            return me is Exception;
        }

        public static bool IsNumeric(this object me)
        {
            return me is sbyte
                    || me is byte
                    || me is short
                    || me is ushort
                    || me is int
                    || me is uint
                    || me is long
                    || me is ulong
                    || me is float
                    || me is double
                    || me is decimal;
        }

        public static bool IsDateTimeData(this object me)
        {
            return me is DateTime || me is TimeSpan || me is DayOfWeek;
        }

        public static String objectToString(this object me, object parent=null)
        {
            try { 
                if (me == null) { return "null"; }
                if (me.IsString() ) { return (String)me; }
                if (me.IsException() || me.IsDateTimeData() || me.IsNumeric() || me.IsBoolean()) { return me.ToString(); }

                StringBuilder builder = new StringBuilder();
           
                if (me.GetType().IsEnumerable())
                {
                    IEnumerator enumerator = ((IEnumerable)me).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        builder.Append(enumerator.Current.objectToString(enumerator));
                    }
                }
                else
                {
                    PropertyInfo[] props = me.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (Equality.Equals(parent, me))
                        {
                            builder.Append("{").Append(prop.Name).Append("|").Append(me.ToString()).Append("}");
                        } else if(prop.CanRead && !prop.GetMethod.IsStatic){
                            object value = prop.GetValue(me, null);
                            builder.Append("{").Append(prop.Name).Append("|").Append(value.objectToString(value)).Append("}");
                        }
                    }
                }

                return (builder.Length > 0) ? builder.ToString() : me.ToString();
            }
            catch (Exception e)
            {
                Log.f(e);
                return "error converting object to string";
            }
        }

        public static byte[] toBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string fromBytes(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static Dictionary<K, V> ToDictionary<S, K ,V>(this IEnumerable<S> me, Func<S, KeyValuePair<K, V>> func)
        {
            Dictionary<K, V> dictionary = new Dictionary<K, V>();

            foreach(S s in me)
            {
                KeyValuePair<K, V> kvp = func(s);
                dictionary.Add(kvp.Key, kvp.Value);
            }

            return dictionary;
        }

        public static List<V> ToList<S, V>(this IEnumerable<S> me, Func<S, V> func)
        {
            List<V> list = new List<V>();
            
            foreach(S s in me)
            {
                list.Add(func(s));
            }

            return list;
        }

        public static Dictionary<K,V> merge<K,V>(this Dictionary<K, V> me, Dictionary<K, V> you)
        {
            foreach(KeyValuePair<K,V> kvp in you)
            {
                me[kvp.Key] = kvp.Value;
            }

            return me;
        }
    }
}

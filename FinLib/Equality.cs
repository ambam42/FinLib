using FinLib.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Equality
    {
        public static bool equals<T>(T a, T b)
        {
            return equals(a, b, false);   
        }

        public static bool equals<T>(T a, T b, bool verbose)
        {
            try
            {
                Type aT = a.GetType();
                Type bT = b.GetType();
                if(aT != bT)
                {
                    if(verbose) Log.v("A AND B Tpes NOT EQUAL!!!");
                    return false;
                }

                Type type = aT;

                if(verbose) Log.v("Comparing Type:", typeof(T), "|AT:", a.GetType(), "|BT:", b.GetType());

                if (type.IsCompareable())
                {
                    if(verbose) Log.v("Type is compareable");
                    if ((((IComparable)a).CompareTo((IComparable)b)) != 0)
                    {
                        if(verbose) Log.v("Failed Comparable test");
                        return false;
                    }
                    else
                    {
                        if(verbose) Log.v("Passed Comparable test");
                        return true;
                    }
                }

                if (type.IsEnumerable())
                {
                    if(verbose) Log.v("Comparing Enumerables");
                    if(!equals((IEnumerable) a, (IEnumerable)b)){
                        if(verbose) Log.v("Enumerables Different");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                FieldInfo[] fi = type.GetFields().Where(x => !x.IsPrivate).ToArray();
                PropertyInfo[] pi = type.GetProperties();

                if (verbose) Log.v(type, " Field Count:", fi.Count());
                if (verbose) Log.v(type," Property Count:", pi.Count());

                if (fi.Count() > 0 || pi.Count() > 0)
                {
                    if(fi.Count() > 0)
                    {
                        if (verbose) Log.v("Comparing Fields");
                        foreach (FieldInfo field in fi)
                        {
                            if (verbose) Log.v("Field:", field.Name);
                                var aV = field.GetValue(a);
                                if (verbose) Log.v("A Value:", aV);
                                var bV = field.GetValue(b);
                                if (verbose) Log.v("B Value:", bV);
                                if (aV != null && bV != null &&
                                !equals(aV, bV))
                                {
                                    if (verbose) Log.v("AB Failed equality comparison");
                                    return false;
                                }
                            //}
                            //else
                            //{
                            //    if (verbose) Log.v("Field:", field.Name, " IsPrivate, cannot compare.");
                            //}
                        }
                    }
                    if (pi.Count() > 0)
                    {
                        if (verbose) Log.v("Comparing Properties");
                        foreach (PropertyInfo prop in pi)
                        {
                            
                            if (verbose) Log.v("Property:", prop.Name);
                            var aV = prop.GetValue(a, null);
                            if (verbose) Log.v("A Value:", aV);
                            var bV = prop.GetValue(b, null);
                            if (verbose) Log.v("B Value:", bV);

                            if (aV != null && bV != null &&
                            !equals(aV, bV))
                            {
                                if (verbose) Log.v("AB Failed equality comparison");
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if(verbose) Log.v("No comparable values found, using basic comparsion");
                    if(verbose) Log.v("A:", a);
                    if(verbose) Log.v("B:", b);

                    if (!EqualityComparer<T>.Default.Equals(a, b))
                    {
                            if(verbose) Log.v("Basic Comparsion Failed");
                            return false;
                    }
                }
                
                if(verbose) Log.v("No differences detected");
                return true;
            }
            catch (Exception e)
            {
                Log.f(e);
            }

            return false;
        }

        public static bool equals(IEnumerable a, IEnumerable b)
        {
            try
            {
                IEnumerator aE = a.GetEnumerator();
                IEnumerator bE = b.GetEnumerator();

                bool aM = aE.MoveNext();
                bool bM = bE.MoveNext();

                while (aM && bM)
                {
                    if (!equals(aE.Current, bE.Current))
                    {
                        return false;
                    }

                    aM = aE.MoveNext();
                    bM = bE.MoveNext();
                }

                return (!(aM | bM));
            }
            catch (Exception e)
            {
                Log.f(e);
            }

            return false;
        }
    }
}

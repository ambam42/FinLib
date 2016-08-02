using FinLib.Collections;
using FinLib.Logger;
using FinLib.Reporting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Reports
    {
        static DictionaryStack<string, IReport> reports= new DictionaryStack<string, IReport>();

        
        static Reports()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        public static void register(String name, IReport report)
        {
            try { 
                reports.Add(name, report);
            }
            catch (Exception e)
            {
                Log.f(e);
            }
        }

        public static void add<R>(string name, object o) where R : IReport, new()
        {
            try {
                if (!reports.ContainsKey(name))
                {
                    reports[name] = new R();
                }
                reports[name].report(o);
            }
            catch (Exception e)
            {
                Log.f(e);
            }
        }

        public static void add<R, T>(string name, T t) where R : Report<T>, new()
        {
            try { 
                add<R>(name, t);
            }
            catch (Exception e)
            {
                Log.f(e);
            }
        }

        public static void add(string name, string value)
        {
            try { 
                add<Report<string>, string>(name, value);
            }
            catch (Exception e)
            {
                Log.f(e);
            }
        }

        public static void add(string name, params string[] value)
        {
            try {
                add<Report<string>, string>(name, value.Collapse(null));
            }catch(Exception e)
            {
                Log.f(e);
            }
        }

        public static void send()
        {
            try
            {
                if (reports != null)
                {
                    while (!reports.empty) {
                        KeyValuePair<string, IReport> report = reports.pop;
                        
                        report.Value.send(report.Key);
                    }
                }
            }
            catch (Exception e)
            {
                Log.f(e);
            }
        }
        static void OnProcessExit(object sender, EventArgs e)
        {
            send();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinLib.Logger
{
    class Config
    {
        static List<String> logs = null;
        static List<String> consoleLogs = null;

        static Config()
        {
            if(ConfigurationManager.AppSettings.HasKeys() && ConfigurationManager.AppSettings["logs"] != null)
            {
                foreach(String log in ConfigurationManager.AppSettings["logs"].Split(','))
                {
                    registerLog(log);
                }
            }
            else
            {
                foreach (Level level in Enum.GetValues(typeof(Level)))
                {
                    registerLog(level);
                    
                }
            }
            if (ConfigurationManager.AppSettings.HasKeys() && ConfigurationManager.AppSettings["consoleLogs"]!=null)
            {
                foreach (String log in ConfigurationManager.AppSettings["consoleLogs"].Split(','))
                {
                    registerConsoleLog(log);
                }
            }
            else
            {
                foreach (Level level in Enum.GetValues(typeof(Level)))
                {
                    registerConsoleLog(level);
                }
            }
        }
        public static void unregisterLevel(Level level)
        {
            unregisterLog(level);
            unregisterConsoleLog(level);
        }
        public static void registerLevel(Level level)
        {
            registerLog(level);
            registerConsoleLog(level);
        }
        public static void registerLog(IEnumerable<String> logs)
        {
            foreach(String log in logs)
            {
                registerLog(log);
            }
        }
        public static void registerLog(Level level)
        {
            registerLog(level.ToString());
        }

        public static void registerLog(String log)
        {
            if(logs == null) { logs = new List<String>(); }
            logs.Add(log.ToLower());
        }
        public static void unregisterLog(Level level)
        {
            unregisterLog(level.ToString());
        }

        public static void unregisterLog(String log)
        {
            if (logs == null) { return; }
            logs.Remove(log.ToLower());
        }
        public static bool logEnabled(String log)
        {
            if(logs == null) { return false;}
            return logs.Contains(log.ToLower());
        }

        public static void registerConsoleLog(IEnumerable<String> logs)
        {
            foreach (String log in logs)
            {
                registerConsoleLog(log);
            }
        }

        public static void registerConsoleLog(Level level)
        {
            registerConsoleLog(level.ToString());
        }

        public static void registerConsoleLog(String log)
        {
            if (consoleLogs == null) { consoleLogs = new List<String>(); }
            consoleLogs.Add(log.ToLower());
        }
        public static void unregisterConsoleLog(Level level)
        {
            unregisterConsoleLog(level.ToString());
        }

        public static void unregisterConsoleLog(String log)
        {
            if (consoleLogs == null) { return; }
            consoleLogs.Remove(log.ToLower());
        }
        public static bool consoleLogEnabled(String log)
        {
            if(consoleLogs == null) {return false;}
            return consoleLogs.Contains(log.ToLower());   
        }
    }
}

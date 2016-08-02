using System;
using System.Collections.Generic;

namespace FinLib.Logger
{

    public class Log
    {
        private static Level LEVEL = Level.Debug;
        private static List<Type> TYPES = new List<Type>();
        
        static Log()
        {
            registerDefaultType(Types.console);
        }

        public static void registerDefaultType(Type type)
        {
            TYPES.Add(type);
        }

        public static void setDefaultLevel(Level level)
        {
            LEVEL = level;
        }

        public static void log(params Object[] logs)
        {
            log(LEVEL, logs);
        }
        public static void log(Type type, params Object[] logs)
        {
            log(type, logs);
        }
        public static void log(IEnumerable<Type> types, params Object[] logs)
        {
            log(types, LEVEL, logs);
        }

        #region //level convience methods
        public static void d(params Object[] logs)
        {
            log(Level.Debug, logs);
        }
        public static void i(params Object[] logs)
        {
            log(Level.Info, logs);
        }
        public static void v(params Object[] logs)
        {
            log(Level.Verbose, logs);
        }
        public static void w(params Object[] logs)
        {
            log(Level.Warn, logs);
        }
        public static void f(params Object[] logs)
        {
            log(Level.Fault, logs);
        }
        #endregion

        public static void log(Level level, params Object[] logs)
        {
            log(TYPES, level, logs);
        }

        public static void log(Type type, Level level, params Object[] logs)
        {
            if(level.enabled()) type.log(level, logs);
        }

        public static void log(IEnumerable<Type> types, Level level, params Object[] logs)
        {
            foreach(Type type in types){
                log(type, level, logs);
            }
        }
    }

    public enum Level
    {
        Debug,
        Info,
        Verbose,
        Warn,
        Fault,
        WTF
    }

    public static class Extensions
    {
        public static void log(this Level level, params Object[] logs)
        {
            Log.log(level, logs);
        }
        public static void log(this Level level, Type type, params Object[] logs)
        {
            Log.log(type, level, logs);
        }
        public static bool enabled(this Level level)
        {
            return Config.logEnabled(level.ToString());
        }
        public static bool consoleEnabled(this Level level)
        {
            return Config.consoleLogEnabled(level.ToString());
        }
    }
}

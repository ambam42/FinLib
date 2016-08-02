using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FinLib.Logger
{
    public abstract class Type : IType
    {
        public abstract void log(Level level, StringBuilder log);
        public virtual void log(Level level, params Object[] logs)
        {
            try
            {
                StringBuilder logBuilder = new StringBuilder();

                foreach (Object log in logs)
                {
                    logBuilder.Append(log.objectToString());
                }

                log(level, logBuilder);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error:" + e.Message);
            }
        }

        public virtual void log(Level level, char sep, params Object[] logs)
        {
            try
            {
                StringBuilder logBuilder = new StringBuilder();

                foreach (Object log in logs)
                {
                    if(logBuilder.Length > 0)
                    {
                        logBuilder.Append(sep);
                    }

                    logBuilder.Append(log.objectToString());
                }

                log(level, logBuilder);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error:" + e.Message);
            }
        }
    }
}

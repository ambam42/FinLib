using log4net;
using System;
//using log4net;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace FinLib.Logger
{
    
    public static class Types
    {
        //static Types(){
        //    log4net.Config.XmlConfigurator.Configure();
        //}

        static Type CONSOLE = null;
        public static Type console { get { if (CONSOLE == null) { CONSOLE = new Console(); } return CONSOLE; } }

        //static Type LOG4NET = null;
        //public static Type log4Net { get { if (LOG4NET == null) { log4net.Config.XmlConfigurator.Configure(); LOG4NET = new Log4Net(); }  return LOG4NET; } }

        //static Type HIST4NET = null;
        //public static Type hist4Net { get { if (HIST4NET == null) { log4net.Config.XmlConfigurator.Configure(); HIST4NET = new Log4Net("HistoryLog"); } return HIST4NET; } }
        
        public class Log4Net : Type 
        {
            
            string loggerName;
            //public Log4Net() : this("SystemLog")
            //{ }

            public Log4Net(String loggerName)
            {
                this.loggerName = loggerName;
                
            }
            override
            public void log(Level level, StringBuilder log)
            {
                try
                {
                    string logStatement = log.Replace(Environment.NewLine, ",").ToString();
                    ILog logger = LogManager.GetLogger(loggerName);
                    switch (level)
                    {
                        case Level.Warn:
                            logger.Warn(logStatement);
                            break;

                        case Level.Verbose:
                        case Level.Debug:
                            logger.Debug(logStatement);
                            break;

                        case Level.Fault:
                            logger.Error(logStatement);
                            break;

                        case Level.WTF:
                            logger.Fatal(logStatement);
                            break;

                        case Level.Info:
                        default:
                            logger.Info(logStatement);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.log(Types.CONSOLE, Level.Fault, e);
                }
            }
        }

        public class Console : Type
        {
            override
            public void log(Level level, params Object[] logs)
            {
                if (level.consoleEnabled()) base.log(level, logs);
            }
            override
            public void log(Level level, StringBuilder log)
            {
                log.Prepend(DateTime.Now, ' ', level, ' ');
                System.Console.WriteLine(log.ToString());
            }
        }
    }
}

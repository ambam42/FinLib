using FinLib.Logger;
using System.Net.Mail;
using System.Configuration;

namespace System
{
    public class Mail
    {
        static String from = null;
        static String to = null;

        static Mail()
        {
            try {
                if (ConfigurationManager.AppSettings.Count > 0)
                {
                    Mail.from = ConfigurationManager.AppSettings["from"];
                    Mail.to = ConfigurationManager.AppSettings["to"];
                }
            }catch(Exception e)
            {
                Log.f(e);
            }
        }

        static SmtpClient client = null;
        static SmtpClient mailClient
        {
            get
            {
                if (client == null)
                {
                    client = new SmtpClient();
                }
                return client;
            }
        }

        public static void Send(string subject, string body, bool isHtml = false)
        {
            if(from == null || to == null)
            {
                Log.w("Default From and To values not set in App.config appSettings");
            }

            Send(from, to, subject, body, isHtml);
        }

        public static void Send(string from, string to, string subject, string body, bool isHtml = false)
        {
            MailMessage message = new MailMessage(from, to, subject, body);
            message.IsBodyHtml = isHtml;
            Send(message);
        }


        public static void Send(MailMessage message)
        {
            try
            {
                mailClient.Send(message);
            }
            catch(Exception e)
            {
                Log.f(e);
            }
        }
    }
}

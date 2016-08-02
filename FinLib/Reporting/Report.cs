using FinLib.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinLib.Reporting
{
    public class Report<T> : IReport
    {
        protected List<T> reports = new List<T>();
        Func<T, string> func;

        public bool isHtml
        {
            get
            {
                return true;
            }
        }

        public Report():this(x=>x.objectToString()){}

        public Report(Func<T, string> func){
            this.func = func;
        }

        public virtual void report(T t)
        {
            reports.Add(t);
            Log.v(func(t));
        }

        public override string ToString()
        {
            return reports.Collapse<T>(func, "<br>");
        }

        public void report(object o)
        {
            report((T)o);
        }

        virtual
        public bool send(string key)
        {
            Mail.Send(new StringBuilder(key).Append(": ").Append(DateTime.Now).ToString(), this.ToString(), this.isHtml);
            return true;
        }
    }
}

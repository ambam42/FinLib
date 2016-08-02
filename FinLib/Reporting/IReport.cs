using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinLib.Reporting
{
    public interface IReport
    {
        bool isHtml { get; }
        void report(object o);
        string ToString();
        bool send(string key);
    }
}

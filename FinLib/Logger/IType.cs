using System;
using System.Text;

namespace FinLib.Logger
{
    public interface IType
    {
        void log(Level level, params Object[] logs);
        void log(Level level, StringBuilder log);
    }
}

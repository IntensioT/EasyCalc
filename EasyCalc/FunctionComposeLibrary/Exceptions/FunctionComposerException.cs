using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCalc.Exceptions
{
    internal class FunctionComposerException : Exception
    {
        internal FunctionComposerException() { }
        internal FunctionComposerException(string message) 
            : base(message) { }
        internal FunctionComposerException(string message, Exception e) 
            : base(message, e) { }
    }
}

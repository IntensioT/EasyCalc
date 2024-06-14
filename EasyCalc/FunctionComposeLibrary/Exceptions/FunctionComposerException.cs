using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionComposeLibrary.Exceptions
{
    public class FunctionComposerException : Exception
    {
        public FunctionComposerException() { }
        public FunctionComposerException(string message) 
            : base(message) { }
        public FunctionComposerException(string message, Exception e) 
            : base(message, e) { }
    }
}

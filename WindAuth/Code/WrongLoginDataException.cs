using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindAuth.Code
{
    class WrongLoginDataException : Exception
    {
        public WrongLoginDataException(string message)
            : base(message)
        {

        }
    }
}

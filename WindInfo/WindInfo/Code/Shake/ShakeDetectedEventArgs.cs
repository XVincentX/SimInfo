using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindInfo
{
    public class ShakeDetectedEventArgs : EventArgs
    {

        private readonly bool _r;
        public ShakeDetectedEventArgs(bool Retrieve)
        {
            _r = Retrieve;
        }


        public bool Retrieve { get { return _r; } }
    }
}

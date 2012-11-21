using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib
{
    public class VMWareToolsNotRunningException : VMWareException
    {
        public VMWareToolsNotRunningException(ulong code)
            : base(code)
        {
        }
    }
}

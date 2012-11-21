using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib
{
    public class VMWareInteractiveSessionNotPresentException : VMWareException
    {
        public VMWareInteractiveSessionNotPresentException(ulong code)
            : base(code)
        {
        }
    }
}

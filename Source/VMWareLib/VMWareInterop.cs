using System;
using System.Collections.Generic;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// Helper functions for VMWare interop.
    /// </summary>
    public abstract class VMWareInterop
    {
        /// <summary>
        /// VIX handler.
        /// </summary>
        public static VixLib _vix = new VixLib();

        /// <summary>
        /// Checks whether an error indicates failure.
        /// </summary>
        /// <param name="errCode">error code</param>
        public static void Check(ulong errCode)
        {
            if (_vix.ErrorIndicatesFailure(errCode))
            {
                throw new VMWareException(errCode);
            }
        }
    }
}

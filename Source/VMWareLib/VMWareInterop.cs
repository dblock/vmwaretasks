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
        /// VixCOM instance handler.
        /// </summary>
        public static VixLib Instance = new VixLib();

        /// <summary>
        /// Default timeouts for VMWare operations.
        /// </summary>
        public static VMWareTimeouts Timeouts = new VMWareTimeouts();

        /// <summary>
        /// Checks whether an error indicates failure and throws an exception in that case.
        /// </summary>
        /// <param name="errCode">error code</param>
        public static void Check(ulong errCode)
        {
            if (Instance.ErrorIndicatesFailure(errCode))
            {
                throw new VMWareException(errCode);
            }
        }
    }
}

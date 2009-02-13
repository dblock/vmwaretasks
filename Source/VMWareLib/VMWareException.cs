using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VMWare operational exception.
    /// </summary>
    public class VMWareException : Exception
    {
        private ulong _errorCode = 0;

        /// <summary>
        /// A VMWare error code.
        /// </summary>
        public ulong ErrorCode
        {
            get 
            { 
                return _errorCode; 
            }
        }

        public VMWareException(ulong code)
            : this(code, VMWareInterop.Vix.GetErrorText(code, "en-US"))
        {
        }

        public VMWareException(ulong code, string message)
            : base(message)
        {
            _errorCode = code;
        }
    }
}

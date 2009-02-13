using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VMWare exception. Every VMWare operational failure is translated into 
    /// a <see ref="Vestris.VMWareLib.VMWareException" />.
    /// </summary>
    public class VMWareException : Exception
    {
        private ulong _errorCode = 0;

        /// <summary>
        /// The original VMWare error code.
        /// </summary>
        public ulong ErrorCode
        {
            get 
            { 
                return _errorCode; 
            }
        }

        /// <summary>
        /// A VMWare exception with default error text in English-US.
        /// </summary>
        /// <param name="code">VMWare VixCOM.Constants error code</param>
        public VMWareException(ulong code)
            : this(code, VMWareInterop.Instance.GetErrorText(code, "en-US"))
        {
        }

        /// <summary>
        /// A VMWare exception.
        /// </summary>
        /// <param name="code">VMWare VixCOM.Constants error code</param>
        /// <param name="message">error description</param>
        public VMWareException(ulong code, string message)
            : base(message)
        {
            _errorCode = code;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Interop.VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VMWare exception. Every VMWare operational failure is translated into 
    /// a <see cref="Vestris.VMWareLib.VMWareException" />.
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
        /// Initializes a new instance of the <see cref="VMWareException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public VMWareException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMWareException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public VMWareException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// A VMWare exception with default error text in English-US.
        /// </summary>
        /// <param name="code">VMWare VixCOM.Constants error code.</param>
        public VMWareException(ulong code)
            : this(code, new VixLib().GetErrorText(code, "en-US"))
        {
        }

        /// <summary>
        /// A VMWare exception.
        /// </summary>
        /// <param name="code">VMWare VixCOM.Constants error code.</param>
        /// <param name="message">Error description.</param>
        public VMWareException(ulong code, string message)
            : base(message)
        {
            _errorCode = code;
        }
    }
}

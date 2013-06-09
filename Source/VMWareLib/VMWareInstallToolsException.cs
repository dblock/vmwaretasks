using System;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// Failed to install guest tools exception
    /// </summary>
    public class VMWareInstallToolsException : VMWareException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VMWareInstallToolsException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public VMWareInstallToolsException(Exception innerException)
            : base("Failed to install tools", innerException)
        {
        }
    }
}

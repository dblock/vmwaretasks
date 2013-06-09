using System;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// Failed to reset virtual machine exception
    /// </summary>
    public class VMWareFailedToResetVirtualMachineException : VMWareException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VMWareFailedToResetVirtualMachineException"/> class.
        /// </summary>
        /// <param name="resetOptions">The reset options.</param>
        /// <param name="innerException">The inner exception.</param>
        public VMWareFailedToResetVirtualMachineException(int resetOptions, Exception innerException)
            : base(string.Format("Failed to reset virtual machine: resetOptions={0}", resetOptions), innerException)
        {
        }
    }
}

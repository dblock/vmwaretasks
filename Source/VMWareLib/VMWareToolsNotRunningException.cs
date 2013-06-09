namespace Vestris.VMWareLib
{
    /// <summary>
    /// The guest tools are not running exception
    /// </summary>
    public class VMWareToolsNotRunningException : VMWareException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VMWareToolsNotRunningException"/> class.
        /// </summary>
        /// <param name="code">VMWare VixCOM.Constants error code.</param>
        public VMWareToolsNotRunningException(ulong code)
            : base(code)
        {
        }
    }
}

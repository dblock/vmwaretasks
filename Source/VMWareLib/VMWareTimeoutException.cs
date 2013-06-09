namespace Vestris.VMWareLib
{
    /// <summary>
    /// A timeout exception
    /// </summary>
    public class VMWareTimeoutException : VMWareException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VMWareTimeoutException"/> class.
        /// </summary>
        /// <param name="timeoutInMilliseconds">The timeout in milliseconds.</param>
        public VMWareTimeoutException(int timeoutInMilliseconds)
            : base(string.Format("The operation has timed out after {0} milliseconds.", timeoutInMilliseconds))
        {
        }
    }
}

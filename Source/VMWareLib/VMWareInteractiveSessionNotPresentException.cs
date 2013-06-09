namespace Vestris.VMWareLib
{
    /// <summary>
    /// The interactive session is not present exception
    /// </summary>
    public class VMWareInteractiveSessionNotPresentException : VMWareException 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VMWareInteractiveSessionNotPresentException"/> class.
        /// </summary>
        /// <param name="code">VMWare VixCOM.Constants error code.</param>
        public VMWareInteractiveSessionNotPresentException(ulong code)
            : base(code)
        {
        }
    }
}

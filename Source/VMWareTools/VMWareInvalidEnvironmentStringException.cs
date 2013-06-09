namespace Vestris.VMWareLib.Tools
{
    /// <summary>
    /// Invalid environment string exception
    /// </summary>
    public class VMWareInvalidEnvironmentStringException : VMWareException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VMWareInvalidEnvironmentStringException"/> class.
        /// </summary>
        /// <param name="environmentString">The environment string.</param>
        public VMWareInvalidEnvironmentStringException(string environmentString)
            : base(string.Format("Invalid environment string: \"{0}\"", environmentString))
        {
        }
    }
}

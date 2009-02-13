using System;
using System.Collections.Generic;
using System.Text;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A wrapper for a VIX handle. Most VIX interfaces are actually handles.
    /// </summary>
    public class VMWareVixHandle
    {
        IVixHandle _handle = null;

        public VMWareVixHandle(IVixHandle handle)
        {
            _handle = handle;
        }

        /// <summary>
        /// Get an array of properties.
        /// </summary>
        /// <param name="properties">properties to fetch</param>
        /// <returns>an array of property values</returns>
        public object[] GetProperties(object[] properties)
        {
            object result = null;
            VMWareInterop.Check(_handle.GetProperties(properties, ref result));
            return (object[]) result;
        }
    }
}

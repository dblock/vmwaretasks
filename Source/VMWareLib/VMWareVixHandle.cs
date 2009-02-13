using System;
using System.Collections.Generic;
using System.Text;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A wrapper for a VIX handle. Most VIX objects returned also implement IVixHandle.
    /// </summary>
    public class VMWareVixHandle<T>
    {
        protected T _handle = default(T);
        
        protected IVixHandle _vixhandle
        {
            get
            {
                return (IVixHandle) _handle;
            }
        }

        public VMWareVixHandle()
        {

        }

        public VMWareVixHandle(T handle)
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
            VMWareInterop.Check(_vixhandle.GetProperties(properties, ref result));
            return (object[]) result;
        }

        /// <summary>
        /// Return the value of a single property.
        /// </summary>
        /// <typeparam name="T">property value type</typeparam>
        /// <param name="propertyId">property id</param>
        /// <returns>the value of a single property of type T</returns>
        public R GetProperty<R>(int propertyId)
        {
            object[] properties = { propertyId };
            return (R) GetProperties(properties)[0];
        }
    }
}

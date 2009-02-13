using System;
using System.Collections.Generic;
using System.Threading;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VixCOM job.
    /// Implements synchronous execution of VixCOM tasks.
    /// </summary>
    public class VMWareJob : VMWareVixHandle<IJob>
    {
        private VMWareJobCallback _callback;

        /// <summary>
        /// A VMWare job created with a job completion callback.
        /// </summary>
        /// <param name="job">an instance of IJob</param>
        /// <param name="callback">job completion callback</param>
        public VMWareJob(IJob job, VMWareJobCallback callback)
            : base(job)
        {
            _callback = callback;
        }

        /// <summary>
        /// Wait for the job to complete, timeout.
        /// </summary>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void Wait(int timeoutInSeconds)
        {
            _callback.WaitForCompletion(timeoutInSeconds * 1000);
            VMWareInterop.Check(_handle.WaitWithoutResults());
        }

        /// <summary>
        /// Wait for the job to complete, return a result.
        /// </summary>
        public T Wait<T>(object[] properties, int timeoutInSeconds)
        {
            _callback.WaitForCompletion(timeoutInSeconds * 1000);
            return (T)Wait<T>(properties);
        }

        /// <summary>
        /// Wait for the job to complete and enumerate results.
        /// </summary>
        /// <param name="properties">properties to yield</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public IEnumerable<object[]> YieldWait(object[] properties, int timeoutInSeconds)
        {
            _callback.WaitForCompletion(timeoutInSeconds * 1000);
            for (int i = 0; i < GetNumProperties((int)properties[0]); i++)
            {
                yield return GetNthProperties<object[]>(i, properties);
            }
        }

        /// <summary>
        /// Wait for the job to complete, return a result.
        /// </summary>
        /// <param name="properties">properties to yield</param>
        /// <param name="index">property index to yield</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        /// <typeparam name="T">type of the property to return</typeparam>
        public T Wait<T>(object[] properties, int index, int timeoutInSeconds)
        {
            _callback.WaitForCompletion(timeoutInSeconds * 1000);
            return (T)Wait<object[]>(properties)[index];
        }

        /// <summary>
        /// Wait for the job to complete, return a single result.
        /// </summary>
        /// <param name="propertyId">property id</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        /// <typeparam name="T">type of property to return</typeparam>
        public T Wait<T>(int propertyId, int timeoutInSeconds)
        {
            object[] properties = { propertyId };
            return Wait<T>(properties, 0, timeoutInSeconds);
        }

        /// <summary>
        /// Wait for the job to complete, return a result.
        /// </summary>
        /// <param name="properties">properties to return</param>
        /// <typeparam name="T">type of results</typeparam>
        private T Wait<T>(object[] properties)
        {
            object result = null;
            VMWareInterop.Check(_handle.Wait(properties, ref result));
            return (T) result;
        }

        /// <summary>
        /// Get n-th properties.
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <param name="index">property index</param>
        /// <param name="properties">property objects</param>
        /// <returns>N'th properties.</returns>
        public T GetNthProperties<T>(int index, object[] properties)
        {
            object result = null;
            VMWareInterop.Check(_handle.GetNthProperties(index, properties, ref result));
            return (T)result;
        }

        /// <summary>
        /// Get the number of property values returned by the job.
        /// </summary>
        public int GetNumProperties(int property)
        {
            return _handle.GetNumProperties(property);
        }
    }
}

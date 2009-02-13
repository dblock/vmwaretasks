using System;
using System.Collections.Generic;
using System.Threading;
using VixCOM;

namespace Vestris.VMWareLib
{
    public class VMWareJob
    {
        /// <summary>
        /// Job wrapped by this instance of VMWareJob.
        /// </summary>
        private IJob _job = null;

        public VMWareJob(IJob job)
        {
            _job = job;
        }

        /// <summary>
        /// Wait for the job to complete.
        /// </summary>
        public void Wait()
        {
            VMWareInterop.Check(_job.WaitWithoutResults());
        }

        /// <summary>
        /// Wait for the job to complete, timeout.
        /// </summary>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void Wait(int timeoutInSeconds)
        {
            InternalWait(timeoutInSeconds);
            Wait();
        }

        /// <summary>
        /// Wait for the job to complete, return a result.
        /// </summary>
        public T Wait<T>(object[] properties, int timeoutInSeconds)
        {
            InternalWait(timeoutInSeconds);
            return (T) Wait<T>(properties);
        }

        /// <summary>
        /// Wait for the job to complete, return a result.
        /// </summary>
        public T Wait<T>(object[] properties, int index, int timeoutInSeconds)
        {
            InternalWait(timeoutInSeconds);
            return (T) Wait<object[]>(properties)[index];
        }

        /// <summary>
        /// Wait for the job to complete, return a result.
        /// </summary>
        public T Wait<T>(object[] properties)
        {
            object result = null;
            VMWareInterop.Check(_job.Wait(properties, ref result));
            return (T) result;
        }

        /// <summary>
        /// Get n-th properties.
        /// </summary>
        public T GetNthProperties<T>(int index, object[] properties)
        {
            object result = null;
            VMWareInterop.Check(_job.GetNthProperties(index, properties, ref result));
            return (T) result;
        }

        /// <summary>
        /// Get the number of property values returned by the job.
        /// </summary>
        public int GetNumProperties(int property)
        {
            return _job.GetNumProperties(Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME);
        }
        
        /// <summary>
        /// Wait for the job to complete with an active timeout.
        /// </summary>
        private void InternalWait(int timeoutInSeconds)
        {
            if (timeoutInSeconds == 0)
            {
                throw new ArgumentOutOfRangeException("timeoutInSeconds");
            }

            // active wait for the job to finish
            bool isComplete = false;
            while (!isComplete && timeoutInSeconds > 0)
            {
                VMWareInterop.Check(_job.CheckCompletion(out isComplete));
                if (isComplete) break;
                Thread.Sleep(1000);
                timeoutInSeconds--;
            }

            if (timeoutInSeconds == 0)
            {
                throw new TimeoutException();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A job completion callback, used with <see ref="Vestris.VMWareLib.VMWareJob" />.
    /// </summary>
    public class VMWareJobCallback : ICallback
    {
        #region ICallback Members

        private EventWaitHandle _jobCompleted = new EventWaitHandle(false, EventResetMode.ManualReset);

        /// <summary>
        /// Handle a Vix event.
        /// Currently handles the job completion event, call WaitForCompletion to block with a timeout.
        /// </summary>
        /// <param name="job">an instance that implements IJob</param>
        /// <param name="eventType">event type</param>
        /// <param name="moreEventInfo">additional event info</param>
        public void OnVixEvent(IJob job, int eventType, IVixHandle moreEventInfo)
        {
            switch (eventType)
            {
                case VixCOM.Constants.VIX_EVENTTYPE_JOB_COMPLETED:
                    _jobCompleted.Set();
                    break;
            }
        }

        /// <summary>
        /// Wait for completion of the job with a timeout.
        /// </summary>
        /// <param name="timeoutInMilliseconds">timeout in milliseconds</param>
        /// <returns>True if job completed, false if timeout expired.</returns>
        public bool TryWaitForCompletion(int timeoutInMilliseconds)
        {
            return _jobCompleted.WaitOne(timeoutInMilliseconds, false);
        }

        /// <summary>
        /// Wait for completion of the job with a timeout.
        /// A <see ref="System.TimeoutException" /> occurs if the job hasn't completed within the timeout specified.
        /// </summary>
        /// <param name="timeoutInMilliseconds">timeout in milliseconds</param>
        public void WaitForCompletion(int timeoutInMilliseconds)
        {
            if (!TryWaitForCompletion(timeoutInMilliseconds))
            {
                throw new TimeoutException();
            }
        }

        #endregion
    }
}

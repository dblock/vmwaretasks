using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VixCOM;

namespace Vestris.VMWareLib
{
    public class VMWareJobCallback : ICallback
    {
        #region ICallback Members

        private EventWaitHandle _jobCompleted = new EventWaitHandle(false, EventResetMode.ManualReset);

        public void OnVixEvent(IJob job, int eventType, IVixHandle moreEventInfo)
        {
            switch (eventType)
            {
                case VixCOM.Constants.VIX_EVENTTYPE_JOB_COMPLETED:
                    _jobCompleted.Set();
                    break;
            }
        }

        public bool TryWaitForCompletion(int timeoutInMilliseconds)
        {
            return _jobCompleted.WaitOne(timeoutInMilliseconds, false);
        }

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

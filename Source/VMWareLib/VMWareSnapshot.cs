using System;
using System.Collections.Generic;
using System.Text;
using VixCOM;

namespace Vestris.VMWareLib
{
    public class VMWareSnapshot
    {
        private IVM _vm = null;
        private ISnapshot _snapshot = null;

        public VMWareSnapshot(IVM vm, ISnapshot snapshot)
        {
            _vm = vm;
            _snapshot = snapshot;
        }

        /// <summary>
        /// Restores the virtual machine to the state when the specified snapshot was created.
        /// </summary>
        private void RevertToSnapshot(int powerOnOptions, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.RevertToSnapshot(_snapshot, powerOnOptions, null, null));
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Restores the virtual machine to the state when the specified snapshot was created.
        /// </summary>
        private void RevertToSnapshot(int timeoutInSeconds)
        {
            RevertToSnapshot(Constants.VIX_VMPOWEROP_NORMAL, timeoutInSeconds);
        }
    }
}

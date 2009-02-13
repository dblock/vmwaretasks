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
        public void RevertToSnapshot(int powerOnOptions, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.RevertToSnapshot(_snapshot, powerOnOptions, null, null));
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Restores the virtual machine to the state when the specified snapshot was created.
        /// </summary>
        public void RevertToSnapshot(int timeoutInSeconds)
        {
            RevertToSnapshot(Constants.VIX_VMPOWEROP_NORMAL, timeoutInSeconds);
        }

        /// <summary>
        /// Restores the virtual machine to the state when the specified snapshot was created.
        /// </summary>
        public void RevertToSnapshot()
        {
            RevertToSnapshot(VMWareInterop.Timeouts.RevertToSnapshotTimeout);
        }

        /// <summary>
        /// Get all child snapshots.
        /// </summary>
        /// <returns>a list of child snapshots</returns>
        public List<VMWareSnapshot> GetChildren()
        {
            List<VMWareSnapshot> childSnapshots = new List<VMWareSnapshot>();
            int nChildSnapshots = 0;
            VMWareInterop.Check(_snapshot.GetNumChildren(out nChildSnapshots));
            for (int i = 0; i < nChildSnapshots; i++)
            {
                ISnapshot childSnapshot = null;
                VMWareInterop.Check(_snapshot.GetChild(i, out childSnapshot));
                childSnapshots.Add(new VMWareSnapshot(_vm, childSnapshot));
            }
            return childSnapshots;
        }
    }
}

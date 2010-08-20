using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// RevertTo a snapshot.
    /// </summary>
    public class VirtualMachineRevertToSnapshot : VirtualMachineOpen
    {
        private int _revertToSnapshotTimeout = VMWareInterop.Timeouts.RevertToSnapshotTimeout;
        private string _snapshotName;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int RevertToSnapshotTimeout
        {
            get
            {
                return _revertToSnapshotTimeout;
            }
            set
            {
                _revertToSnapshotTimeout = value;
            }
        }

        /// <summary>
        /// Name of the snapshot to revertTo.
        /// </summary>
        public string SnapshotName
        {
            get
            {
                return _snapshotName;
            }
            set
            {
                _snapshotName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    Log.LogMessage(string.Format("Reverting to snapshot {0}", _snapshotName));
                    virtualMachine.Snapshots.FindSnapshotByName(_snapshotName).RevertToSnapshot(
                        0, _revertToSnapshotTimeout);
                }
            }

            return true;
        }
    }
}

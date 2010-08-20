using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Remove a snapshot.
    /// </summary>
    public class VirtualMachineRemoveSnapshot : VirtualMachineOpen
    {
        private int _removeSnapshotTimeout = VMWareInterop.Timeouts.RemoveSnapshotTimeout;
        private string _snapshotName;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int RemoveSnapshotTimeout
        {
            get
            {
                return _removeSnapshotTimeout;
            }
            set
            {
                _removeSnapshotTimeout = value;
            }
        }

        /// <summary>
        /// Name of the snapshot to remove.
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
                    Log.LogMessage(string.Format("Removing snapshot {0}", _snapshotName));
                    virtualMachine.Snapshots.RemoveSnapshot(_snapshotName, _removeSnapshotTimeout);
                }
            }

            return true;
        }
    }
}

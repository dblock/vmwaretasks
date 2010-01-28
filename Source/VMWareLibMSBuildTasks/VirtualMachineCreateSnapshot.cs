using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Create a snapshot.
    /// </summary>
    public class VirtualMachineCreateSnapshot : VirtualMachineOpen
    {
        private int _createSnapshotTimeout = VMWareInterop.Timeouts.CreateSnapshotTimeout;
        private string _snapshotName;
        private string _snapshotDescription;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int CreateSnapshotTimeout
        {
            get
            {
                return _createSnapshotTimeout;
            }
            set
            {
                _createSnapshotTimeout = value;
            }
        }

        /// <summary>
        /// Name of the snapshot to create.
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
        /// Description of the snapshot to create.
        /// </summary>
        public string SnapshotDescription
        {
            get
            {
                return _snapshotDescription;
            }
            set
            {
                _snapshotDescription = value;
            }
        }

        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    Log.LogMessage(string.Format("Creating snapshot {0}", _snapshotName));
                    using (VMWareSnapshot snapshot = virtualMachine.Snapshots.CreateSnapshot(
                        _snapshotName, _snapshotDescription, 0, _createSnapshotTimeout))
                    {
                        // snapshot created
                    }
                }
            }

            return true;
        }
    }
}

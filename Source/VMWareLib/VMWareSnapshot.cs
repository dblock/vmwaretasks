using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using VixCOM;

namespace Vestris.VMWareLib
{
    public class VMWareSnapshot
    {
        private IVM _vm = null;
        private ISnapshot _snapshot = null;
        private VMWareSnapshotCollection _childSnapshots = null;

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
        /// Remove/delete this snapshot.
        /// </summary>
        public void RemoveSnapshot()
        {
            RemoveSnapshot(VMWareInterop.Timeouts.RemoveSnapshotTimeout);
        }

        /// <summary>
        /// Remove/delete this snapshot.
        /// </summary>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void RemoveSnapshot(int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.RemoveSnapshot(_snapshot, 0, null));
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Get all child snapshots.
        /// </summary>
        /// <returns>a list of child snapshots</returns>
        public VMWareSnapshotCollection ChildSnapshots
        {
            get
            {
                if (_childSnapshots == null)
                {
                    VMWareSnapshotCollection childSnapshots = new VMWareSnapshotCollection(_vm);
                    int nChildSnapshots = 0;
                    VMWareInterop.Check(_snapshot.GetNumChildren(out nChildSnapshots));
                    for (int i = 0; i < nChildSnapshots; i++)
                    {
                        ISnapshot childSnapshot = null;
                        VMWareInterop.Check(_snapshot.GetChild(i, out childSnapshot));
                        childSnapshots.Add(new VMWareSnapshot(_vm, childSnapshot));
                    }
                    _childSnapshots = childSnapshots;
                }
                return _childSnapshots;
            }
        }

        /// <summary>
        /// Display name of the snapshot.
        /// </summary>
        public string DisplayName
        {
            get
            {
                object[] properties = { Constants.VIX_PROPERTY_SNAPSHOT_DISPLAYNAME };
                return (string)new VMWareVixHandle((IVixHandle)_snapshot).GetProperties(properties)[0];
            }
        }

        /// <summary>
        /// Display name of the snapshot.
        /// </summary>
        public string Description
        {
            get
            {
                object[] properties = { Constants.VIX_PROPERTY_SNAPSHOT_DESCRIPTION };
                return (string)new VMWareVixHandle((IVixHandle)_snapshot).GetProperties(properties)[0];
            }
        }

        /// <summary>
        /// Complete snapshot path, from root.
        /// </summary>
        public string Path
        {
            get
            {
                ISnapshot parentSnapshot = null;
                VMWareInterop.Check(_snapshot.GetParent(out parentSnapshot));
                // hack: get the parent's parent snapshot: if this fails, we're looking at the root
                ISnapshot parentsParentSnapshot = null;
                return (parentSnapshot.GetParent(out parentsParentSnapshot) != VixCOM.Constants.VIX_OK)
                    ? DisplayName
                    : System.IO.Path.Combine(new VMWareSnapshot(_vm, parentSnapshot).Path, DisplayName);
            }
        }
    }
}

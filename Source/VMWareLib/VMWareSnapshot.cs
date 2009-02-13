using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VMWare snapshot.
    /// </summary>
    public class VMWareSnapshot : VMWareVixHandle<ISnapshot>
    {
        private IVM _vm = null;
        private VMWareSnapshotCollection _childSnapshots = null;
        private VMWareSnapshot _parent = null;

        /// <summary>
        /// A VMWare snapshot constructor.
        /// </summary>
        /// <param name="vm">virtual machine</param>
        /// <param name="snapshot">snapshot</param>
        /// <param name="parent">parent snapshot</param>
        public VMWareSnapshot(IVM vm, ISnapshot snapshot, VMWareSnapshot parent)
            : base(snapshot)
        {
            _vm = vm;
            _parent = parent;
        }

        /// <summary>
        /// Parent snapshot.
        /// </summary>
        /// <remarks>
        /// Root snapshots have a null parent.
        /// </remarks>
        public VMWareSnapshot Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        /// <summary>
        /// Restores the virtual machine to the state when the specified snapshot was created.
        /// </summary>
        /// <param name="powerOnOptions">additional power-on options</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void RevertToSnapshot(int powerOnOptions, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_vm.RevertToSnapshot(_handle, powerOnOptions, null, callback), callback);
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Restores the virtual machine to the state when the specified snapshot was created.
        /// </summary>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
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
        /// <remarks>
        /// If the snapshot is a member of a collection, the latter is updated with orphaned
        /// snapshots appended to the parent.
        /// </remarks>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void RemoveSnapshot(int timeoutInSeconds)
        {
            // resolve child snapshots that will move one level up
            IEnumerable<VMWareSnapshot> childSnapshots = ChildSnapshots;
            // remove the snapshot
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_vm.RemoveSnapshot(_handle, 0, callback), callback);
            job.Wait(timeoutInSeconds);
            if (_parent != null)
            {
                // child snapshots from this snapshot have now moved one level up
                _parent.ChildSnapshots.Remove(this);
            }
        }

        /// <summary>
        /// Child snapshots.
        /// </summary>
        public VMWareSnapshotCollection ChildSnapshots
        {
            get
            {
                if (_childSnapshots == null)
                {
                    VMWareSnapshotCollection childSnapshots = new VMWareSnapshotCollection(_vm, this);
                    int nChildSnapshots = 0;
                    VMWareInterop.Check(_handle.GetNumChildren(out nChildSnapshots));
                    for (int i = 0; i < nChildSnapshots; i++)
                    {
                        ISnapshot childSnapshot = null;
                        VMWareInterop.Check(_handle.GetChild(i, out childSnapshot));
                        childSnapshots.Add(new VMWareSnapshot(_vm, childSnapshot, this));
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
                return GetProperty<string>(Constants.VIX_PROPERTY_SNAPSHOT_DISPLAYNAME);
            }
        }

        /// <summary>
        /// Display name of the snapshot.
        /// </summary>
        public string Description
        {
            get
            {
                return GetProperty<string>(Constants.VIX_PROPERTY_SNAPSHOT_DESCRIPTION);
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
                ulong ulError = 0;
                switch ((ulError = _handle.GetParent(out parentSnapshot)))
                {
                    case Constants.VIX_OK:
                        return System.IO.Path.Combine(new VMWareSnapshot(_vm, parentSnapshot, null).Path, DisplayName);
                    case Constants.VIX_E_SNAPSHOT_NOTFOUND: // no parent
                        return DisplayName;
                    case Constants.VIX_E_INVALID_ARG: // root snapshot
                        return string.Empty;
                    default:
                        throw new VMWareException(ulError);
                }
            }
        }

        /// <summary>
        /// The power state of this snapshot, an OR-ed set of VIX_POWERSTATE_* values.
        /// </summary>
        public int PowerState
        {
            get
            {
                return GetProperty<int>(Constants.VIX_PROPERTY_SNAPSHOT_POWERSTATE);
            }
        }

        /// <summary>
        /// Returns true if the snapshot is replayable.
        /// </summary>
        public bool IsReplayable
        {
            get
            {
                return GetProperty<bool>(Constants.VIX_PROPERTY_SNAPSHOT_IS_REPLAYABLE);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VixCOM;
using System.IO;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A collection of root snapshots.
    /// Shared snapshots will only be accessible inside the guest operating system if snapshots are 
    /// enabled for the virtual machine.
    /// </summary>
    public class VMWareRootSnapshotCollection : VMWareSnapshotCollection
    {
        public VMWareRootSnapshotCollection(IVM vm)
            : base(vm)
        {

        }

        /// <summary>
        /// Get snapshots.
        /// </summary>
        /// <returns>a list of snapshots</returns>
        protected override List<VMWareSnapshot> Snapshots
        {
            get
            {
                if (_snapshots == null)
                {
                    List<VMWareSnapshot> snapshots = new List<VMWareSnapshot>();
                    int nSnapshots = 0;
                    VMWareInterop.Check(_vm.GetNumRootSnapshots(out nSnapshots));
                    for (int i = 0; i < nSnapshots; i++)
                    {
                        ISnapshot snapshot = null;
                        VMWareInterop.Check(_vm.GetRootSnapshot(i, out snapshot));
                        snapshots.Add(new VMWareSnapshot(_vm, snapshot));
                    }
                    _snapshots = snapshots;
                }

                return _snapshots;
            }
        }

        /// <summary>
        /// Get the first snapshot by its exact name. 
        /// </summary>
        /// <param name="name">snapshot name</param>
        /// <returns>a snapshot</returns>
        public VMWareSnapshot GetNamedSnapshot(string name)
        {
            ISnapshot snapshot = null;
            VMWareInterop.Check(_vm.GetNamedSnapshot(name, out snapshot));
            return new VMWareSnapshot(_vm, snapshot);
        }

        /// <summary>
        /// Current snapshot.
        /// </summary>
        /// <returns>current snapshot</returns>
        public VMWareSnapshot GetCurrentSnapshot()
        {
            ISnapshot snapshot = null;
            VMWareInterop.Check(_vm.GetCurrentSnapshot(out snapshot));
            return new VMWareSnapshot(_vm, snapshot);
        }

        /// <summary>
        /// Delete/remove a snapshot.
        /// </summary>
        /// <param name="item">snapshot to delete</param>
        /// <returns>true if the snapshot was deleted</returns>
        public void RemoveSnapshot(VMWareSnapshot item)
        {
            item.RemoveSnapshot();
            _snapshots = null;
        }

        /// <summary>
        /// Delete a snapshot.
        /// </summary>
        /// <param name="item">snapshot to delete</param>
        /// <returns>true if the snapshot was deleted</returns>
        public void RemoveSnapshot(string name)
        {
            RemoveSnapshot(GetNamedSnapshot(name));
        }

        /// <summary>
        /// Create a new snapshot, child of the current snapshot.
        /// </summary>
        /// <param name="name">snapshot name</param>
        /// <param name="description">snapshot description</param>
        public void CreateSnapshot(string name, string description)
        {
            CreateSnapshot(name, description, 0, VMWareInterop.Timeouts.CreateSnapshotTimeout);
        }

        /// <summary>
        /// Create a new snapshot, child of the current snapshot.
        /// </summary>
        /// <param name="name">snapshot name</param>
        /// <param name="description">snapshot description</param>
        /// <param name="flags">flags, one of 
        /// VIX_SNAPSHOT_INCLUDE_MEMORY: Captures the full state of a running virtual machine, including the memory
        /// </param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void CreateSnapshot(string name, string description, int flags, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.CreateSnapshot(name, description, 0, null, null));
            job.Wait(timeoutInSeconds);
            _snapshots = null;
        }
    }
}

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
    /// </summary>
    /// <remarks>
    /// Shared snapshots will only be accessible inside the guest operating system if snapshots are 
    /// enabled for the virtual machine.
    /// </remarks>
    public class VMWareRootSnapshotCollection : VMWareSnapshotCollection
    {
        /// <summary>
        ///  A collection of snapshots that belong to a virtual machine.
        /// </summary>
        /// <param name="vm">a virtual machine instance</param>
        public VMWareRootSnapshotCollection(IVM vm)
            : base(vm, null)
        {

        }

        /// <summary>
        /// A list of root snapshots on the current virtual machine.
        /// </summary>
        /// <remarks>
        /// The list is populated on first access, this may time some time.
        /// </remarks>
        /// <returns>A list of snapshots.</returns>
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
                        snapshots.Add(new VMWareSnapshot(_vm, snapshot, null));
                    }
                    _snapshots = snapshots;
                }

                return _snapshots;
            }
        }

        /// <summary>
        /// Get a snapshot by its exact name. 
        /// </summary>
        /// <param name="name">Snapshot name.</param>
        /// <returns>A snapshot or null if the snapshot doesn't exist.</returns>
        /// <remarks>This function will throw an exception if more than one snapshot with the same exists.</remarks>
        public VMWareSnapshot GetNamedSnapshot(string name)
        {
            ISnapshot snapshot = null;
            ulong rc = _vm.GetNamedSnapshot(name, out snapshot);
            switch (rc)
            {
                case VixCOM.Constants.VIX_E_SNAPSHOT_NOTFOUND:
                    break;
                case VixCOM.Constants.VIX_OK:
                    return new VMWareSnapshot(_vm, snapshot, null);
                default:
                    VMWareInterop.Check(rc);
                    break;
            }
            return null;
        }

        /// <summary>
        /// Current snapshot.
        /// </summary>
        /// <returns>Current snapshot.</returns>
        public VMWareSnapshot GetCurrentSnapshot()
        {
            ISnapshot snapshot = null;
            VMWareInterop.Check(_vm.GetCurrentSnapshot(out snapshot));
            return new VMWareSnapshot(_vm, snapshot, null);
        }

        /// <summary>
        /// Delete/remove a snapshot.
        /// </summary>
        /// <param name="item">snapshot to delete</param>
        /// <returns>True if the snapshot was deleted.</returns>
        public void RemoveSnapshot(VMWareSnapshot item)
        {
            item.RemoveSnapshot();
            _snapshots = null;
        }

        /// <summary>
        /// Delete a snapshot.
        /// </summary>
        /// <param name="name">name of the snapshot to delete</param>
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
        /// <list type="bullet">
        ///  <item>VIX_SNAPSHOT_INCLUDE_MEMORY: Captures the full state of a running virtual machine, including the memory</item>
        /// </list>
        /// </param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void CreateSnapshot(string name, string description, int flags, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_vm.CreateSnapshot(name, description, 0, null, callback), callback);
            job.Wait(timeoutInSeconds);
            _snapshots = null;
        }
    }
}

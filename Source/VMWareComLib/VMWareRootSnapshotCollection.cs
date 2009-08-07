using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    [ComVisible(true)]
    [ComDefaultInterface(typeof(IVMWareRootSnapshotCollection))]
    [ProgId("VMWareComLib.VMWareRootSnapshotCollection")]
    [Guid("7A733057-9205-424e-852B-C5F64F91719F")]
    public class VMWareRootSnapshotCollection : IVMWareSnapshotCollection, IVMWareRootSnapshotCollection
    {
        private Vestris.VMWareLib.VMWareRootSnapshotCollection _snapshots = null;

        public VMWareRootSnapshotCollection()
        {

        }

        public VMWareRootSnapshotCollection(Vestris.VMWareLib.VMWareRootSnapshotCollection snapshots)
        {
            _snapshots = snapshots;
        }

        public IVMWareSnapshot GetNamedSnapshot(string name)
        {
            Vestris.VMWareLib.VMWareSnapshot snapshot = _snapshots.GetNamedSnapshot(name);
            return (snapshot == null ? null : new VMWareSnapshot(snapshot));
        }

        public IVMWareSnapshot GetCurrentSnapshot()
        {
            Vestris.VMWareLib.VMWareSnapshot snapshot = _snapshots.GetCurrentSnapshot();
            return (snapshot == null ? null : new VMWareSnapshot(snapshot));
        }

        public void RemoveSnapshot(string name)
        {
            _snapshots.RemoveSnapshot(name);
        }

        public void CreateSnapshot(string name, string description)
        {
            _snapshots.CreateSnapshot(name, description);
        }

        public void CreateSnapshot2(string name, string description, int flags, int timeoutInSeconds)
        {
            _snapshots.CreateSnapshot(name, description, flags, timeoutInSeconds);
        }

        public IVMWareSnapshot FindSnapshot(string pathToSnapshot)
        {
            Vestris.VMWareLib.VMWareSnapshot snapshot = _snapshots.FindSnapshot(pathToSnapshot);
            return (snapshot == null ? null : new VMWareSnapshot(snapshot));
        }

        public IVMWareSnapshot FindSnapshotByName(string name)
        {
            Vestris.VMWareLib.VMWareSnapshot snapshot = _snapshots.FindSnapshotByName(name);
            return (snapshot == null ? null : new VMWareSnapshot(snapshot));
        }

        public IVMWareSnapshot[] FindSnapshotsByName(string name)
        {
            List<Vestris.VMWareLib.VMWareSnapshot> snapshots = new List<Vestris.VMWareLib.VMWareSnapshot>(_snapshots.FindSnapshotsByName(name));
            List<VMWareSnapshot> result = new List<VMWareSnapshot>(snapshots.Count);
            foreach (Vestris.VMWareLib.VMWareSnapshot snapshot in snapshots)
            {
                result.Add(new VMWareSnapshot(snapshot));
            }
            return result.ToArray();
        }

        public int Count
        {
            get
            {
                return _snapshots.Count;
            }
        }
    }
}

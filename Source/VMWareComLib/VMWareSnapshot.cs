using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    [ComVisible(true)]
    [ComDefaultInterface(typeof(IVMWareSnapshot))]
    [Guid("0D890EF7-B433-41f7-95EC-A5AAA2E046A0")]
    [ProgId("VMWareComLib.VMWareSnapshot")]
    public class VMWareSnapshot : IVMWareSnapshot
    {
        private Vestris.VMWareLib.VMWareSnapshot _snapshot = null;

        public VMWareSnapshot()
        {

        }

        public VMWareSnapshot(Vestris.VMWareLib.VMWareSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public IVMWareSnapshot Parent
        {
            get
            {
                return new VMWareSnapshot(_snapshot.Parent);
            }
        }

        public void RevertToSnapshot()
        {
            _snapshot.RevertToSnapshot();
        }

        public void RevertToSnapshot2(int powerOnOptions, int timeoutInSeconds)
        {
            _snapshot.RevertToSnapshot(powerOnOptions, timeoutInSeconds);
        }

        public void RemoveSnapshot()
        {
            _snapshot.RemoveSnapshot();
        }

        public void RemoveSnapshot2(int timeoutInSeconds)
        {
            _snapshot.RemoveSnapshot(timeoutInSeconds);
        }

        public IVMWareSnapshotCollection ChildSnapshots
        {
            get
            {
                return new VMWareSnapshotCollection(_snapshot.ChildSnapshots);
            }
        }

        public string DisplayName
        {
            get
            {
                return _snapshot.DisplayName;
            }
        }

        public string Description
        {
            get
            {
                return _snapshot.Description;
            }
        }

        public string Path
        {
            get
            {
                return _snapshot.Path;
            }
        }

        public int PowerState
        {
            get
            {
                return _snapshot.PowerState;
            }
        }

        public bool IsReplayable
        {
            get
            {
                return _snapshot.IsReplayable;
            }
        }

        public void BeginReplay()
        {
            _snapshot.BeginReplay();
        }

        public void BeginReplay2(int powerOnOptions, int timeoutInSeconds)
        {
            _snapshot.BeginReplay(powerOnOptions, timeoutInSeconds);
        }

        public void EndReplay()
        {
            _snapshot.EndReplay();
        }

        public void EndReplay2(int timeoutInSeconds)
        {
            _snapshot.EndReplay(timeoutInSeconds);
        }

        //public void Clone(VMWareVirtualMachineCloneType cloneType, string destConfigPathName)
        //{
        //    _snapshot.Clone(cloneType, destConfigPathName);
        //}

        //public void Clone(VMWareVirtualMachineCloneType cloneType, string destConfigPathName, int timeoutInSeconds)
        //{
        //    _snapshot.Clone(cloneType, destConfigPathName, timeoutInSeconds);
        //}
    }
}

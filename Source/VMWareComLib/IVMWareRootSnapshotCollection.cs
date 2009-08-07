using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("9F3BF070-992F-4188-ACAE-9D588689475C")]
    public interface IVMWareRootSnapshotCollection
    {
        void CreateSnapshot(string name, string description);
        void CreateSnapshot2(string name, string description, int flags, int timeoutInSeconds);
        IVMWareSnapshot GetCurrentSnapshot();
        IVMWareSnapshot GetNamedSnapshot(string name);
        void RemoveSnapshot(string name);
    }
}

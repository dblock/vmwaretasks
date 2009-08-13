using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    /// <summary>
    /// A COM interface to <see cref="Vestris.VMWareLib.VMWareSnapshotCollection" />.
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("C828C67A-15A7-431b-95DF-B6EEF6467408")]
    public interface IVMWareSnapshotCollection
    {
        int Count { get; }
        IVMWareSnapshot FindSnapshot(string pathToSnapshot);
        IVMWareSnapshot FindSnapshotByName(string name);
        IVMWareSnapshot[] FindSnapshotsByName(string name);
    }
}

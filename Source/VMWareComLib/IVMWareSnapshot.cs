using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    /// <summary>
    /// A COM interface to <see cref="Vestris.VMWareLib.VMWareSnapshot" />.
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("BBE59046-66AF-458c-AD14-3F25546193E9")]
    public interface IVMWareSnapshot
    {
        //void BeginReplay();
        //void BeginReplay2(int powerOnOptions, int timeoutInSeconds);
        string Description { get; }
        string DisplayName { get; }
        //void EndReplay();
        //void EndReplay2(int timeoutInSeconds);
        //bool IsReplayable { get; }
        IVMWareSnapshot Parent { get; }
        string Path { get; }
        int PowerState { get; }
        void RemoveSnapshot();
        void RemoveSnapshot2(int timeoutInSeconds);
        void RevertToSnapshot();
        void RevertToSnapshot2(int powerOnOptions, int timeoutInSeconds);
        IVMWareSnapshotCollection ChildSnapshots { get; }
    }
}

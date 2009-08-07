using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("D43E20F9-F151-4b44-BD5B-13AFA7FB27D5")]
    public interface IVMWareSharedFolderCollection
    {
        void Clear();
        int Count { get; }
        bool Enabled { set; }
        IVMWareSharedFolder this[int index] { get; }
    }
}

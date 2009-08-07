using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("E3610635-DF7B-48b1-BFB5-F9D2CED6961C")]
    public interface IGuestFileInfo
    {
        long FileSize { get; }
        int Flags { get; }
        string GuestPathName { get; }
        bool IsDirectory { get; }
        bool IsSymLink { get; }
        DateTime LastModified { get; }
    }
}

using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    /// <summary>
    /// A COM interface to <see cref="Vestris.VMWareLib.VMWareSharedFolder" />.
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("276BCAEF-0C02-4a4e-9FB6-A826804D67DC")]
    public interface IVMWareSharedFolder
    {
        int Flags { get; }
        string HostPath { get; }
        string ShareName { get; }
    }
}

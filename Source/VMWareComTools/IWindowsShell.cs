using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib.Tools.Windows
{
    /// <summary>
    /// A COM interface to <see cref="Vestris.VMWareLib.Tools.Windows.Shell" />.
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("392F462E-E4A3-4771-8074-804316D4E426")]
    public interface IShell
    {
        IShellOutput RunCommandInGuest(string guestCommandLine);
        Vestris.VMWareComLib.IVMWareVirtualMachine VirtualMachine { get; set; }
    }
}

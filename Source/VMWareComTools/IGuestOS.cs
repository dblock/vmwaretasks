using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib.Tools
{
    /// <summary>
    /// A COM interface to <see cref="Vestris.VMWareLib.Tools.GuestOS" />.
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("8CEC2F45-71F2-446a-B878-C9E5AA79D34C")]
    public interface IGuestOS
    {
        string IpAddress { get; }
        string ReadFile(string guestFilename);
        // string ReadFile(string guestFilename, System.Text.Encoding encoding);
        byte[] ReadFileBytes(string guestFilename);
        string[] ReadFileLines(string guestFilename);
        //string[] ReadFileLines(string guestFilename, System.Text.Encoding encoding);
        Vestris.VMWareComLib.IVMWareVirtualMachine VirtualMachine { get; set; }
    }
}

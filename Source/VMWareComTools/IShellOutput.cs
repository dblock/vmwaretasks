using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib.Tools
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("E02DC9CC-322B-4d4a-A7A4-D0140435B68A")]
    public interface IShellOutput
    {
        string StdErr { get; }
        string StdOut { get; }
    }
}

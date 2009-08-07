using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("504BC958-8E3E-4db0-A64C-E7410E1CDCCC")]
    public interface IVariableIndexer
    {
        string this[string name] { get; }
    }
}

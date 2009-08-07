using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("01E37251-FEB4-497f-9302-E05B1DEE9DAA")]
    public interface IProcess
    {
        string Command { get; }
        int ExitCode { get; }
        long Id { get; }
        bool IsBeingDebugged { get; }
        void KillProcessInGuest();
        void KillProcessInGuest(int timeoutInSeconds);
        string Name { get; }
        string Owner { get; }
        DateTime StartDateTime { get; }
    }
}

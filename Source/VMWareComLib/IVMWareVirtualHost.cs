using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    /// <summary>
    /// A COM interface to <see cref="Vestris.VMWareLib.VMWareVirtualHost" />.
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("0209B715-5167-42e7-8A32-DFEB9AAC767C")]
    public interface IVMWareVirtualHost
    {
        void ConnectToVMWareServer(string hostName, string username, string password);
        void ConnectToVMWareServer2(string hostName, string username, string password, int timeoutInSeconds);
        void ConnectToVMWareVIServer(string hostName, string username, string password);
        void ConnectToVMWareWorkstation();
        void ConnectToVMWareWorkstation2(int timeoutInSeconds);
        void Disconnect();
        bool IsConnected { get; }
        IVMWareVirtualMachine Open(string fileName);
        IVMWareVirtualMachine Open2(string fileName, int timeoutInSeconds);
        void Register(string fileName);
        void Register2(string fileName, int timeoutInSeconds);
        IVMWareVirtualMachine[] RegisteredVirtualMachines { get; }
        IVMWareVirtualMachine[] RunningVirtualMachines { get; }
        void Unregister(string fileName);
        void Unregister2(string fileName, int timeoutInSeconds);
    }
}

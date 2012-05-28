using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Vestris.VMWareComLib
{
    /// <summary>
    /// The default implementation of the <see cref="Vestris.VMWareComLib.IVMWareVirtualHost" /> COM interface.
    /// </summary>
    [ComVisible(true)]
    [Guid("23333B5F-D53F-40c6-8B1F-96D11004CA91")]
    [ProgId("VMWareComLib.VMWareVirtualHost")]
    [ComDefaultInterface(typeof(IVMWareVirtualHost))]
    public class VMWareVirtualHost : IVMWareVirtualHost
    {
        private Vestris.VMWareLib.VMWareVirtualHost _host = new Vestris.VMWareLib.VMWareVirtualHost();

        public VMWareVirtualHost()
        {

        }

        public void ConnectToVMWareWorkstation()
        {
            _host.ConnectToVMWareWorkstation();
        }

        public void ConnectToVMWareWorkstation2(int timeoutInSeconds)
        {
            _host.ConnectToVMWareWorkstation(timeoutInSeconds);
        }

        public void ConnectToVMWareWorkstationShared()
        {
            _host.ConnectToVMWareWorkstationShared();
        }

        public void ConnectToVMWareWorkstationShared2(int timeoutInSeconds)
        {
            _host.ConnectToVMWareWorkstationShared(timeoutInSeconds);
        }

        public void ConnectToVMWareVIServer(string hostName, string username, string password)
        {
            _host.ConnectToVMWareVIServer(hostName, username, password);
        }

        public void ConnectToVMWareVIServer2(string hostName, string username, string password, int timeoutInSeconds)
        {
            _host.ConnectToVMWareVIServer(hostName, username, password, timeoutInSeconds);
        }

        public void ConnectToVMWareServer(string hostName, string username, string password)
        {
            _host.ConnectToVMWareServer(hostName, username, password);
        }

        public void ConnectToVMWareServer2(string hostName, string username, string password, int timeoutInSeconds)
        {
            _host.ConnectToVMWareServer(hostName, username, password, timeoutInSeconds);
        }

        public IVMWareVirtualMachine Open(string fileName)
        {
            return new VMWareVirtualMachine(_host.Open(fileName));
        }

        public IVMWareVirtualMachine Open2(string fileName, int timeoutInSeconds)
        {
            return new VMWareVirtualMachine(_host.Open(fileName, timeoutInSeconds));
        }

        public void Register(string fileName)
        {
            _host.Register(fileName);
        }

        public void Register2(string fileName, int timeoutInSeconds)
        {
            _host.Register(fileName, timeoutInSeconds);
        }

        public void Unregister(string fileName)
        {
            _host.Unregister(fileName);
        }

        public void Unregister2(string fileName, int timeoutInSeconds)
        {
            _host.Unregister(fileName, timeoutInSeconds);
        }

        public void Disconnect()
        {
            _host.Disconnect();
        }

        public bool IsConnected
        {
            get
            {
                return _host.IsConnected;
            }
        }

        public IVMWareVirtualMachine[] RunningVirtualMachines
        {
            get
            {
                List<VMWareVirtualMachine> virtualMachines = new List<VMWareVirtualMachine>();
                foreach (Vestris.VMWareLib.VMWareVirtualMachine vm in _host.RunningVirtualMachines)
                {
                    virtualMachines.Add(new VMWareVirtualMachine(vm));
                }
                return virtualMachines.ToArray();
            }
        }

        public IVMWareVirtualMachine[] RegisteredVirtualMachines
        {
            get
            {
                List<VMWareVirtualMachine> virtualMachines = new List<VMWareVirtualMachine>();
                foreach (Vestris.VMWareLib.VMWareVirtualMachine vm in _host.RegisteredVirtualMachines)
                {
                    virtualMachines.Add(new VMWareVirtualMachine(vm));
                }
                return virtualMachines.ToArray();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VMWare virtual host.
    /// </summary>
    public class VMWareVirtualHost
    {
        private IHost _host = null;

        public VMWareVirtualHost()
        {

        }

        /// <summary>
        /// Connect to a WMWare Workstation.
        /// </summary>
        public void ConnectToVMWareWorkstation()
        {
            ConnectToVMWareWorkstation(VMWareInterop.Timeouts.ConnectTimeout);
        }

        /// <summary>
        /// Connect to a WMWare Workstation.
        /// </summary>
        public void ConnectToVMWareWorkstation(int timeoutInSeconds)
        {
            Connect(Constants.VIX_SERVICEPROVIDER_VMWARE_WORKSTATION,
                string.Empty, 0, string.Empty, string.Empty, timeoutInSeconds);
        }

        /// <summary>
        /// Connect to a WMWare Virtual Infrastructure Server (eg. ESX).
        /// </summary>
        public void ConnectToVMWareVIServer(string hostName, int hostPort, string username, string password)
        {
            ConnectToVMWareVIServer(hostName, hostPort, username, password, VMWareInterop.Timeouts.ConnectTimeout);
        }

        /// <summary>
        /// Connect to a WMWare Virtual Infrastructure Server (eg. ESX).
        /// </summary>
        public void ConnectToVMWareVIServer(string hostName, int hostPort, string username, string password, int timeoutInSeconds)
        {
            Connect(Constants.VIX_SERVICEPROVIDER_VMWARE_VI_SERVER,
                hostName, hostPort, username, password, timeoutInSeconds);
        }

        /// <summary>
        /// Connects to a VMWare Server or Workstation.
        /// </summary>
        private void Connect(int hostType, string hostName, int hostPort, string username, string password, int timeout)
        {
            VMWareJob job = new VMWareJob(VMWareInterop.Vix.Connect(
                Constants.VIX_API_VERSION,
                hostType, hostName, hostPort,
                username, password, 0, null, null)
                );
            object[] resultProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
            _host = job.Wait<IHost>(resultProperties, 0, timeout);
        }

        /// <summary>
        /// Open a .vmx file.
        /// </summary>
        /// <param name="fileName">Virtual Machine file, local .vmx or [storage] .vmx</param>
        /// <returns>an instance of a virtual machine</returns>
        public VMWareVirtualMachine Open(string fileName)
        {
            return Open(fileName, VMWareInterop.Timeouts.OpenFileTimeout);
        }

        /// <summary>
        /// Open a .vmx file.
        /// </summary>
        /// <param name="fileName">Virtual Machine file, local .vmx or [storage] .vmx</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        /// <returns>an instance of a virtual machine</returns>
        public VMWareVirtualMachine Open(string fileName, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_host.OpenVM(fileName, null));
            object[] resultProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
            return new VMWareVirtualMachine(job.Wait<IVM>(resultProperties, 0, timeoutInSeconds));
        }
    }
}

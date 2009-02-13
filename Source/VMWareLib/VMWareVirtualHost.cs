using System;
using System.Collections.Generic;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VMWare virtual host.
    /// </summary>
    public class VMWareVirtualHost : VMWareVixHandle<IHost>, IDisposable
    {
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
        /// <param name="hostName">VMWare host name</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        public void ConnectToVMWareVIServer(string hostName, string username, string password)
        {
            ConnectToVMWareVIServer(new Uri(string.Format("http://{0}/sdk", hostName)),
                username, password, VMWareInterop.Timeouts.ConnectTimeout);
        }

        /// <summary>
        /// Connect to a WMWare Virtual Infrastructure Server (eg. ESX).
        /// </summary>
        /// <param name="hostUri">host SDK uri, eg. http://server/sdk</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="hostPort">host port</param>
        public void ConnectToVMWareVIServer(Uri hostUri, string username, string password, int timeoutInSeconds)
        {
            Connect(Constants.VIX_SERVICEPROVIDER_VMWARE_VI_SERVER,
                hostUri.ToString(), 0, username, password, timeoutInSeconds);
        }

        /// <summary>
        /// Connects to a VMWare Server or Workstation.
        /// </summary>
        private void Connect(int hostType, string hostName, int hostPort, string username, string password, int timeout)
        {
            VMWareJob job = new VMWareJob(VMWareInterop.Instance.Connect(
                Constants.VIX_API_VERSION,
                hostType, hostName, hostPort,
                username, password, 0, null, null)
                );

            _handle = job.Wait<IHost>(Constants.VIX_PROPERTY_JOB_RESULT_HANDLE, timeout);
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
            VMWareJob job = new VMWareJob(_handle.OpenVM(fileName, null));
            return new VMWareVirtualMachine(job.Wait<IVM2>(
                Constants.VIX_PROPERTY_JOB_RESULT_HANDLE,
                timeoutInSeconds));
        }

        public void Dispose()
        {
            if (_handle != null)
            {
                Disconnect();
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disconnect from a remote host.
        /// </summary>
        public void Disconnect()
        {
            if (_handle == null)
            {
                throw new InvalidOperationException("No connection established");
            }

            _handle.Disconnect();
            _handle = null;
        }

        ~VMWareVirtualHost()
        {
            if (_handle != null)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// All running virtual machines.
        /// </summary>
        public IEnumerable<VMWareVirtualMachine> RunningVirtualMachines
        {
            get
            {
                VMWareJob job = new VMWareJob(_handle.FindItems(Constants.VIX_FIND_RUNNING_VMS, null, -1, null));
                object[] properties = { Constants.VIX_PROPERTY_FOUND_ITEM_LOCATION };
                foreach (object[] runningVirtualMachine in job.YieldWait(properties, VMWareInterop.Timeouts.FindItemsTimeout))
                {
                    yield return this.Open((string) runningVirtualMachine[0]);
                }
            }
        }
    }
}

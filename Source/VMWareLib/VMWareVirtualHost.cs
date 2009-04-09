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
        /// <summary>
        /// VMWare service provider type.
        /// </summary>
        public enum ServiceProviderType
        {
            /// <summary>
            /// No service provider type, not connected.
            /// </summary>
            None = 0,
            /// <summary>
            /// VMWare Server.
            /// </summary>
            Server = Constants.VIX_SERVICEPROVIDER_VMWARE_SERVER,
            /// <summary>
            /// VMWare Workstation.
            /// </summary>
            Workstation = Constants.VIX_SERVICEPROVIDER_VMWARE_WORKSTATION,
            /// <summary>
            /// Virtual Infrastructure Server, eg. ESX.
            /// </summary>
            VirtualInfrastructureServer = Constants.VIX_SERVICEPROVIDER_VMWARE_VI_SERVER
        }

        private ServiceProviderType _serviceProviderType = ServiceProviderType.None;

        /// <summary>
        /// A VMWare virtual host.
        /// </summary>
        public VMWareVirtualHost()
        {

        }

        /// <summary>
        /// Connected host type.
        /// </summary>
        public ServiceProviderType ConnectionType
        {
            get
            {
                return _serviceProviderType;
            }
        }

        /// <summary>
        /// Connect to a WMWare Workstation.
        /// </summary>
        /// <example>
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using Vestris.VMWareLib;
        /// 
        /// VMWareVirtualHost virtualHost = new VMWareVirtualHost();
        /// virtualHost.ConnectToVMWareWorkstation();
        /// VMWareVirtualMachine virtualMachine = virtualHost.Open("C:\Virtual Machines\xp\xp.vmx");
        /// virtualMachine.PowerOn();
        /// </code>
        /// </example>
        public void ConnectToVMWareWorkstation()
        {
            ConnectToVMWareWorkstation(VMWareInterop.Timeouts.ConnectTimeout);
        }

        /// <summary>
        /// Connect to a WMWare Workstation.
        /// </summary>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public void ConnectToVMWareWorkstation(int timeoutInSeconds)
        {
            Connect(ServiceProviderType.Workstation, null, 0, null, null, timeoutInSeconds);
        }

        /// <summary>
        /// Connect to a WMWare Virtual Infrastructure Server (eg. ESX).
        /// </summary>
        /// <param name="hostName">VMWare host name.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <example>
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using Vestris.VMWareLib;
        /// 
        /// VMWareVirtualHost virtualHost = new VMWareVirtualHost();
        /// virtualHost.ConnectToVMWareVIServer("esx.mycompany.com", "vmuser", "password");
        /// VMWareVirtualMachine virtualMachine = virtualHost.Open("[storage] testvm/testvm.vmx");
        /// virtualMachine.PowerOn();
        /// </code>
        /// </example>
        public void ConnectToVMWareVIServer(string hostName, string username, string password)
        {
            ConnectToVMWareVIServer(new Uri(string.Format("https://{0}/sdk", hostName)),
                username, password, VMWareInterop.Timeouts.ConnectTimeout);
        }

        /// <summary>
        /// Connect to a WMWare Virtual Infrastructure Server (eg. ESX).
        /// </summary>
        /// <param name="hostUri">Host SDK uri, eg. http://server/sdk.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public void ConnectToVMWareVIServer(Uri hostUri, string username, string password, int timeoutInSeconds)
        {
            Connect(ServiceProviderType.VirtualInfrastructureServer,
                hostUri.ToString(), 0, username, password, timeoutInSeconds);
        }

        /// <summary>
        /// Connect to a WMWare Server.
        /// </summary>
        /// <param name="hostName">DNS name or IP address of a VMWare host, leave blank for localhost.</param>
        public void ConnectToVMWareServer(string hostName)
        {
            Connect(ServiceProviderType.Server,
                hostName, 0, null, null, VMWareInterop.Timeouts.ConnectTimeout);
        }

        /// <summary>
        /// Connect to a WMWare Server.
        /// </summary>
        /// <param name="hostName">DNS name or IP address of a VMWare host, leave blank for localhost.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public void ConnectToVMWareServer(string hostName, int timeoutInSeconds)
        {
            Connect(ServiceProviderType.Server,
                hostName, 0, null, null, timeoutInSeconds);
        }

        /// <summary>
        /// Connects to a VMWare VI Server, VMWare Server or Workstation.
        /// </summary>
        private void Connect(ServiceProviderType serviceProviderType, 
            string hostName, int hostPort, string username, string password, int timeout)
        {
            int serviceProvider = (int)serviceProviderType;
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(VMWareInterop.Instance.Connect(
                Constants.VIX_API_VERSION, serviceProvider, hostName, hostPort,
                username, password, 0, null, callback), callback);

            _handle = job.Wait<IHost>(Constants.VIX_PROPERTY_JOB_RESULT_HANDLE, timeout);
            _serviceProviderType = serviceProviderType;
        }

        /// <summary>
        /// Open a .vmx file.
        /// </summary>
        /// <param name="fileName">Virtual Machine file, local .vmx or [storage] .vmx.</param>
        /// <returns>An instance of a virtual machine.</returns>
        public VMWareVirtualMachine Open(string fileName)
        {
            return Open(fileName, VMWareInterop.Timeouts.OpenFileTimeout);
        }

        /// <summary>
        /// Open a .vmx file.
        /// </summary>
        /// <param name="fileName">Virtual Machine file, local .vmx or [storage] .vmx.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        /// <returns>An instance of a virtual machine.</returns>
        public VMWareVirtualMachine Open(string fileName, int timeoutInSeconds)
        {
            if (_handle == null)
            {
                throw new InvalidOperationException("No connection established");
            }

            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.OpenVM(fileName, callback), callback);
            return new VMWareVirtualMachine(job.Wait<IVM2>(
                Constants.VIX_PROPERTY_JOB_RESULT_HANDLE,
                timeoutInSeconds));
        }

        /// <summary>
        /// Dispose the object, hard-disconnect from the remote host.
        /// </summary>
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
            _serviceProviderType = ServiceProviderType.None;
        }

        /// <summary>
        /// Returns true when connected to a virtual host, false otherwise.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _handle != null;
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~VMWareVirtualHost()
        {
            if (_handle != null)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Returns all running virtual machines.
        /// </summary>
        public IEnumerable<VMWareVirtualMachine> RunningVirtualMachines
        {
            get
            {
                if (_handle == null)
                {
                    throw new InvalidOperationException("No connection established");
                }

                VMWareJobCallback callback = new VMWareJobCallback();
                VMWareJob job = new VMWareJob(_handle.FindItems(
                    Constants.VIX_FIND_RUNNING_VMS, null, -1, callback), 
                    callback);
                
                object[] properties = { Constants.VIX_PROPERTY_FOUND_ITEM_LOCATION };
                foreach (object[] runningVirtualMachine in job.YieldWait(
                    properties, VMWareInterop.Timeouts.FindItemsTimeout))
                {
                    yield return this.Open((string) runningVirtualMachine[0]);
                }
            }
        }

        /// <summary>
        /// All registered virtual machines.
        /// </summary>
        /// <remarks>This function is only supported on Virtual Infrastructure servers.</remarks>
        public IEnumerable<VMWareVirtualMachine> RegisteredVirtualMachines
        {
            get
            {
                switch (ConnectionType)
                {
                    case ServiceProviderType.VirtualInfrastructureServer:
                        VMWareJobCallback callback = new VMWareJobCallback();
                        VMWareJob job = new VMWareJob(_handle.FindItems(
                            Constants.VIX_FIND_REGISTERED_VMS, null, -1, callback),
                            callback);
                        
                        object[] properties = { Constants.VIX_PROPERTY_FOUND_ITEM_LOCATION };
                        foreach (object[] runningVirtualMachine in job.YieldWait(properties, VMWareInterop.Timeouts.FindItemsTimeout))
                        {
                            yield return this.Open((string)runningVirtualMachine[0]);
                        }

                        break;
                    default:
                        throw new NotSupportedException(string.Format("RegisteredVirtualMachines is not supported on {0}",
                            ConnectionType));
                }
            }
        }

    }
}

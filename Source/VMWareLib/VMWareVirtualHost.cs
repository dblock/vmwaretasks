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
        public void ConnectToVMWareWorkstation()
        {
            ConnectToVMWareWorkstation(VMWareInterop.Timeouts.ConnectTimeout);
        }

        /// <summary>
        /// Connect to a WMWare Workstation.
        /// </summary>
        public void ConnectToVMWareWorkstation(int timeoutInSeconds)
        {
            Connect(ServiceProviderType.Workstation, null, 0, null, null, timeoutInSeconds);
        }

        /// <summary>
        /// Connect to a WMWare Virtual Infrastructure Server (eg. ESX).
        /// </summary>
        /// <param name="hostName">VMWare host name</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        public void ConnectToVMWareVIServer(string hostName, string username, string password)
        {
            ConnectToVMWareVIServer(new Uri(string.Format("https://{0}/sdk", hostName)),
                username, password, VMWareInterop.Timeouts.ConnectTimeout);
        }

        /// <summary>
        /// Connect to a WMWare Virtual Infrastructure Server (eg. ESX).
        /// </summary>
        /// <param name="hostUri">host SDK uri, eg. http://server/sdk</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void ConnectToVMWareVIServer(Uri hostUri, string username, string password, int timeoutInSeconds)
        {
            Connect(ServiceProviderType.VirtualInfrastructureServer,
                hostUri.ToString(), 0, username, password, timeoutInSeconds);
        }

        /// <summary>
        /// Connects to a VMWare Server or Workstation.
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
        /// <param name="fileName">Virtual Machine file, local .vmx or [storage] .vmx</param>
        /// <returns>An instance of a virtual machine.</returns>
        public VMWareVirtualMachine Open(string fileName)
        {
            return Open(fileName, VMWareInterop.Timeouts.OpenFileTimeout);
        }

        /// <summary>
        /// Open a .vmx file.
        /// </summary>
        /// <param name="fileName">Virtual Machine file, local .vmx or [storage] .vmx</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
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

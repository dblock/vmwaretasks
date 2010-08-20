using System;
using Vestris.VMWareLib;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Connect to a virtual machine host.
    /// </summary>
    public class VirtualHostConnect : Task
    {
        private VMWareVirtualHost.ServiceProviderType _type = VMWareVirtualHost.ServiceProviderType.None;
        private int _connectTimeout = VMWareInterop.Timeouts.ConnectTimeout;
        private string _host = null;
        private string _hostUsername = null;
        private string _hostPassword = null;

        /// <summary>
        /// Connected host type.
        /// See <see cref="Vestris.VMWareLib.VMWareVirtualHost.ServiceProviderType" /> for possible values.
        /// </summary>
        public string ConnectionType
        {
            get
            {
                return _type.ToString();
            }
            set
            {
                _type = (VMWareVirtualHost.ServiceProviderType)
                    Enum.Parse(typeof(VMWareVirtualHost.ServiceProviderType), value);
            }
        }

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int ConnectTimeout
        {
            get
            {
                return _connectTimeout;
            }
            set
            {
                _connectTimeout = value;
            }
        }

        /// <summary>
        /// Host name.
        /// </summary>
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
            }
        }

        /// <summary>
        /// Host username.
        /// </summary>
        public string HostUsername
        {
            get
            {
                return _hostUsername;
            }
            set
            {
                _hostUsername = value;
            }
        }

        /// <summary>
        /// Host password.
        /// </summary>
        public string HostPassword
        {
            get
            {
                return _hostPassword;
            }
            set
            {
                _hostPassword = value;
            }
        }

        /// <summary>
        /// Connected virtual machine host.
        /// </summary>
        protected VMWareVirtualHost GetConnectedHost()
        {
            VMWareVirtualHost host = new VMWareVirtualHost();

            switch (_type)
            {
                case VMWareVirtualHost.ServiceProviderType.Player:
                    Log.LogMessage("Connecting to VMWare Player");
                    host.ConnectToVMWarePlayer(_connectTimeout);
                    break;
                case VMWareVirtualHost.ServiceProviderType.Server:
                    Log.LogMessage(string.Format("Connecting to VMWare Server '{0}'",
                        string.IsNullOrEmpty(_host) ? "localhost" : _host));
                    host.ConnectToVMWareServer(_host, _hostUsername, _hostPassword, _connectTimeout);
                    break;
                case VMWareVirtualHost.ServiceProviderType.VirtualInfrastructureServer:
                    Log.LogMessage(string.Format("Connecting to VMWare VI server '{0}'",
                        _host));
                    host.ConnectToVMWareVIServer(_host, _hostUsername, _hostPassword, _connectTimeout);
                    break;
                case VMWareVirtualHost.ServiceProviderType.Workstation:
                    Log.LogMessage("Connecting to VMWare Workstation");
                    host.ConnectToVMWareWorkstation(_connectTimeout);
                    break;
                default:
                    Log.LogError(string.Format("Invalid connection type: {0}", _type));
                    throw new InvalidOperationException(string.Format("Invalid connection type: {0}", _type));
            }

            return host;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                // connected host
            }

            return true;
        }
    }
}

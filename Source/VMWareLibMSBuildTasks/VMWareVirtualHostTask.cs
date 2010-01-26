using System;
using Vestris.VMWareLib;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// VMWare virtual host MSBuild task.
    /// </summary>
    public class VirtualHostConnect : Task
    {
        private VMWareVirtualHost.ServiceProviderType _type = VMWareVirtualHost.ServiceProviderType.None;
        private int _timeout = VMWareInterop.Timeouts.ConnectTimeout;
        private string _host = null;
        private string _username = null;
        private string _password = null;

        /// <summary>
        /// Connected host type.
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
        /// Connection timeout in seconds.
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
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
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        /// <summary>
        /// Host password.
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public override bool Execute()
        {            
            using (VMWareVirtualHost host = new VMWareVirtualHost())
            {
                switch (_type)
                {
                    case VMWareVirtualHost.ServiceProviderType.Player:
                        Log.LogMessage("Connecting to VMWare Player ...");
                        host.ConnectToVMWarePlayer(_timeout);
                        break;
                    case VMWareVirtualHost.ServiceProviderType.Server:
                        Log.LogMessage(string.Format("Connecting to VMWare Server '{0}' ...", 
                            string.IsNullOrEmpty(_host) ? "localhost" : _host));
                        host.ConnectToVMWareServer(_host, _username, _password, _timeout);
                        break;
                    case VMWareVirtualHost.ServiceProviderType.VirtualInfrastructureServer:
                        Log.LogMessage(string.Format("Connecting to VMWare VI server '{0}' ...",
                            _host));
                        host.ConnectToVMWareVIServer(_host, _username, _password, _timeout);
                        break;
                    case VMWareVirtualHost.ServiceProviderType.Workstation:
                        Log.LogMessage("Connecting to VMWare Workstation ...");
                        host.ConnectToVMWareWorkstation(_timeout);
                        break;
                    default:
                        Log.LogError(string.Format("Invalid connection type: {0}", _type));
                        throw new InvalidOperationException(string.Format("Invalid connection type: {0}", _type));
                }
            }

            return true;
        }
    }
}

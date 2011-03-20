using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using System.Configuration;
using Interop.VixCOM;

namespace Vestris.VMWareLibUnitTests
{
    public class TestVI : IVMWareTestProvider
    {
        private VMWareVirtualHost _host = null;
        private VMWareVirtualMachine _virtualMachine = null;
        private VMWareVirtualMachineConfig _config = null;

        public TestVI(VMWareVirtualMachineConfig config)
        {
            _config = config;
        }

        public VMWareVirtualHost ConnectedVirtualHost
        {
            get
            {
                if (_host == null)
                {
                    _host = new VMWareVirtualHost();
                }

                if (!_host.IsConnected)
                {
                    ConsoleOutput.WriteLine("Connecting to: {0} ...", _config.Host);
                    _host.ConnectToVMWareVIServer(_config.Host, _config.HostUsername, _config.HostPassword);
                    ConsoleOutput.WriteLine("Connection established.");
                }

                return _host;
            }
        }

        public VMWareVirtualHost VirtualHost
        {
            get
            {
                if (_host == null)
                {
                    _host = new VMWareVirtualHost();
                }

                return _host;
            }
        }

        public VMWareVirtualHost Reconnect()
        {
            _virtualMachine = null;
            _host = null;
            return ConnectedVirtualHost;
        }

        public VMWareVirtualMachine VirtualMachine
        {
            get
            {
                if (_virtualMachine == null)
                {
                    ConsoleOutput.WriteLine("Opening: {0}", _config.File);
                    _virtualMachine = ConnectedVirtualHost.Open(_config.File);
                }
                return _virtualMachine;
            }
        }

        public VMWareVirtualMachine PoweredVirtualMachine
        {
            get
            {
                if (!string.IsNullOrEmpty(_config.Snapshot))
                {
                    ConsoleOutput.WriteLine(string.Format("Reverting to snapshot {0}", _config.Snapshot));
                    VirtualMachine.Snapshots.FindSnapshotByName(_config.Snapshot).RevertToSnapshot(Constants.VIX_VMPOWEROP_SUPPRESS_SNAPSHOT_POWERON);
                }

                if (! VirtualMachine.IsRunning)
                {
                    ConsoleOutput.WriteLine("Powering on: {0}", _config.File);
                    VirtualMachine.PowerOn();

                    ConsoleOutput.WriteLine("Waiting for tools ...");
                    VirtualMachine.WaitForToolsInGuest();
                }

                ConsoleOutput.WriteLine("Logging in ...");
                VirtualMachine.LoginInGuest(Username, Password);

                return _virtualMachine;
            }
        }

        public string Username
        {
            get
            {
                return _config.GuestUsername;
            }
        }

        public string Password
        {
            get
            {
                return _config.GuestPassword;
            }
        }

        public void Dispose()
        {
            if (_virtualMachine != null)
            {
                _virtualMachine.Dispose();
                _virtualMachine = null;
            }

            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }

            _config = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    public class VMWareTestVirtualMachine
    {
        private VMWareVirtualHost _host = null;
        private VMWareVirtualMachine _virtualMachine = null;
        private bool _poweredOn = false;

        public VMWareVirtualHost LocalHost
        {
            get
            {
                if (_host == null)
                {
                    VMWareVirtualHost virtualHost = new VMWareVirtualHost();
                    // connect to a local VM
                    virtualHost.ConnectToVMWareWorkstation();
                    _host = virtualHost;
                }
                return _host;
            }
        }

        public VMWareVirtualMachine VirtualMachine
        {
            get
            {
                if (_virtualMachine == null)
                {
                    string testWorkstationFilename = ConfigurationManager.AppSettings["testWorkstationFilename"];
                    _virtualMachine = LocalHost.Open(testWorkstationFilename);
                }
                return _virtualMachine;
            }
        }

        public VMWareVirtualMachine PoweredVirtualMachine
        {
            get
            {
                if (! _poweredOn)
                {
                    // power-on current snapshot
                    VirtualMachine.PowerOn();
                    string testUsername = ConfigurationManager.AppSettings["testWorkstationUsername"];
                    string testPassword = ConfigurationManager.AppSettings["testWorkstationPassword"];
                    VirtualMachine.Login(testUsername, testPassword);
                    // assign last not to get a value on exception
                    _poweredOn = true;
                }
                return _virtualMachine;
            }
        }

        public static VMWareTestVirtualMachine VM = new VMWareTestVirtualMachine();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    public class TestVI
    {
        private VMWareVirtualHost _host = null;
        private VMWareVirtualMachine _virtualMachine = null;
        private bool _poweredOn = false;

        public VMWareVirtualHost VirtualHost
        {
            get
            {
                if (_host == null)
                {
                    VMWareVirtualHost virtualHost = new VMWareVirtualHost();
                    Console.WriteLine("Connecting to: {0}", ConfigurationManager.AppSettings["testVIHost"]);
                    virtualHost.ConnectToVMWareVIServer(
                        ConfigurationManager.AppSettings["testVIHost"],
                        ConfigurationManager.AppSettings["testVIHostUsername"],
                        ConfigurationManager.AppSettings["testVIHostPassword"]);
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
                    string testVIFilename = ConfigurationManager.AppSettings["testVIFilename"];
                    Console.WriteLine("Opening: {0}", testVIFilename);
                    _virtualMachine = VirtualHost.Open(testVIFilename);
                }
                return _virtualMachine;
            }
        }

        public VMWareVirtualMachine PoweredVirtualMachine
        {
            get
            {
                if (!_poweredOn)
                {
                    Console.WriteLine("Powering on: {0}", ConfigurationManager.AppSettings["testVIFilename"]);
                    // power-on current snapshot
                    VirtualMachine.PowerOn();
                    string testUsername = ConfigurationManager.AppSettings["testVIUsername"];
                    string testPassword = ConfigurationManager.AppSettings["testVIPassword"];
                    VirtualMachine.Login(testUsername, testPassword);
                    // assign last not to get a value on exception
                    _poweredOn = true;
                }
                return _virtualMachine;
            }
        }

        public static TestVI Instance = new TestVI();
    }
}

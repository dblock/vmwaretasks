using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    public class TestWorkstation : IVMWareTestProvider
    {
        private VMWareVirtualHost _host = null;
        private VMWareVirtualMachine _virtualMachine = null;

        public VMWareVirtualHost VirtualHost
        {
            get
            {
                if (_host == null)
                {
                    VMWareVirtualHost virtualHost = new VMWareVirtualHost();
                    Console.WriteLine("Connecting to local host ...");
                    // connect to a local VM
                    virtualHost.ConnectToVMWareWorkstation();
                    Console.WriteLine("Connection established.");
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
                    Console.WriteLine("Opening: {0}", ConfigurationManager.AppSettings["testWorkstationFilename"]);
                    string testWorkstationFilename = ConfigurationManager.AppSettings["testWorkstationFilename"];
                    _virtualMachine = VirtualHost.Open(testWorkstationFilename);
                }
                return _virtualMachine;
            }
        }

        public VMWareVirtualMachine PoweredVirtualMachine
        {
            get
            {
                if (! VirtualMachine.IsRunning)
                {
                    Console.WriteLine("Powering on: {0}", ConfigurationManager.AppSettings["testWorkstationFilename"]);
                    VirtualMachine.PowerOn();
                    Console.WriteLine("Waiting for tools ...");
                    VirtualMachine.WaitForToolsInGuest();
                }
                Console.WriteLine("Logging in ...");
                VirtualMachine.LoginInGuest(Username, Password);
                return _virtualMachine;
            }
        }

        public string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["testWorkstationUsername"];
            }
        }

        public string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["testWorkstationPassword"];
            }
        }

        public static TestWorkstation Instance = new TestWorkstation();
    }
}

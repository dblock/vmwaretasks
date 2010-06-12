using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using System.Configuration;
using NUnit.Framework;

namespace Vestris.VMWareLibUnitTests
{
    public interface IVMWareTestProvider : IDisposable
    {
        VMWareVirtualHost VirtualHost { get; }
        VMWareVirtualHost Reconnect();
        VMWareVirtualMachine VirtualMachine { get; }
        VMWareVirtualMachine PoweredVirtualMachine { get; }
        string Username { get; }
        string Password { get; }
    }

    public class VMWareTest : IDisposable
    {
        private VMWareTestsConfig _config = null;

        public VMWareTest Instance = null;

        public VMWareTest()
        {
            _config = (VMWareTestsConfig)ConfigurationManager.GetSection("TestsConfig");
            Assert.IsNotNull(_config);
        }

        public void SetUp()
        {
            if (Instance == null)
            {
                Instance = new VMWareTest();
            }
        }

        public void TearDown()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
        }

        public VMWareTestsConfig Config
        {
            get
            {
                return _config;
            }
        }

        /// <summary>
        /// A set of generic virtual machines to run tests on.
        /// </summary>
        public IEnumerable<VMWareVirtualMachine> VirtualMachines
        {
            get
            {
                foreach (IVMWareTestProvider provider in Providers)
                    yield return provider.VirtualMachine;
            }
        }

        /// <summary>
        /// A set of generic virtual machines to run tests on.
        /// </summary>
        public IEnumerable<VMWareVirtualMachine> PoweredVirtualMachines
        {
            get
            {
                foreach (IVMWareTestProvider provider in Providers)
                    yield return provider.PoweredVirtualMachine;
            }
        }

        /// <summary>
        /// A set of generic virtual hosts.
        /// </summary>
        public IEnumerable<VMWareVirtualHost> VirtualHosts
        {
            get
            {
                foreach (IVMWareTestProvider provider in Providers)
                    yield return provider.VirtualHost;
            }
        }

        /// <summary>
        /// A set of generic virtual hosts.
        /// </summary>
        public IEnumerable<IVMWareTestProvider> Providers
        {
            get
            {
                foreach (VMWareVirtualMachineConfig virtualMachineConfig in _config.VirtualMachines)
                {
                    if (_config.RunVITests && virtualMachineConfig.Type == VMWareVirtualMachineType.ESX)
                        yield return virtualMachineConfig.Provider;

                    if (_config.RunWorkstationTests && virtualMachineConfig.Type == VMWareVirtualMachineType.Workstation)
                        yield return virtualMachineConfig.Provider;
                }
            }
        }

        public void Dispose()
        {
            if (_config != null)
            {
                _config.Dispose();
                _config = null;
            }
        }
    }
}

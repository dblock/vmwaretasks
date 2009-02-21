using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    public enum VMWareTestType
    {
        VI,
        Workstation
    }

    public interface IVMWareTestProvider
    {
        VMWareVirtualHost VirtualHost { get; }
        VMWareVirtualMachine VirtualMachine { get; }
        VMWareVirtualMachine PoweredVirtualMachine { get; }
    }

    /// <summary>
    /// An abstract VMWareTest driver.
    /// </summary>
    public class VMWareTest : IVMWareTestProvider
    {
        private VMWareTestType _testType = VMWareTestType.Workstation;
        private IVMWareTestProvider _provider = null;

        public VMWareTest()
            : this((VMWareTestType) Enum.Parse(typeof(VMWareTestType), ConfigurationManager.AppSettings["testType"]))
        {

        }

        /// <summary>
        /// Current test type.
        /// </summary>
        public VMWareTestType TestType
        {
            get
            {
                return _testType;
            }
        }

        public VMWareTest(VMWareTestType testType)
        {
            _testType = testType;
            switch (testType)
            {
                case VMWareTestType.VI:
                    _provider = TestVI.Instance;
                    break;
                case VMWareTestType.Workstation:
                default:
                    _provider = TestWorkstation.Instance;
                    break;
            }
        }

        /// <summary>
        /// A virtual machine.
        /// </summary>
        public VMWareVirtualMachine VirtualMachine
        {
            get
            {
                return _provider.VirtualMachine;
            }
        }

        /// <summary>
        /// A powered VM.
        /// </summary>
        public VMWareVirtualMachine PoweredVirtualMachine
        {
            get
            {
                return _provider.PoweredVirtualMachine;
            }
        }

        /// <summary>
        /// A virtual host.
        /// </summary>
        public VMWareVirtualHost VirtualHost
        {
            get
            {
                return _provider.VirtualHost;
            }
        }

        public static VMWareTest Instance = new VMWareTest();
    }
}

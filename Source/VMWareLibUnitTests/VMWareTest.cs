using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    public interface IVMWareTestProvider
    {
        VMWareVirtualHost VirtualHost { get; }
        VMWareVirtualMachine VirtualMachine { get; }
        VMWareVirtualMachine PoweredVirtualMachine { get; }
        string Username { get; }
        string Password { get; }
    }

    public abstract class VMWareTest
    {
        public static bool RunVITests
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["testVI"]);
            }
        }

        public static bool RunWorkstationTests
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["testWorkstation"]);
            }
        }

        /// <summary>
        /// A set of generic virtual machines to run tests on.
        /// </summary>
        public static IEnumerable<VMWareVirtualMachine> VirtualMachines
        {
            get
            {
                if (RunVITests)
                    yield return TestVI.Instance.VirtualMachine;
                if (RunWorkstationTests)
                    yield return TestWorkstation.Instance.VirtualMachine;
            }
        }

        /// <summary>
        /// A set of generic virtual machines to run tests on.
        /// </summary>
        public static IEnumerable<VMWareVirtualMachine> PoweredVirtualMachines
        {
            get
            {
                if (RunVITests)
                    yield return TestVI.Instance.PoweredVirtualMachine;

                if (RunWorkstationTests)
                    yield return TestWorkstation.Instance.PoweredVirtualMachine;
            }
        }

        /// <summary>
        /// A set of generic virtual hosts.
        /// </summary>
        public static IEnumerable<VMWareVirtualHost> VirtualHosts
        {
            get
            {
                if (RunVITests)
                    yield return TestVI.Instance.VirtualHost;
                if (RunWorkstationTests)
                    yield return TestWorkstation.Instance.VirtualHost;
            }
        }

        /// <summary>
        /// A set of test providers.
        /// </summary>
        public static IEnumerable<IVMWareTestProvider> Providers
        {
            get
            {
                if (RunVITests)
                    yield return TestVI.Instance;
                if (RunWorkstationTests)
                    yield return TestWorkstation.Instance;
            }
        }
    }   
}

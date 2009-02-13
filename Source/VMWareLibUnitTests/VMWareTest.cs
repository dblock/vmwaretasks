using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    public abstract class VMWareTest
    {
        /// <summary>
        /// A set of generic virtual machines to run tests on.
        /// </summary>
        public static IEnumerable<VMWareVirtualMachine> VirtualMachines
        {
            get
            {
                if (bool.Parse(ConfigurationManager.AppSettings["testVI"]))
                    yield return TestVI.Instance.VirtualMachine;
                if (bool.Parse(ConfigurationManager.AppSettings["testWorkstation"]))
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
                if (bool.Parse(ConfigurationManager.AppSettings["testVI"]))
                    yield return TestVI.Instance.PoweredVirtualMachine;
                if (bool.Parse(ConfigurationManager.AppSettings["testWorkstation"]))
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
                if (bool.Parse(ConfigurationManager.AppSettings["testVI"]))
                    yield return TestVI.Instance.VirtualHost;
                if (bool.Parse(ConfigurationManager.AppSettings["testWorkstation"]))
                    yield return TestWorkstation.Instance.LocalHost;
            }
        }

    }
}

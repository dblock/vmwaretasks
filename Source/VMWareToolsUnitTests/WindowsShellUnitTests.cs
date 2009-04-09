using System;
using System.Text;
using NUnit.Framework;
using Vestris.VMWareLibUnitTests;
using Vestris.VMWareLib.Tools.Windows;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Vestris.VMWareToolsUnitTests.Windows
{
    [TestFixture]
    public class ShellUnitTests : VMWareTestSetup
    {
        [Test]
        public void TestRunCommandStdOutInGuest()
        {
            Shell guestShell = new Shell(VMWareTest.Instance.PoweredVirtualMachine);
            string guid = Guid.NewGuid().ToString();
            StringBuilder commands = new StringBuilder();
            commands.AppendLine(string.Format("echo {0}", guid));
            Shell.ShellOutput output = guestShell.RunCommandInGuest(commands.ToString());
            Assert.AreEqual(guid, output.StdOut.Trim());
            Assert.IsTrue(string.IsNullOrEmpty(output.StdErr));
        }

        [Test]
        public void TestRunCommandStdOutStdErrInGuest()
        {
            Shell guestShell = new Shell(VMWareTest.Instance.PoweredVirtualMachine); 
            string guid = Guid.NewGuid().ToString();
            StringBuilder commands = new StringBuilder();
            commands.AppendLine(string.Format("echo {0}", guid));
            commands.AppendLine(string.Format("echo {0} 1>&2", guid));
            Shell.ShellOutput output = guestShell.RunCommandInGuest(commands.ToString());
            Console.WriteLine(output.StdOut);
            Console.WriteLine(output.StdErr);
            Assert.AreEqual(guid, output.StdOut.Trim());
            Assert.AreEqual(guid, output.StdErr.Trim());
        }

        [Test]
        public void TestGetEnvironmentVariables()
        {
            Shell guestShell = new Shell(VMWareTest.Instance.PoweredVirtualMachine);
            Dictionary<string, string> guestEnvironmentVariables = guestShell.GetEnvironmentVariables();
            Dictionary<string, string>.Enumerator guestEnumerator = guestEnvironmentVariables.GetEnumerator();
            while (guestEnumerator.MoveNext())
            {
                Console.WriteLine(string.Format("{0}: {1}",
                    guestEnumerator.Current.Key, guestEnumerator.Current.Value));
            }
            Assert.IsTrue(guestEnvironmentVariables.ContainsKey("Path"));
            Assert.IsTrue(guestEnvironmentVariables.ContainsKey("USERPROFILE"));
        }
    }
}

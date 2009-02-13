using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;
using System.IO;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualMachineTests
    {
        private VMWareVirtualMachine _virtualMachine = null;

        private VMWareVirtualMachine VirtualMachine
        {
            get
            {
                if (_virtualMachine == null)
                {
                    VMWareVirtualHost virtualHost = new VMWareVirtualHost();
                    // connect to a local VM
                    virtualHost.ConnectToVMWareWorkstation();
                    string testWorkstationFilename = ConfigurationManager.AppSettings["testWorkstationFilename"];
                    VMWareVirtualMachine virtualMachine = virtualHost.Open(testWorkstationFilename);
                    // power-on current snapshot
                    virtualMachine.PowerOn();
                    string testUsername = ConfigurationManager.AppSettings["testWorkstationUsername"];
                    string testPassword = ConfigurationManager.AppSettings["testWorkstationPassword"];
                    virtualMachine.Login(testUsername, testPassword);
                    // assign last not to get a value on exception
                    _virtualMachine = virtualMachine;
                }

                return _virtualMachine;
            }
        }

        [Test]
        public void TestWorkstationTasks()
        {
            // copy a batch file to the remote machine, execute it and collect results
            string localTempFilename = Path.GetTempFileName();
            string remoteTempFilename = string.Format(@"C:\{0}", Path.GetFileName(localTempFilename));
            string remoteBatFilename = string.Format(@"C:\{0}.bat", Path.GetFileNameWithoutExtension(localTempFilename));
            File.WriteAllText(localTempFilename, string.Format(@"dir C:\ > {0}", remoteTempFilename));
            VirtualMachine.CopyFileFromHostToGuest(localTempFilename, remoteBatFilename);
            Assert.AreEqual(0, VirtualMachine.Execute(@"cmd.exe", string.Format("/C \"{0}\"", remoteBatFilename)));
            VirtualMachine.CopyFileFromGuestToHost(remoteTempFilename, localTempFilename);
            string remoteDirectoryListing = File.ReadAllText(localTempFilename);
            Console.WriteLine(remoteDirectoryListing);
            Assert.IsTrue(remoteDirectoryListing.Contains(Path.GetFileName(remoteTempFilename)));
            Assert.IsTrue(remoteDirectoryListing.Contains(Path.GetFileName(remoteBatFilename)));
            // delete the temp files
            VirtualMachine.DeleteFileFromGuest(remoteTempFilename);
            VirtualMachine.DeleteFileFromGuest(remoteBatFilename);
            File.Delete(localTempFilename);
            // VirtualMachine.PowerOff();
        }

        [Test, ExpectedException(typeof(VMWareException))]
        public void TestWorkstationListDirectoryInGuestInvalidDirectory()
        {
            VirtualMachine.ListDirectoryInGuest(string.Format(@"C:\{0}", Guid.NewGuid()), false);
        }

        [Test]
        public void TestWorkstationListDirectoryInGuestEmptyDirectory()
        {
            List<string> listOfInetPubBadMail = VirtualMachine.ListDirectoryInGuest(@"C:\Inetpub\mailroot\Badmail", false);
            Assert.AreEqual(0, listOfInetPubBadMail.Count);
        }

        [Test]
        public void TestWorkstationListDirectoryInGuest()
        {
            List<string> listOfInetPub = VirtualMachine.ListDirectoryInGuest(@"C:\Inetpub", false);
            List<string> listOfInetPubWithSub = VirtualMachine.ListDirectoryInGuest(@"C:\Inetpub", true);
            Assert.AreEqual(0, listOfInetPub.Count);
            Assert.IsTrue(listOfInetPub.Count < listOfInetPubWithSub.Count);
            Assert.IsTrue(listOfInetPubWithSub.Contains(@"C:\Inetpub\AdminScripts\adsutil.vbs"));
        }

        [Test]
        public void TestGetSetGuestVariables()
        {
            string ip = VirtualMachine.GuestVariables["ip"];
            Assert.IsFalse(string.IsNullOrEmpty(ip));
            Console.WriteLine("IP: {0}", VirtualMachine.GuestVariables["ip"]);
            string guid = Guid.NewGuid().ToString();
            Assert.IsTrue(string.IsNullOrEmpty(VirtualMachine.GuestVariables[guid]));
            VirtualMachine.GuestVariables[guid] = guid;
            Assert.AreEqual(guid, VirtualMachine.GuestVariables[guid]);
        }

        /// <summary>
        /// bug: this isn't working, the API doesn't do what it's supposed to do
        /// </summary>
        [Test]
        protected void TestGetSetGuestEnvironmentVariables()
        {
            string guid = Guid.NewGuid().ToString();
            Assert.IsTrue(string.IsNullOrEmpty(VirtualMachine.GuestEnvironmentVariables[guid]));
            VirtualMachine.GuestVariables[guid] = guid;
            Assert.AreEqual(guid, VirtualMachine.GuestEnvironmentVariables[guid]);
        }

        [Test]
        public void TestGetSetRuntimeConfigVariables()
        {
            string displayName = VirtualMachine.RuntimeConfigVariables["displayname"];
            Console.WriteLine("Display name: {0}", displayName);
            Assert.IsFalse(string.IsNullOrEmpty(displayName));
        }

    }
}

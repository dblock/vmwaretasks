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
        [Test]
        public void TestWorkstationTasks()
        {
            // copy a batch file to the remote machine, execute it and collect results
            string localTempFilename = Path.GetTempFileName();
            string remoteTempFilename = string.Format(@"C:\{0}", Path.GetFileName(localTempFilename));
            string remoteBatFilename = string.Format(@"C:\{0}.bat", Path.GetFileNameWithoutExtension(localTempFilename));
            File.WriteAllText(localTempFilename, string.Format(@"dir C:\ > {0}", remoteTempFilename));
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.CopyFileFromHostToGuest(localTempFilename, remoteBatFilename);
            Assert.AreEqual(0, VMWareTestVirtualMachine.VM.PoweredVirtualMachine.Execute(@"cmd.exe", string.Format("/C \"{0}\"", remoteBatFilename)));
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.CopyFileFromGuestToHost(remoteTempFilename, localTempFilename);
            string remoteDirectoryListing = File.ReadAllText(localTempFilename);
            Console.WriteLine(remoteDirectoryListing);
            Assert.IsTrue(remoteDirectoryListing.Contains(Path.GetFileName(remoteTempFilename)));
            Assert.IsTrue(remoteDirectoryListing.Contains(Path.GetFileName(remoteBatFilename)));
            // delete the temp files
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.DeleteFileFromGuest(remoteTempFilename);
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.DeleteFileFromGuest(remoteBatFilename);
            File.Delete(localTempFilename);
            // VMWareTestVirtualMachine.VM.PoweredVirtualMachine.PowerOff();
        }

        [Test, ExpectedException(typeof(VMWareException))]
        public void TestWorkstationListDirectoryInGuestInvalidDirectory()
        {
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.ListDirectoryInGuest(string.Format(@"C:\{0}", Guid.NewGuid()), false);
        }

        [Test]
        public void TestWorkstationListDirectoryInGuestEmptyDirectory()
        {
            List<string> listOfInetPubBadMail = VMWareTestVirtualMachine.VM.PoweredVirtualMachine.ListDirectoryInGuest(@"C:\Inetpub\mailroot\Badmail", false);
            Assert.AreEqual(0, listOfInetPubBadMail.Count);
        }

        [Test]
        public void TestWorkstationListDirectoryInGuest()
        {
            List<string> listOfInetPub = VMWareTestVirtualMachine.VM.PoweredVirtualMachine.ListDirectoryInGuest(@"C:\Inetpub", false);
            List<string> listOfInetPubWithSub = VMWareTestVirtualMachine.VM.PoweredVirtualMachine.ListDirectoryInGuest(@"C:\Inetpub", true);
            Assert.AreEqual(0, listOfInetPub.Count);
            Assert.IsTrue(listOfInetPub.Count < listOfInetPubWithSub.Count);
            Assert.IsTrue(listOfInetPubWithSub.Contains(@"C:\Inetpub\AdminScripts\adsutil.vbs"));
        }

        [Test]
        public void TestGetSetGuestVariables()
        {
            string ip = VMWareTestVirtualMachine.VM.PoweredVirtualMachine.GuestVariables["ip"];
            Assert.IsFalse(string.IsNullOrEmpty(ip));
            Console.WriteLine("IP: {0}", ip);
            string guid = Guid.NewGuid().ToString();
            Assert.IsTrue(string.IsNullOrEmpty(VMWareTestVirtualMachine.VM.PoweredVirtualMachine.GuestVariables[guid]));
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.GuestVariables[guid] = guid;
            Assert.AreEqual(guid, VMWareTestVirtualMachine.VM.PoweredVirtualMachine.GuestVariables[guid]);
        }

        /// <summary>
        /// bug: this isn't working, the API doesn't do what it's supposed to do
        /// </summary>
        [Test]
        protected void TestGetSetGuestEnvironmentVariables()
        {
            string guid = Guid.NewGuid().ToString();
            Assert.IsTrue(string.IsNullOrEmpty(VMWareTestVirtualMachine.VM.PoweredVirtualMachine.GuestEnvironmentVariables[guid]));
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.GuestVariables[guid] = guid;
            Assert.AreEqual(guid, VMWareTestVirtualMachine.VM.PoweredVirtualMachine.GuestEnvironmentVariables[guid]);
        }

        [Test]
        public void TestGetSetRuntimeConfigVariables()
        {
            string displayName = VMWareTestVirtualMachine.VM.PoweredVirtualMachine.RuntimeConfigVariables["displayname"];
            Console.WriteLine("Display name: {0}", displayName);
            Assert.IsFalse(string.IsNullOrEmpty(displayName));
        }

        [Test]
        public void TestGetAddRemoveSharedFolders()
        {
            int count = VMWareTestVirtualMachine.VM.PoweredVirtualMachine.SharedFolders.Count;
            Console.WriteLine("Shared folders: {0}", count);
            // add a shared folder
            VMWareSharedFolder currentDirectory = new VMWareSharedFolder(Guid.NewGuid().ToString(), Environment.CurrentDirectory);
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.SharedFolders.Add(currentDirectory);
            Assert.AreEqual(count + 1, VMWareTestVirtualMachine.VM.PoweredVirtualMachine.SharedFolders.Count);
            foreach (VMWareSharedFolder sharedFolder in VMWareTestVirtualMachine.VM.PoweredVirtualMachine.SharedFolders)
            {
                Console.WriteLine("Shared folder: {0} ({1})", 
                    sharedFolder.ShareName, sharedFolder.HostPath);
            }
            // remove the shared folder
            VMWareTestVirtualMachine.VM.PoweredVirtualMachine.SharedFolders.Remove(currentDirectory);
            Assert.AreEqual(count, VMWareTestVirtualMachine.VM.PoweredVirtualMachine.SharedFolders.Count);
        }
    }
}

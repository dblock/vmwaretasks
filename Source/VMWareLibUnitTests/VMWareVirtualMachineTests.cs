using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Threading;
using VixCOM;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualMachineTests : VMWareTestSetup
    {
        [Test]
        public void TestTasks()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            // copy a batch file to the remote machine, execute it and collect results
            string localTempFilename = Path.GetTempFileName();
            string remoteTempFilename = string.Format(@"C:\{0}", Path.GetFileName(localTempFilename));
            string remoteBatFilename = string.Format(@"C:\{0}.bat", Path.GetFileNameWithoutExtension(localTempFilename));
            File.WriteAllText(localTempFilename, string.Format(@"dir C:\ > {0}", remoteTempFilename));
            virtualMachine.CopyFileFromHostToGuest(localTempFilename, remoteBatFilename);
            VMWareVirtualMachine.Process cmdProcess = virtualMachine.RunProgramInGuest(@"cmd.exe", string.Format("/C \"{0}\"", remoteBatFilename));
            Assert.IsNotNull(cmdProcess);
            Assert.AreEqual(0, cmdProcess.ExitCode);
            virtualMachine.CopyFileFromGuestToHost(remoteTempFilename, localTempFilename);
            string remoteDirectoryListing = File.ReadAllText(localTempFilename);
            Console.WriteLine(remoteDirectoryListing);
            Assert.IsTrue(remoteDirectoryListing.Contains(Path.GetFileName(remoteTempFilename)));
            Assert.IsTrue(remoteDirectoryListing.Contains(Path.GetFileName(remoteBatFilename)));
            // delete the temp files
            virtualMachine.DeleteFileFromGuest(remoteTempFilename);
            virtualMachine.DeleteFileFromGuest(remoteBatFilename);
            File.Delete(localTempFilename);
            // virtualMachine.PowerOff();
        }

        [Test, ExpectedException(typeof(VMWareException))]
        public void TestListDirectoryInGuestInvalidDirectory()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            virtualMachine.ListDirectoryInGuest(string.Format(@"C:\{0}", Guid.NewGuid()), false);
        }

        [Test]
        public void TestListDirectoryInGuestEmptyDirectory()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            // create an empty directory
            string directory = string.Format(@"C:\{0}", Guid.NewGuid());
            virtualMachine.CreateDirectoryInGuest(directory);
            try
            {
                // list the directory
                List<string> listOfEmptyDirectory = virtualMachine.ListDirectoryInGuest(directory, false);
                Assert.AreEqual(0, listOfEmptyDirectory.Count);
            }
            finally
            {
                virtualMachine.DeleteDirectoryFromGuest(directory);
            }
        }

        [Test]
        public void TestListDirectoryInGuest()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            List<string> listOfDrivers = virtualMachine.ListDirectoryInGuest(@"C:\WINDOWS\system32\drivers", false);
            List<string> listOfDriversWithSub = virtualMachine.ListDirectoryInGuest(@"C:\WINDOWS\system32\drivers", true);
            Assert.IsTrue(listOfDrivers.Count < listOfDriversWithSub.Count);
            Assert.IsTrue(listOfDriversWithSub.Contains(@"C:\WINDOWS\system32\drivers\etc\hosts"));
        }

        [Test]
        public void TestGetSetGuestVariables()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            string ip = virtualMachine.GuestVariables["ip"];
            Assert.IsFalse(string.IsNullOrEmpty(ip));
            Console.WriteLine("IP: {0}", ip);
            string guid = Guid.NewGuid().ToString();
            Assert.IsTrue(string.IsNullOrEmpty(virtualMachine.GuestVariables[guid]));
            virtualMachine.GuestVariables[guid] = guid;
            Assert.AreEqual(guid, virtualMachine.GuestVariables[guid]);
        }

        /// <summary>
        /// bug: this isn't working, the API doesn't do what it's supposed to do
        /// </summary>
        [Test]
        protected void TestGetSetGuestEnvironmentVariables()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            string guid = Guid.NewGuid().ToString();
            Assert.IsTrue(string.IsNullOrEmpty(virtualMachine.GuestEnvironmentVariables[guid]));
            virtualMachine.GuestVariables[guid] = guid;
            Assert.AreEqual(guid, virtualMachine.GuestEnvironmentVariables[guid]);
        }

        [Test]
        public void TestGetSetRuntimeConfigVariables()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            string displayName = virtualMachine.RuntimeConfigVariables["displayname"];
            Console.WriteLine("Display name: {0}", displayName);
            Assert.IsFalse(string.IsNullOrEmpty(displayName));
        }

        [Test]
        public void TestGetAddRemoveSharedFolders()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            try
            {
                int count = virtualMachine.SharedFolders.Count;
                Console.WriteLine("Shared folders: {0}", count);
                // add a shared folder
                VMWareSharedFolder currentDirectory = new VMWareSharedFolder(Guid.NewGuid().ToString(), Environment.CurrentDirectory);
                virtualMachine.SharedFolders.Add(currentDirectory);
                Assert.AreEqual(count + 1, virtualMachine.SharedFolders.Count);
                foreach (VMWareSharedFolder sharedFolder in virtualMachine.SharedFolders)
                {
                    Console.WriteLine("Shared folder: {0} ({1})",
                        sharedFolder.ShareName, sharedFolder.HostPath);
                }
                // remove the shared folder
                virtualMachine.SharedFolders.Remove(currentDirectory);
                Assert.AreEqual(count, virtualMachine.SharedFolders.Count);
            }
            catch (VMWareException ex)
            {
                switch (ex.ErrorCode)
                {
                    case Constants.VIX_E_NOT_SUPPORTED:
                        Assert.Ignore("Shared folders not supported on this VMWare platform.");
                        break;
                    default:
                        throw;
                }
            }
        }

        [Test]
        public void TestCaptureScreenImage()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            Image image = virtualMachine.CaptureScreenImage();
            Console.WriteLine("Image: {0}x{1}", image.Width, image.Height);
            Assert.IsTrue(image.Width > 0);
            Assert.IsTrue(image.Height > 0);
        }

        [Test]
        public void TestCreateDeleteDirectory()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            string directoryName = string.Format(@"c:\{0}", Guid.NewGuid());
            Assert.IsTrue(!virtualMachine.DirectoryExistsInGuest(directoryName));
            Assert.IsTrue(!virtualMachine.FileExistsInGuest(directoryName));
            virtualMachine.CreateDirectoryInGuest(directoryName);
            Assert.IsTrue(virtualMachine.DirectoryExistsInGuest(directoryName));
            Assert.IsTrue(!virtualMachine.FileExistsInGuest(directoryName));
            virtualMachine.DeleteDirectoryFromGuest(directoryName);
            Assert.IsTrue(!virtualMachine.DirectoryExistsInGuest(directoryName));
            Assert.IsTrue(!virtualMachine.FileExistsInGuest(directoryName));
        }

        [Test]
        public void TestCreateDeleteTempFile()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            string fileName = virtualMachine.CreateTempFileInGuest();
            Assert.IsFalse(string.IsNullOrEmpty(fileName));
            Console.WriteLine("Temp filename: {0}", fileName);
            Assert.IsTrue(!virtualMachine.DirectoryExistsInGuest(fileName));
            Assert.IsTrue(virtualMachine.FileExistsInGuest(fileName));
            virtualMachine.DeleteFileFromGuest(fileName);
            Assert.IsTrue(!virtualMachine.DirectoryExistsInGuest(fileName));
            Assert.IsTrue(!virtualMachine.FileExistsInGuest(fileName));
        }

        [Test]
        public void TestListAndKillProcesses()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            VMWareVirtualMachine.Process notepadProcess = virtualMachine.DetachProgramInGuest("notepad.exe");
            Console.WriteLine("Notepad.exe: {0}", notepadProcess.Id);
            Dictionary<long, VMWareVirtualMachine.Process> guestProcesses = virtualMachine.GuestProcesses;
            Assert.IsTrue(guestProcesses.ContainsKey(notepadProcess.Id));
            foreach (KeyValuePair<long, VMWareVirtualMachine.Process> process in guestProcesses)
            {
                Assert.IsTrue(process.Value.Id >= 0);
                Assert.IsFalse(string.IsNullOrEmpty(process.Value.Name));
                Console.WriteLine("{0}: {1} [{2}] ({3})", process.Value.Id, process.Value.Name, process.Value.Command, process.Value.Owner);
            }
            notepadProcess.KillProcessInGuest();
            Thread.Sleep(3000); // doc says: depending on the behavior of the guest operating system, there may be a short delay after the job completes before the process truly disappears
            Dictionary<long, VMWareVirtualMachine.Process> guestProcesses2 = virtualMachine.GuestProcesses;
            Assert.IsFalse(guestProcesses2.ContainsKey(notepadProcess.Id));
        }

        [Test]
        public void TestPowerOnPoweredHost()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            virtualMachine.PowerOn();
            virtualMachine.WaitForToolsInGuest();
            virtualMachine.PowerOn();
            virtualMachine.WaitForToolsInGuest();
            Assert.IsTrue(virtualMachine.IsRunning);
        }

        [Test]
        public void TestPauseUnpause()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            virtualMachine.Pause();
            Assert.AreEqual(true, virtualMachine.IsPaused);
            virtualMachine.Unpause();
            Assert.AreEqual(false, virtualMachine.IsPaused);
        }

        [Test]
        public void TestReset()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            // hardware reset
            Console.WriteLine("Reset ...");
            virtualMachine.Reset();
            Assert.AreEqual(true, virtualMachine.IsRunning);
            Console.WriteLine("Wait ...");
            virtualMachine.WaitForToolsInGuest();
        }

        [Test]
        public void TestSuspend()
        {
            VMWareVirtualMachine virtualMachine = VMWareTest.Instance.PoweredVirtualMachine;
            Console.WriteLine("Suspend ...");
            virtualMachine.Suspend();
            Assert.AreEqual(false, virtualMachine.IsPaused);
            Assert.AreEqual(true, virtualMachine.IsSuspended);
            Console.WriteLine("Power ...");
            virtualMachine.PowerOn();
            Console.WriteLine("Wait ...");
            virtualMachine.WaitForToolsInGuest();
        }
    }
}

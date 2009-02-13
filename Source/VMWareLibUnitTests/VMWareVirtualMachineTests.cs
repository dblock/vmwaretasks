using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;
using System.IO;
using System.Drawing;
using VixCOM;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualMachineTests
    {
        [Test]
        public void TestTasks()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
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
        }

        [Test, ExpectedException(typeof(VMWareException))]
        public void TestListDirectoryInGuestInvalidDirectory()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                virtualMachine.ListDirectoryInGuest(string.Format(@"C:\{0}", Guid.NewGuid()), false);
            }
        }

        [Test]
        public void TestListDirectoryInGuestEmptyDirectory()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
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
        }

        [Test]
        public void TestListDirectoryInGuest()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                List<string> listOfDrivers = virtualMachine.ListDirectoryInGuest(@"C:\WINDOWS\system32\drivers", false);
                List<string> listOfDriversWithSub = virtualMachine.ListDirectoryInGuest(@"C:\WINDOWS\system32\drivers", true);
                Assert.IsTrue(listOfDrivers.Count < listOfDriversWithSub.Count);
                Assert.IsTrue(listOfDriversWithSub.Contains(@"C:\WINDOWS\system32\drivers\etc\hosts"));
            }
        }

        [Test]
        public void TestGetSetGuestVariables()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                string ip = virtualMachine.GuestVariables["ip"];
                Assert.IsFalse(string.IsNullOrEmpty(ip));
                Console.WriteLine("IP: {0}", ip);
                string guid = Guid.NewGuid().ToString();
                Assert.IsTrue(string.IsNullOrEmpty(virtualMachine.GuestVariables[guid]));
                virtualMachine.GuestVariables[guid] = guid;
                Assert.AreEqual(guid, virtualMachine.GuestVariables[guid]);
            }
        }

        /// <summary>
        /// bug: this isn't working, the API doesn't do what it's supposed to do
        /// </summary>
        [Test]
        protected void TestGetSetGuestEnvironmentVariables()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                string guid = Guid.NewGuid().ToString();
                Assert.IsTrue(string.IsNullOrEmpty(virtualMachine.GuestEnvironmentVariables[guid]));
                virtualMachine.GuestVariables[guid] = guid;
                Assert.AreEqual(guid, virtualMachine.GuestEnvironmentVariables[guid]);
            }
        }

        [Test]
        public void TestGetSetRuntimeConfigVariables()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                string displayName = virtualMachine.RuntimeConfigVariables["displayname"];
                Console.WriteLine("Display name: {0}", displayName);
                Assert.IsFalse(string.IsNullOrEmpty(displayName));
            }
        }

        [Test]
        public void TestGetAddRemoveSharedFolders()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
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
        }

        [Test]
        public void TestCaptureScreenImage()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                Image image = virtualMachine.CaptureScreenImage();
                Console.WriteLine("Image: {0}x{1}", image.Width, image.Height);
                Assert.IsTrue(image.Width > 0);
                Assert.IsTrue(image.Height > 0);
            }
        }

        [Test]
        public void TestCreateDeleteDirectory()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
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
        }

        [Test]
        public void TestCreateDeleteTempFile()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                string fileName = virtualMachine.CreateTempFileInGuest();
                Assert.IsFalse(string.IsNullOrEmpty(fileName));
                Console.WriteLine("Temp filename: {0}", fileName);
                Assert.IsTrue(!virtualMachine.DirectoryExistsInGuest(fileName));
                Assert.IsTrue(virtualMachine.FileExistsInGuest(fileName));
                virtualMachine.DeleteFileFromGuest(fileName);
                Assert.IsTrue(!virtualMachine.DirectoryExistsInGuest(fileName));
                Assert.IsTrue(!virtualMachine.FileExistsInGuest(fileName));
            }
        }

        [Test]
        public void TestListAndKillProcesses()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
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
                Dictionary<long, VMWareVirtualMachine.Process> guestProcesses2 = virtualMachine.GuestProcesses;
                Assert.IsFalse(guestProcesses2.ContainsKey(notepadProcess.Id));
            }
        }
    }
}

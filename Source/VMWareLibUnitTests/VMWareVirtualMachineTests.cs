using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Text;
using Interop.VixCOM;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualMachineTests : VMWareUnitTest
    {
        [Test]
        public void TestTasks()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                // copy a batch file to the remote machine, execute it and collect results
                string localTempFilename = Path.GetTempFileName();
                string remoteTempFilename = string.Format(@"C:\{0}", Path.GetFileName(localTempFilename));
                string remoteBatFilename = string.Format(@"C:\{0}.bat", Path.GetFileNameWithoutExtension(localTempFilename));
                File.WriteAllText(localTempFilename, string.Format(@"dir C:\ > {0}", remoteTempFilename));
                virtualMachine.CopyFileFromHostToGuest(localTempFilename, remoteBatFilename);
                VMWareVirtualMachine.Process cmdProcess = virtualMachine.RunProgramInGuest(
                    "cmd.exe", string.Format("/C \"{0}\"", remoteBatFilename));
                Assert.IsNotNull(cmdProcess);
                Assert.AreEqual(0, cmdProcess.ExitCode);
                virtualMachine.CopyFileFromGuestToHost(remoteTempFilename, localTempFilename);
                string remoteDirectoryListing = File.ReadAllText(localTempFilename);
                ConsoleOutput.WriteLine(remoteDirectoryListing);
                Assert.IsTrue(remoteDirectoryListing.Contains(Path.GetFileName(remoteTempFilename)));
                Assert.IsTrue(remoteDirectoryListing.Contains(Path.GetFileName(remoteBatFilename)));
                // delete the temp files
                virtualMachine.DeleteFileFromGuest(remoteTempFilename);
                virtualMachine.DeleteFileFromGuest(remoteBatFilename);
                File.Delete(localTempFilename);
                // virtualMachine.PowerOff();
            }
        }

        [Test]
        public void TestListDirectoryInGuestInvalidDirectory()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                try
                {
                    virtualMachine.ListDirectoryInGuest(string.Format(@"C:\{0}", Guid.NewGuid()), false);
                    throw new Exception("Expected VMWareException");
                }
                catch (VMWareException)
                {
                    // expected
                }
            }
        }

        [Test]
        public void TestListDirectoryInGuestEmptyDirectory()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
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
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                string system32drivers = @"C:\WINDOWS\system32\drivers";
                List<string> listOfDrivers = virtualMachine.ListDirectoryInGuest(system32drivers, false);
                if (listOfDrivers.Count == 1 && listOfDrivers[0] == Path.Combine(system32drivers, "hfile.txt"))
                    Assert.Ignore("Remote file system is protected.");
                List<string> listOfDriversWithSub = virtualMachine.ListDirectoryInGuest(system32drivers, true);
                Assert.IsTrue(listOfDrivers.Count < listOfDriversWithSub.Count);
                Assert.IsTrue(listOfDriversWithSub.Contains(Path.Combine(system32drivers, @"etc\hosts")));
            }
        }

        [Test]
        public void TestGetSetGuestVariables()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                string ip = virtualMachine.GuestVariables["ip"];
                Assert.IsFalse(string.IsNullOrEmpty(ip));
                ConsoleOutput.WriteLine("IP: {0}", ip);
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
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
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
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                string displayName = virtualMachine.RuntimeConfigVariables["displayname"];
                ConsoleOutput.WriteLine("Display name: {0}", displayName);
                Assert.IsFalse(string.IsNullOrEmpty(displayName));
            }
        }

        [Test]
        public void TestGetDisplayName()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                string displayName = virtualMachine.Name;
                ConsoleOutput.WriteLine("Display name: {0}", displayName);
                Assert.IsFalse(string.IsNullOrEmpty(displayName));
            }
        }

        [Test]
        public void TestGetAddRemoveSharedFolders()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                try
                {
                    ConsoleOutput.WriteLine("Enabling shared folders ...");
                    virtualMachine.SharedFolders.Enabled = true;
                    int count = virtualMachine.SharedFolders.Count;
                    ConsoleOutput.WriteLine("Shared folders: {0}", count);
                    // add a shared folder
                    VMWareSharedFolder currentDirectory = new VMWareSharedFolder(
                        Guid.NewGuid().ToString(), Environment.CurrentDirectory);
                    virtualMachine.SharedFolders.Add(currentDirectory);
                    Assert.AreEqual(count + 1, virtualMachine.SharedFolders.Count);
                    foreach (VMWareSharedFolder sharedFolder in virtualMachine.SharedFolders)
                    {
                        ConsoleOutput.WriteLine("Shared folder: {0} ({1})",
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
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                Image image = virtualMachine.CaptureScreenImage();
                ConsoleOutput.WriteLine("Image: {0}x{1}", image.Width, image.Height);
                Assert.IsTrue(image.Width > 0);
                Assert.IsTrue(image.Height > 0);
            }
        }

        [Test]
        public void TestCreateDeleteDirectory()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
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
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                string fileName = virtualMachine.CreateTempFileInGuest();
                Assert.IsFalse(string.IsNullOrEmpty(fileName));
                ConsoleOutput.WriteLine("Temp filename: {0}", fileName);
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
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                VMWareVirtualMachine.Process notepadProcess = virtualMachine.DetachProgramInGuest("notepad.exe");
                ConsoleOutput.WriteLine("Notepad.exe: {0}", notepadProcess.Id);
                VMWareProcessCollection guestProcesses = virtualMachine.GuestProcesses;
                Assert.IsTrue(guestProcesses.ContainsKey(notepadProcess.Id));
                foreach (KeyValuePair<long, VMWareVirtualMachine.Process> process in guestProcesses)
                {
                    Assert.IsTrue(process.Value.Id >= 0);
                    Assert.IsFalse(string.IsNullOrEmpty(process.Value.Name));
                    ConsoleOutput.WriteLine("{0}: {1} [{2}] ({3})", process.Value.Id, process.Value.Name, process.Value.Command, process.Value.Owner);
                }
                notepadProcess.KillProcessInGuest();
                Thread.Sleep(3000); // doc says: depending on the behavior of the guest operating system, there may be a short delay after the job completes before the process truly disappears
                VMWareProcessCollection guestProcesses2 = virtualMachine.GuestProcesses;
                Assert.IsFalse(guestProcesses2.ContainsKey(notepadProcess.Id));
            }
        }

        [Test]
        public void TestPowerOnPoweredHost()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                virtualMachine.PowerOn();
                virtualMachine.WaitForToolsInGuest();
                virtualMachine.PowerOn();
                virtualMachine.WaitForToolsInGuest();
                Assert.IsTrue(virtualMachine.IsRunning);
            }
        }

        [Test]
        public void TestPauseUnpause()
        {
            if (!_test.Config.RunWorkstationTests)
                Assert.Ignore("Skipping, Workstation tests disabled.");

            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                virtualMachine.Pause();
                Assert.AreEqual(true, virtualMachine.IsPaused);
                virtualMachine.Unpause();
                Assert.AreEqual(false, virtualMachine.IsPaused);
            }
        }

        [Test]
        protected void TestRunScriptInGuest()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                StringBuilder script = new StringBuilder();
                script.AppendLine("print \"Hello World\";");
                VMWareVirtualMachine.Process cmdProcess = virtualMachine.RunScriptInGuest(@"c:\perl\bin\perl.exe", script.ToString());
                Assert.IsNotNull(cmdProcess);
                Assert.AreEqual(0, cmdProcess.ExitCode);
            }
        }

        [Test]
        public void TestOpenUrlInGuest()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                virtualMachine.OpenUrlInGuest("http://vmwaretasks.codeplex.com/");
            }
        }

        [Test]
        public void TestGetFolderInfoInGuest()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                string tmpPath = virtualMachine.GuestEnvironmentVariables["tmp"];
                VMWareVirtualMachine.GuestFileInfo tmpPathInfo = virtualMachine.GetFileInfoInGuest(tmpPath);
                ConsoleOutput.WriteLine("{0}: {1}, {2} byte(s)",
                    tmpPathInfo.GuestPathName,
                    tmpPathInfo.LastModified,
                    tmpPathInfo.FileSize);
                Assert.AreEqual(0, tmpPathInfo.FileSize);
                Assert.AreEqual(tmpPath, tmpPathInfo.GuestPathName);
                Assert.AreEqual(true, tmpPathInfo.IsDirectory);
                Assert.AreEqual(false, tmpPathInfo.IsSymLink);
                Assert.IsTrue(tmpPathInfo.LastModified > DateTime.MinValue);
            }
        }

        [Test]
        public void TestGetFileInfoInGuest()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                string hostTmpFilename = Path.GetTempFileName();
                File.WriteAllText(hostTmpFilename, Guid.NewGuid().ToString());
                FileInfo hostTmpFileInfo = new FileInfo(hostTmpFilename);
                string guestTmpFilename = virtualMachine.CreateTempFileInGuest();
                DateTime dtBeforeCopy = DateTime.Now;
                virtualMachine.CopyFileFromHostToGuest(hostTmpFilename, guestTmpFilename);
                VMWareVirtualMachine.GuestFileInfo tmpPathInfo = virtualMachine.GetFileInfoInGuest(guestTmpFilename);
                ConsoleOutput.WriteLine("{0}: {1}, {2} byte(s)",
                    tmpPathInfo.GuestPathName,
                    tmpPathInfo.LastModified,
                    tmpPathInfo.FileSize);
                Assert.AreEqual(hostTmpFileInfo.Length, tmpPathInfo.FileSize);
                Assert.AreEqual(guestTmpFilename, tmpPathInfo.GuestPathName);
                Assert.AreEqual(false, tmpPathInfo.IsDirectory);
                Assert.AreEqual(false, tmpPathInfo.IsSymLink);
                Assert.IsTrue(tmpPathInfo.LastModified >= dtBeforeCopy);
                virtualMachine.DeleteFileFromGuest(guestTmpFilename);
                File.Delete(hostTmpFilename);
            }
        }

        [Test]
        protected void TestInstallTools()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                virtualMachine.InstallTools();
            }
        }

        [Test]
        public void TestLogInInteractivelyAfterWaitingForVMwareUserProcess()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                VMWareVirtualMachineConfig config = _test.GetConfiguration(virtualMachine);

                Assert.IsNotNull(config);

                virtualMachine.WaitForVMWareUserProcessInGuest(
                    config.GuestUsername, 
                    config.GuestPassword, 
                    VMWareInterop.Timeouts.WaitForToolsTimeout);

                virtualMachine.LoginInGuest(
                    config.GuestUsername, 
                    config.GuestPassword,
                    Constants.VIX_LOGIN_IN_GUEST_REQUIRE_INTERACTIVE_ENVIRONMENT,
                    VMWareInterop.Timeouts.LoginTimeout);
            }
        }
    }
}

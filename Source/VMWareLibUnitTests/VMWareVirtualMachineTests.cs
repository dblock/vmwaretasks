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
            // copy a batch file to the remote machine, execute it and collect results
            string localTempFilename = Path.GetTempFileName();
            string remoteTempFilename = string.Format(@"C:\{0}", Path.GetFileName(localTempFilename));
            string remoteBatFilename = string.Format(@"C:\{0}.bat", Path.GetFileNameWithoutExtension(localTempFilename));
            File.WriteAllText(localTempFilename, string.Format(@"dir C:\ > {0}", remoteTempFilename));
            virtualMachine.CopyFileFromHostToGuest(localTempFilename, remoteBatFilename);
            Assert.AreEqual(0, virtualMachine.Execute(@"cmd.exe", string.Format("/C \"{0}\"", remoteBatFilename)));
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
        public void TestWorkstationListDirectoryInGuestInvalidDirectory()
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
            // list directories 
            virtualMachine.ListDirectoryInGuest(string.Format(@"C:\{0}", Guid.NewGuid()), false);
        }

        [Test]
        public void TestWorkstationListDirectoryInGuestEmptyDirectory()
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
            // list directories 
            List<string> listOfInetPubBadMail = virtualMachine.ListDirectoryInGuest(@"C:\Inetpub\mailroot\Badmail", false);
            Assert.AreEqual(0, listOfInetPubBadMail.Count);
        }

        [Test]
        public void TestWorkstationListDirectoryInGuest()
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
            // list directories 
            List<string> listOfInetPub = virtualMachine.ListDirectoryInGuest(@"C:\Inetpub", false);
            List<string> listOfInetPubWithSub = virtualMachine.ListDirectoryInGuest(@"C:\Inetpub", true);
            Assert.AreEqual(0, listOfInetPub.Count);
            Assert.IsTrue(listOfInetPub.Count < listOfInetPubWithSub.Count);
            Assert.IsTrue(listOfInetPubWithSub.Contains(@"C:\Inetpub\AdminScripts\adsutil.vbs"));
        }
    }
}

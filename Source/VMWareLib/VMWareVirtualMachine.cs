using System;
using System.Collections.Generic;
using System.IO;
using VixCOM;
using System.Runtime.InteropServices;

namespace Vestris.VMWareLib
{
    public class VMWareVirtualMachine
    {
        private IVM _vm = null;

        public VMWareVirtualMachine(IVM vm)
        {
            _vm = vm;
        }

        /// <summary>
        /// Power on a virtual machine.
        /// </summary>
        public void PowerOn()
        {
            PowerOn(VMWareTimeouts.defaultPowerOnTimeout);
        }

        /// <summary>
        /// Power on a virtual machine.
        /// </summary>
        /// <param name="timeoutInSeconds">timeout</param>
        public void PowerOn(int timeoutInSeconds)
        {
            PowerOn(Constants.VIX_VMPOWEROP_NORMAL | Constants.VIX_VMPOWEROP_LAUNCH_GUI,
                timeoutInSeconds);
        }

        /// <summary>
        /// Power on a virtual machine.
        /// </summary>
        /// <param name="powerOnOptions">additional options</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void PowerOn(int powerOnOptions, int timeoutInSeconds)
        {
            VMWareJob powerOnJob = new VMWareJob(_vm.PowerOn(powerOnOptions, null, null));
            powerOnJob.Wait(timeoutInSeconds);
            // wait till the machine boots or times out with an error
            VMWareJob waitForToolsInGuestJob = new VMWareJob(_vm.WaitForToolsInGuest(timeoutInSeconds, null));
            waitForToolsInGuestJob.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Find a snapshot.
        /// </summary>
        /// <param name="name">snapshot name</param>
        /// <returns>a snapshot</returns>
        public VMWareSnapshot FindSnapshot(string name)
        {
            ISnapshot snapshot = null;
            VMWareInterop.Check(_vm.GetNamedSnapshot(name, out snapshot));
            return new VMWareSnapshot(_vm, snapshot);
        }

        /// <summary>
        /// Current snapshot.
        /// </summary>
        /// <returns>current snapshot</returns>
        public VMWareSnapshot GetCurrentSnapshot()
        {
            ISnapshot snapshot = null;
            VMWareInterop.Check(_vm.GetCurrentSnapshot(out snapshot));
            return new VMWareSnapshot(_vm, snapshot);
        }

        /// <summary>
        /// Get all snapshots.
        /// </summary>
        /// <returns>a list of snapshots</returns>
        public List<VMWareSnapshot> GetSnapshots()
        {
            List<VMWareSnapshot> snapshots = new List<VMWareSnapshot>();
            int nSnapshots = 0;
            VMWareInterop.Check(_vm.GetNumRootSnapshots(out nSnapshots));
            for (int i = 0; i < nSnapshots; i++)
            {
                ISnapshot snapshot = null;
                VMWareInterop.Check(_vm.GetRootSnapshot(i, out snapshot));
                snapshots.Add(new VMWareSnapshot(_vm, snapshot));
            }
            return snapshots;
        }

        /// <summary>
        /// This function establishes a guest operating system authentication context that can be used 
        /// with guest functions for the given virtual machine handle. 
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        public void Login(string username, string password)
        {
            Login(username, password, VMWareTimeouts.defaultLoginTimeout);
        }

        /// <summary>
        /// This function establishes a guest operating system authentication context that can be used 
        /// with guest functions for the given virtual machine handle. 
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        public void Login(string username, string password, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.LoginInGuest(username, password, 0, null));
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Copies a file or directory from the local system (where the Vix client is running) to the guest operating system.
        /// </summary>
        public void CopyFileFromHostToGuest(string hostPathName, string guestPathName)
        {
            CopyFileFromHostToGuest(hostPathName, guestPathName, VMWareTimeouts.defaultCopyFileTimeout);
        }

        /// <summary>
        /// Copies a file or directory from the local system (where the Vix client is running) to the guest operating system.
        /// You must call LoginInGuest() before calling this procedure.
        /// Only absolute paths should be used for files in the guest; the resolution of relative paths is not specified.
        /// </summary>
        public void CopyFileFromHostToGuest(string hostPathName, string guestPathName, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.CopyFileFromHostToGuest(hostPathName, guestPathName, 0, null, null));
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Deletes a file from guest file system.
        /// </summary>
        public void DeleteFileFromGuest(string guestPathName)
        {
            DeleteFileFromGuest(guestPathName, VMWareTimeouts.defaultDeleteTimeout);
        }

        /// <summary>
        /// Deletes a file from guest file system.
        /// </summary>
        public void DeleteFileFromGuest(string guestPathName, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.DeleteFileInGuest(guestPathName, null));
            job.Wait(timeoutInSeconds);
        }


        /// <summary>
        /// Copies a file or directory from the guest operating system to the local system (where the Vix client is running).
        /// </summary>
        public void CopyFileFromGuestToHost(string guestPathName, string hostPathName)
        {
            CopyFileFromGuestToHost(guestPathName, hostPathName, VMWareTimeouts.defaultCopyFileTimeout);
        }

        /// <summary>
        /// Copies a file or directory from the guest operating system to the local system (where the Vix client is running).
        /// You must call LoginInGuest() before calling this procedure.
        /// Only absolute paths should be used for files in the guest; the resolution of relative paths is not specified. 
        /// </summary>
        public void CopyFileFromGuestToHost(string guestPathName, string hostPathName, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.CopyFileFromGuestToHost(guestPathName, hostPathName, 0, null, null));
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Runs a program on the VM
        /// </summary>
        public int Execute(string path, string parameters)
        {
            return Execute(path, parameters, VMWareTimeouts.defaultExecuteTimeout);
        }

        /// <summary>
        /// Runs a program on the VM
        /// </summary>
        public int Execute(string path, string parameters, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.RunProgramInGuest(path, parameters, VixCOM.Constants.VIX_RUNPROGRAM_ACTIVATE_WINDOW, null, null));
            object[] properties = { Constants.VIX_PROPERTY_JOB_RESULT_GUEST_PROGRAM_EXIT_CODE };
            return job.Wait<int>(properties, 0, timeoutInSeconds);
        }

        /// <summary>
        /// This function tests the existence of a file in the guest operating system.
        /// </summary>
        public bool FileExistsInGuest(string guestPathName)
        {
            return FileExistsInGuest(guestPathName, VMWareTimeouts.defaultFileExistsTimeout);
        }

        /// <summary>
        /// This function tests the existence of a file in the guest operating system.
        /// </summary>
        public bool FileExistsInGuest(string guestPathName, int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.FileExistsInGuest(guestPathName, null));
            object[] properties = { Constants.VIX_PROPERTY_JOB_RESULT_GUEST_OBJECT_EXISTS };
            return job.Wait<bool>(properties, 0, timeoutInSeconds);
        }

        /// <summary>
        /// This function removes any guest operating system authentication context created by a previous call to LoginInGuest(). 
        /// </summary>
        public void Logout()
        {
            Logout(VMWareTimeouts.defaultLogoutTimeout);
        }

        /// <summary>
        /// This function removes any guest operating system authentication context created by a previous call to LoginInGuest(). 
        /// </summary>
        public void Logout(int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.LogoutFromGuest(null));
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Powers off a virtual machine.
        /// </summary>
        public void PowerOff()
        {
            PowerOff(VMWareTimeouts.defaultPowerOffTimeout);
        }

        /// <summary>
        /// Powers off a virtual machine.
        /// </summary>
        public void PowerOff(int timeoutInSeconds)
        {
            VMWareJob job = new VMWareJob(_vm.PowerOff(VixCOM.Constants.VIX_VMPOWEROP_NORMAL, null));
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Lists files in a remote path.
        /// </summary>
        public List<string> ListDirectoryInGuest(string pathName, bool recurse)
        {
            return ListDirectoryInGuest(pathName, recurse, VMWareTimeouts.defaultListDirectoryInGuestTimeout);
        }

        /// <summary>
        /// Lists files in a remote path.
        /// </summary>
        public List<string> ListDirectoryInGuest(string pathName, bool recurse, int timeoutInSeconds)
        {
            List<string> results = new List<string>();
            VMWareJob job = new VMWareJob(_vm.ListDirectoryInGuest(pathName, 0, null));

            object[] properties = 
                { 
                    Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME, 
                    Constants.VIX_PROPERTY_JOB_RESULT_FILE_FLAGS
                };

            try
            {
                object[] propertyValues = job.Wait<object[]>(properties, timeoutInSeconds);
                int count = job.GetNumProperties(Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME);
                for (int i = 0; i < count; i++)
                {
                    object[] fileProperties = job.GetNthProperties<object[]>(i, properties);
                    string fileName = (string)fileProperties[0];
                    int flags = (int)fileProperties[1];

                    if ((flags & 1) > 0)
                    {
                        if (recurse)
                        {
                            results.AddRange(ListDirectoryInGuest(Path.Combine(pathName, fileName), true, timeoutInSeconds));
                        }
                    }
                    else
                    {
                        results.Add(Path.Combine(pathName, fileName));
                    }
                }
            }
            catch (VMWareException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 6000:
                        // invalid properties, the directory exists, but contains no files
                        break;
                    default:
                        throw;
                }
            }

            return results;
        }
    }
}

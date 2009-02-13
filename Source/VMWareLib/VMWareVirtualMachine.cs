using System;
using System.Collections.Generic;
using System.IO;
using VixCOM;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Vestris.VMWareLib
{
    public class VMWareVirtualMachine
    {
        /// <summary>
        /// An indexer for variables
        /// </summary>
        public class VariableIndexer
        {
            private IVM _vm;
            private int _variableType;

            /// <summary>
            /// A variables indexer
            /// </summary>
            /// <param name="vm">virtual machine's variables to index</param>
            /// <param name="variableType">variable type, VixCOM.Constants.VIX_VM_GUEST_VARIABLE, VIX_VM_CONFIG_RUNTIME_ONLY or VIX_GUEST_ENVIRONMENT_VARIABLE</param>
            public VariableIndexer(IVM vm, int variableType)
            {
                _vm = vm;
                _variableType = variableType;
            }

            /// <summary>
            /// Environment, guest and runtime variables
            /// </summary>
            /// <param name="name">name of the variable</param>
            [IndexerName("Variables")]
            public string this[string name]
            {
                get
                {
                    VMWareJob job = new VMWareJob(_vm.ReadVariable(_variableType, name, 0, null));
                    object[] properties = { Constants.VIX_PROPERTY_JOB_RESULT_VM_VARIABLE_STRING };
                    return job.Wait<string>(properties, 0, VMWareInterop.Timeouts.ReadVariableTimeout);
                }
                set
                {
                    VMWareJob job = new VMWareJob(_vm.WriteVariable(_variableType, name, value, 0, null));
                    job.Wait(VMWareInterop.Timeouts.WriteVariableTimeout);
                }
            }
        }

        private IVM _vm = null;
        private VariableIndexer _guestEnvironmentVariables = null;
        private VariableIndexer _runtimeConfigVariables = null;
        private VariableIndexer _guestVariables = null;

        public VMWareVirtualMachine(IVM vm)
        {
            _vm = vm;
            _guestEnvironmentVariables = new VariableIndexer(_vm, Constants.VIX_GUEST_ENVIRONMENT_VARIABLE);
            _runtimeConfigVariables = new VariableIndexer(_vm, Constants.VIX_VM_CONFIG_RUNTIME_ONLY);
            _guestVariables = new VariableIndexer(_vm, Constants.VIX_VM_GUEST_VARIABLE);
        }

        /// <summary>
        /// Power on a virtual machine.
        /// </summary>
        public void PowerOn()
        {
            PowerOn(VMWareInterop.Timeouts.PowerOnTimeout);
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
            Login(username, password, VMWareInterop.Timeouts.LoginTimeout);
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
            CopyFileFromHostToGuest(hostPathName, guestPathName, VMWareInterop.Timeouts.CopyFileTimeout);
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
            DeleteFileFromGuest(guestPathName, VMWareInterop.Timeouts.DeleteTimeout);
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
            CopyFileFromGuestToHost(guestPathName, hostPathName, VMWareInterop.Timeouts.CopyFileTimeout);
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
            return Execute(path, parameters, VMWareInterop.Timeouts.ExecuteTimeout);
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
            return FileExistsInGuest(guestPathName, VMWareInterop.Timeouts.FileExistsTimeout);
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
            Logout(VMWareInterop.Timeouts.LogoutTimeout);
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
            PowerOff(VMWareInterop.Timeouts.PowerOffTimeout);
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
            return ListDirectoryInGuest(pathName, recurse, VMWareInterop.Timeouts.ListDirectoryInGuestTimeout);
        }

        /// <summary>
        /// Lists files in a remote path, with or without subdirectories.
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

        /// <summary>
        /// An environment variable in the guest of the VM. On a Windows NT series guest, writing these 
        /// values is saved persistently so they are immediately visible to every process. On a Linux or Windows 9X guest, 
        /// writing these values is not persistent so they are only visible to the VMware tools process. 
        /// </summary>
        public VariableIndexer GuestEnvironmentVariables
        {
            get 
            { 
                return _guestEnvironmentVariables; 
            }
        }

        /// <summary>
        /// A "Guest Variable". This is a runtime-only value; it is never stored persistently. 
        /// This is the same guest variable that is exposed through the VMControl APIs, and is a simple 
        /// way to pass runtime values in and out of the guest. 
        /// </summary>
        public VariableIndexer GuestVariables
        {
            get
            {
                return _guestVariables;
            }
        }

        /// <summary>
        /// The configuration state of the virtual machine. This is the .vmx file that is stored on the host. 
        /// You can read this and it will return the persistent data. If you write to this, it will only be a 
        /// runtime change, so changes will be lost when the VM powers off. 
        /// </summary>
        public VariableIndexer RuntimeConfigVariables
        {
            get
            {
                return _runtimeConfigVariables;
            }
        }
    }
}

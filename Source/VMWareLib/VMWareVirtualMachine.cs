using System;
using System.Collections.Generic;
using System.IO;
using VixCOM;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VMWare Virtual Machine.
    /// </summary>
    public class VMWareVirtualMachine : VMWareVixHandle<IVM2>
    {
        /// <summary>
        /// An indexer for variables
        /// </summary>
        public class VariableIndexer
        {
            private IVM2 _handle;
            private int _variableType;

            /// <summary>
            /// A variables indexer
            /// </summary>
            /// <param name="vm">virtual machine's variables to index</param>
            /// <param name="variableType">variable type, Constants.VIX_VM_GUEST_VARIABLE, VIX_VM_CONFIG_RUNTIME_ONLY or VIX_GUEST_ENVIRONMENT_VARIABLE</param>
            public VariableIndexer(IVM2 vm, int variableType)
            {
                _handle = vm;
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
                    VMWareJobCallback callback = new VMWareJobCallback();
                    VMWareJob job = new VMWareJob(_handle.ReadVariable(
                        _variableType, name, 0, callback),
                        callback);
                    return job.Wait<string>(
                        Constants.VIX_PROPERTY_JOB_RESULT_VM_VARIABLE_STRING,
                        VMWareInterop.Timeouts.ReadVariableTimeout);
                }
                set
                {
                    VMWareJobCallback callback = new VMWareJobCallback();
                    VMWareJob job = new VMWareJob(_handle.WriteVariable(
                        _variableType, name, value, 0, callback),
                        callback);
                    job.Wait(VMWareInterop.Timeouts.WriteVariableTimeout);
                }
            }
        }

        /// <summary>
        /// A process running in the guest operating system.
        /// </summary>
        public class Process
        {
            /// <summary>
            /// Process ID.
            /// </summary>
            public long Id;
            /// <summary>
            /// Process name.
            /// </summary>
            public string Name;
            /// <summary>
            /// Process owner.
            /// </summary>
            public string Owner;
            /// <summary>
            /// Process start date/time.
            /// </summary>
            public DateTime StartDateTime;
            /// <summary>
            /// Process command line.
            /// </summary>
            public string Command;
            /// <summary>
            /// True if process is being debugged.
            /// </summary>
            public bool IsBeingDebugged = false;
            /// <summary>
            /// Process exit code for finished processes.
            /// </summary>
            public int ExitCode = 0;

            private IVM2 _vm;

            /// <summary>
            /// A process running in the guest operating system on a virtual machine.
            /// </summary>
            /// <param name="vm">virtual machine</param>
            public Process(IVM2 vm)
            {
                _vm = vm;
            }

            /// <summary>
            /// Kill a process in the guest operating system.
            /// </summary>
            public void KillProcessInGuest()
            {
                KillProcessInGuest(VMWareInterop.Timeouts.KillProcessTimeout);
            }

            /// <summary>
            /// Kill a process in the guest operating system.
            /// </summary>
            /// <param name="timeoutInSeconds">timeout in seconds</param>
            public void KillProcessInGuest(int timeoutInSeconds)
            {
                VMWareJobCallback callback = new VMWareJobCallback();
                VMWareJob job = new VMWareJob(_vm.KillProcessInGuest(
                    Convert.ToUInt64(Id), 0, callback),
                    callback);
                job.Wait(timeoutInSeconds);
            }
        }

        private VariableIndexer _guestEnvironmentVariables = null;
        private VariableIndexer _runtimeConfigVariables = null;
        private VariableIndexer _guestVariables = null;
        private VMWareRootSnapshotCollection _snapshots = null;
        private VMWareSharedFolderCollection _sharedFolders = null;

        /// <summary>
        /// A VMWare Virtual Machine.
        /// </summary>
        /// <param name="vm">a handle to a virtual machine</param>
        public VMWareVirtualMachine(IVM2 vm)
            : base(vm)
        {
            _guestEnvironmentVariables = new VariableIndexer(_handle, Constants.VIX_GUEST_ENVIRONMENT_VARIABLE);
            _runtimeConfigVariables = new VariableIndexer(_handle, Constants.VIX_VM_CONFIG_RUNTIME_ONLY);
            _guestVariables = new VariableIndexer(_handle, Constants.VIX_VM_GUEST_VARIABLE);
            _sharedFolders = new VMWareSharedFolderCollection(_handle);
            _snapshots = new VMWareRootSnapshotCollection(_handle);
        }


        /// <summary>
        /// The path to the virtual machine configuration file.
        /// </summary>
        public string PathName
        {
            get
            {
                return GetProperty<string>(Constants.VIX_PROPERTY_VM_VMX_PATHNAME);
            }
        }

        /// <summary>
        /// Returns true if the virtual machine is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return GetProperty<bool>(Constants.VIX_PROPERTY_VM_IS_RUNNING);
            }
        }

        /// <summary>
        /// The memory size of the virtual machine. 
        /// </summary>
        public int MemorySize
        {
            get
            {
                return GetProperty<int>(Constants.VIX_PROPERTY_VM_MEMORY_SIZE);
            }
        }

        /// <summary>
        /// The number of virtual CPUs configured for the virtual machine.
        /// </summary>
        public int CPUCount
        {
            get
            {
                return GetProperty<int>(Constants.VIX_PROPERTY_VM_NUM_VCPUS);
            }
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
            VMWareJobCallback powerOnCallback = new VMWareJobCallback();
            VMWareJob powerOnJob = new VMWareJob(_handle.PowerOn(
                powerOnOptions, null, powerOnCallback),
                powerOnCallback);
            powerOnJob.Wait(timeoutInSeconds);
            // wait till the machine boots or times out with an error
            VMWareJobCallback waitForToolsCallback = new VMWareJobCallback();
            VMWareJob waitForToolsInGuestJob = new VMWareJob(
                _handle.WaitForToolsInGuest(timeoutInSeconds, waitForToolsCallback), 
                waitForToolsCallback);
            waitForToolsInGuestJob.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Get all snapshots.
        /// </summary>
        /// <returns>A list of snapshots.</returns>
        public VMWareRootSnapshotCollection Snapshots
        {
            get
            {
                return _snapshots;
            }
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
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        public void Login(string username, string password, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.LoginInGuest(
                username, password, 0, callback),
                callback);
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
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.CopyFileFromHostToGuest(
                hostPathName, guestPathName, 0, null, callback), callback);
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Deletes a file from guest file system.
        /// </summary>
        public void DeleteFileFromGuest(string guestPathName)
        {
            DeleteFileFromGuest(guestPathName, VMWareInterop.Timeouts.DeleteFileTimeout);
        }

        /// <summary>
        /// Deletes a file from guest file system.
        /// </summary>
        public void DeleteFileFromGuest(string guestPathName, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.DeleteFileInGuest(
                guestPathName, callback), 
                callback);
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Deletes a directory from guest directory system.
        /// </summary>
        public void DeleteDirectoryFromGuest(string guestPathName)
        {
            DeleteDirectoryFromGuest(guestPathName, VMWareInterop.Timeouts.DeleteDirectoryTimeout);
        }

        /// <summary>
        /// Deletes a directory from guest directory system.
        /// </summary>
        public void DeleteDirectoryFromGuest(string guestPathName, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.DeleteDirectoryInGuest(
                guestPathName, 0, callback),
                callback);
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
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.CopyFileFromGuestToHost(
                guestPathName, hostPathName, 0, null, callback),
                callback);
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Creates a directory on the guest operating system.
        /// </summary>
        public void CreateDirectoryInGuest(string guestPathName)
        {
            CreateDirectoryInGuest(guestPathName, VMWareInterop.Timeouts.CreateDirectoryTimeout);
        }

        /// <summary>
        /// Creates a directory on the guest operating system.
        /// </summary>
        public void CreateDirectoryInGuest(string guestPathName, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.CreateDirectoryInGuest(
                guestPathName, null, callback),
                callback);
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Creates a temp file on the guest operating system.
        /// </summary>
        public string CreateTempFileInGuest()
        {
            return CreateTempFileInGuest(VMWareInterop.Timeouts.CreateTempFileTimeout);
        }

        /// <summary>
        /// Creates a temp file on the guest operating system.
        /// </summary>
        public string CreateTempFileInGuest(int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.CreateTempFileInGuest(
                0, null, callback),
                callback);
            return job.Wait<string>(Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME, timeoutInSeconds);
        }

        /// <summary>
        /// Runs a program in the guest operating system.
        /// </summary>       
        /// <returns>Process information.</returns>
        public Process RunProgramInGuest(string guestProgramName)
        {
            return RunProgramInGuest(guestProgramName, string.Empty);
        }

        /// <summary>
        /// Run a program in the guest operating system.
        /// </summary>
        /// <param name="commandLineArgs">additional command line arguments</param>
        /// <param name="guestProgramName">program to execute</param>
        /// <returns>Process information.</returns>
        public Process RunProgramInGuest(string guestProgramName, string commandLineArgs)
        {
            return RunProgramInGuest(guestProgramName, commandLineArgs,
                Constants.VIX_RUNPROGRAM_ACTIVATE_WINDOW,
                VMWareInterop.Timeouts.RunProgramTimeout);
        }

        /// <summary>
        /// Run a detached program in the guest operating system.
        /// </summary>
        /// <param name="guestProgramName">program to execute</param>
        /// <returns>Process information.</returns>
        public Process DetachProgramInGuest(string guestProgramName)
        {
            return DetachProgramInGuest(guestProgramName, string.Empty);
        }

        /// <summary>
        /// Run a detached program in the guest operating system.
        /// </summary>
        /// <param name="commandLineArgs">additional command line arguments</param>
        /// <param name="guestProgramName">program to execute</param>
        /// <returns>Process information.</returns>
        public Process DetachProgramInGuest(string guestProgramName, string commandLineArgs)
        {
            return RunProgramInGuest(guestProgramName, commandLineArgs,
                Constants.VIX_RUNPROGRAM_ACTIVATE_WINDOW | Constants.VIX_RUNPROGRAM_RETURN_IMMEDIATELY,
                VMWareInterop.Timeouts.RunProgramTimeout);
        }

        /// <summary>
        /// Run a program in the guest operating system.
        /// </summary>
        /// <param name="guestProgramName">guest program to run</param>
        /// <param name="commandLineArgs">additional command line arguments</param>
        /// <param name="options">additional options, one of VIX_RUNPROGRAM_RETURN_IMMEDIATELY or VIX_RUNPROGRAM_ACTIVATE_WINDOW</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        /// <returns>Process information.</returns>
        public Process RunProgramInGuest(string guestProgramName, string commandLineArgs, int options, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.RunProgramInGuest(
                guestProgramName, commandLineArgs, options, null, callback),
                callback);
            object[] properties = 
            { 
                Constants.VIX_PROPERTY_JOB_RESULT_GUEST_PROGRAM_EXIT_CODE, 
                Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_ID
            };
            object[] propertyValues = job.Wait<object[]>(properties, timeoutInSeconds);
            Process process = new Process(_handle);
            process.Name = Path.GetFileName(guestProgramName);
            process.Command = guestProgramName;
            if (!string.IsNullOrEmpty(commandLineArgs))
            {
                process.Command += " ";
                process.Command += commandLineArgs;
            }
            process.ExitCode = (int)propertyValues[0];
            process.Id = (long)propertyValues[1];
            return process;
        }

        /// <summary>
        /// Tests the existence of a file in the guest operating system.
        /// </summary>
        public bool FileExistsInGuest(string guestPathName)
        {
            return FileExistsInGuest(guestPathName, VMWareInterop.Timeouts.FileExistsTimeout);
        }

        /// <summary>
        /// Tests the existence of a file in the guest operating system.
        /// </summary>
        public bool FileExistsInGuest(string guestPathName, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.FileExistsInGuest(
                guestPathName, callback),
                callback);
            return job.Wait<bool>(Constants.VIX_PROPERTY_JOB_RESULT_GUEST_OBJECT_EXISTS, timeoutInSeconds);
        }

        /// <summary>
        /// Tests the existence of a directory in the guest operating system.
        /// </summary>
        public bool DirectoryExistsInGuest(string guestPathName)
        {
            return DirectoryExistsInGuest(guestPathName, VMWareInterop.Timeouts.DirectoryExistsTimeout);
        }

        /// <summary>
        /// Tests the existence of a directory in the guest operating system.
        /// </summary>
        public bool DirectoryExistsInGuest(string guestPathName, int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.DirectoryExistsInGuest(
                guestPathName, callback),
                callback);
            return job.Wait<bool>(Constants.VIX_PROPERTY_JOB_RESULT_GUEST_OBJECT_EXISTS, timeoutInSeconds);
        }

        /// <summary>
        /// Remove any guest operating system authentication context created by a previous call to LoginInGuest(), ie. Logout.
        /// </summary>
        public void Logout()
        {
            Logout(VMWareInterop.Timeouts.LogoutTimeout);
        }

        /// <summary>
        /// Remove any guest operating system authentication context created by a previous call to LoginInGuest(), ie. Logout.
        /// </summary>
        public void Logout(int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.LogoutFromGuest(callback), callback);
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// Power off a virtual machine.
        /// </summary>
        public void PowerOff()
        {
            PowerOff(VMWareInterop.Timeouts.PowerOffTimeout);
        }

        /// <summary>
        /// Power off a virtual machine.
        /// </summary>
        public void PowerOff(int timeoutInSeconds)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.PowerOff(
                Constants.VIX_VMPOWEROP_NORMAL, callback),
                callback);
            job.Wait(timeoutInSeconds);
        }

        /// <summary>
        /// List files in the guest operating system.
        /// </summary>
        /// <param name="pathName">path in the guest operating system to list</param>
        /// <param name="recurse">recruse into subdirectories</param>
        public List<string> ListDirectoryInGuest(string pathName, bool recurse)
        {
            return ListDirectoryInGuest(pathName, recurse, VMWareInterop.Timeouts.ListDirectoryTimeout);
        }

        /// <summary>
        /// List files in the guest operating system.
        /// </summary>
        /// <param name="pathName">path in the guest operating system to list</param>
        /// <param name="recurse">recruse into subdirectories</param>
        /// <param name="timeoutInSeconds">timeout in seconds</param>
        /// <remarks>
        /// This function behaves differently on VMWare Workstation (returns empty list) and 
        /// ESX (throws an exception) for directories or files that don't exist.
        /// </remarks>
        public List<string> ListDirectoryInGuest(string pathName, bool recurse, int timeoutInSeconds)
        {
            List<string> results = new List<string>();
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.ListDirectoryInGuest(
                pathName, 0, callback), callback);

            object[] properties = 
            { 
                Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME, 
                Constants.VIX_PROPERTY_JOB_RESULT_FILE_FLAGS
            };

            try
            {
                foreach (object[] fileProperties in job.YieldWait(properties, timeoutInSeconds))
                {
                    string fileName = (string)fileProperties[0];
                    int flags = (int)fileProperties[1];

                    if ((flags & 1) > 0)
                    {
                        if (recurse)
                        {
                            results.AddRange(ListDirectoryInGuest(Path.Combine(pathName, fileName), 
                                true, timeoutInSeconds));
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
                    case 2:
                        // file not found? empty directory in ESX
                    case Constants.VIX_E_UNRECOGNIZED_PROPERTY:
                        // unrecognized property returned by GetNumProperties, the directory exists, but contains no files
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
        /// VMWare doesn't publish a list of known variables, the following guest variables have been observed.
        /// <list type="bullet">
        /// <item>ip: IP address of the guest operating system</item>
        /// </list>
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

        /// <summary>
        /// Shared folders on this virtual machine.
        /// </summary>
        public VMWareSharedFolderCollection SharedFolders
        {
            get
            {
                return _sharedFolders;
            }
        }

        /// <summary>
        /// Captures the screen of the guest operating system.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Image"/> object holding the captured screen image.</returns>
        public Image CaptureScreenImage()
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_handle.CaptureScreenImage(
                Constants.VIX_CAPTURESCREENFORMAT_PNG, null, callback), 
                callback);
            byte[] imageBytes = job.Wait<byte[]>(
                Constants.VIX_PROPERTY_JOB_RESULT_SCREEN_IMAGE_DATA,
                VMWareInterop.Timeouts.CaptureScreenImageTimeout);
            return Image.FromStream(new MemoryStream(imageBytes));
        }

        /// <summary>
        /// Running processes in the guest operating system by process id.
        /// </summary>
        public Dictionary<long, Process> GuestProcesses
        {
            get
            {
                Dictionary<long, Process> processes = new Dictionary<long, Process>();
                VMWareJobCallback callback = new VMWareJobCallback();
                VMWareJob job = new VMWareJob(_handle.ListProcessesInGuest(
                    0, callback), callback);
                object[] properties = 
                { 
                    Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_ID,
                    Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME,
                    Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_OWNER,
                    Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_START_TIME,
                    Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_COMMAND,
                    Constants.VIX_PROPERTY_JOB_RESULT_PROCESS_BEING_DEBUGGED,
                };

                foreach (object[] processProperties in job.YieldWait(properties, VMWareInterop.Timeouts.ListProcessesTimeout))
                {
                    Process process = new Process(_handle);
                    process.Id = (long)processProperties[0];
                    process.Name = (string)processProperties[1];
                    process.Owner = (string)processProperties[2];
                    process.StartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((int)processProperties[3]);
                    process.Command = (string)processProperties[4];
                    process.IsBeingDebugged = (bool)processProperties[5];
                    processes.Add(process.Id, process);
                }

                return processes;
            }
        }
    }
}

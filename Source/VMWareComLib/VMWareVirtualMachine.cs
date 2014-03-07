using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Vestris.VMWareComLib
{
    /// <summary>
    /// The default implementation of the <see cref="Vestris.VMWareComLib.IVMWareVirtualMachine" /> COM interface.
    /// </summary>
    [ComVisible(true)]
    [ProgId("VMWareComLib.VMWareVirtualMachine")]
    [ComDefaultInterface(typeof(IVMWareVirtualMachine))]
    [Guid("F4BC0794-4BE4-47e0-9FC4-804ADAAEB4C1")]
    public class VMWareVirtualMachine : IVMWareVirtualMachine 
    {
        [ComVisible(true)]
        [ProgId("VMWareComLib.VMWareVirtualMachine.GuestFileInfo")]
        [ComDefaultInterface(typeof(IGuestFileInfo))]
        [Guid("FD69E775-94E4-425b-BA1F-967BB258993F")]
        public class GuestFileInfo : Vestris.VMWareComLib.IGuestFileInfo
        {
            Vestris.VMWareLib.VMWareVirtualMachine.GuestFileInfo _gfi;

            public GuestFileInfo()
            {
            }

            public GuestFileInfo(Vestris.VMWareLib.VMWareVirtualMachine.GuestFileInfo gfi)
            {
                _gfi = gfi;
            }

            public long FileSize
            {
                get
                {
                    return _gfi.FileSize;
                }
            }

            public int Flags
            {
                get
                {
                    return _gfi.Flags;
                }
            }

            public bool IsDirectory
            {
                get
                {
                    return _gfi.IsDirectory;
                }
            }

            public bool IsSymLink
            {
                get
                {
                    return _gfi.IsSymLink;
                }
            }

            public DateTime LastModified
            {
                get
                {
                    return _gfi.LastModified.Value;
                }
            }

            public string GuestPathName
            {
                get
                {
                    return _gfi.GuestPathName;
                }
            }
        }

        [ComVisible(true)]
        [ProgId("VMWareComLib.VMWareVirtualMachine.Process")]
        [ComDefaultInterface(typeof(IProcess))]
        [Guid("58954C2A-F83C-43f1-B735-431C9794072E")]
        public class Process : Vestris.VMWareComLib.IProcess
        {
            private Vestris.VMWareLib.VMWareVirtualMachine.Process _p = null;

            public Process()
            {
            }

            public Process(Vestris.VMWareLib.VMWareVirtualMachine.Process p)
            {
                _p = p;
            }

            public long Id
            {
                get
                {
                    return _p.Id;
                }
            }

            public string Name
            {
                get
                {
                    return _p.Name;
                }
            }

            public string Owner
            {
                get
                {
                    return _p.Owner;
                }
            }

            public DateTime StartDateTime
            {
                get
                {
                    return _p.StartDateTime;
                }
            }

            public string Command
            {
                get
                {
                    return _p.Command;
                }
            }

            public bool IsBeingDebugged
            {
                get
                {
                    return _p.IsBeingDebugged;
                }
            }

            public int ExitCode
            {
                get
                {
                    return _p.ExitCode;
                }
            }

            public void KillProcessInGuest()
            {
                _p.KillProcessInGuest();
            }

            public void KillProcessInGuest(int timeoutInSeconds)
            {
                _p.KillProcessInGuest(timeoutInSeconds);
            }
        }

        [ComVisible(true)]
        [ProgId("VMWareComLib.VMWareVirtualMachine.VariableIndexer")]
        [ComDefaultInterface(typeof(IVariableIndexer))]
        [Guid("F9FE604D-6E08-4164-A068-2DC505A79470")]
        public class VariableIndexer : IVariableIndexer
        {
            private Vestris.VMWareLib.VMWareVirtualMachine.VariableIndexer _ix = null;

            public VariableIndexer()
            {

            }

            public VariableIndexer(Vestris.VMWareLib.VMWareVirtualMachine.VariableIndexer ix)
            {
                _ix = ix;
            }

            [IndexerName("Variables")]
            public string this[string name]
            {
                get
                {
                    return _ix[name];
                }
            }
        }

        private Vestris.VMWareLib.VMWareVirtualMachine _vm = null;

        public VMWareVirtualMachine()
        {
        }

        public VMWareVirtualMachine(Vestris.VMWareLib.VMWareVirtualMachine vm)
        {
            _vm = vm;
        }

        public string Name
        {
            get
            {
                return _vm.Name;
            }
        }

        public string PathName
        {
            get
            {
                return _vm.PathName;
            }
        }

        public bool IsRunning
        {
            get
            {
                return _vm.IsRunning;
            }
        }

        public int PowerState
        {
            get
            {
                return _vm.PowerState;
            }
        }

        public bool IsPaused
        {
            get
            {
                return _vm.IsPaused;
            }
        }

        public bool IsSuspended
        {
            get
            {
                return _vm.IsSuspended;
            }
        }

        public int MemorySize
        {
            get
            {
                return _vm.MemorySize;
            }
        }

        public int CPUCount
        {
            get
            {
                return _vm.CPUCount;
            }
        }

        public void PowerOn()
        {
            _vm.PowerOn();
        }

        public void PowerOn2(int powerOnOptions, int timeoutInSeconds)
        {
            _vm.PowerOn(powerOnOptions, timeoutInSeconds);
        }

        public void WaitForToolsInGuest()
        {
            _vm.WaitForToolsInGuest();
        }

        public void WaitForToolsInGuest2(int timeoutInSeconds)
        {
            _vm.WaitForToolsInGuest(timeoutInSeconds);
        }

        public IVMWareRootSnapshotCollection Snapshots
        {
            get
            {
                return new VMWareRootSnapshotCollection(_vm.Snapshots);
            }
        }

        public void LoginInGuest(string username, string password)
        {
            _vm.LoginInGuest(username, password);
        }

        public void LoginInGuest2(string username, string password, int options, int timeoutInSeconds)
        {
            _vm.LoginInGuest(username, password, options, timeoutInSeconds);
        }

        public void WaitForVMWareUserProcessInGuest(string username, string password)
        {
            _vm.WaitForVMWareUserProcessInGuest(username, password);
        }

        public void WaitForVMWareUserProcessInGuest(string username, string password, int timeoutInSeconds)
        {
            _vm.WaitForVMWareUserProcessInGuest(username, password, timeoutInSeconds);
        }

        public void CopyFileFromHostToGuest(string hostPathName, string guestPathName)
        {
            _vm.CopyFileFromGuestToHost(hostPathName, guestPathName);
        }

        public void CopyFileFromHostToGuest2(string hostPathName, string guestPathName, int timeoutInSeconds)
        {
            _vm.CopyFileFromGuestToHost(hostPathName, guestPathName, timeoutInSeconds);
        }

        public void DeleteFileFromGuest(string guestPathName)
        {
            _vm.DeleteFileFromGuest(guestPathName);
        }

        public void DeleteFileFromGuest2(string guestPathName, int timeoutInSeconds)
        {
            _vm.DeleteFileFromGuest(guestPathName, timeoutInSeconds);
        }

        public void DeleteDirectoryFromGuest(string guestPathName)
        {
            _vm.DeleteDirectoryFromGuest(guestPathName);
        }

        public void DeleteDirectoryFromGuest2(string guestPathName, int timeoutInSeconds)
        {
            _vm.DeleteDirectoryFromGuest(guestPathName, timeoutInSeconds);
        }

        public void CopyFileFromGuestToHost(string guestPathName, string hostPathName)
        {
            _vm.CopyFileFromGuestToHost(guestPathName, hostPathName);
        }

        public void CopyFileFromGuestToHost2(string guestPathName, string hostPathName, int timeoutInSeconds)
        {
            _vm.CopyFileFromGuestToHost(guestPathName, hostPathName, timeoutInSeconds);
        }

        public void CreateDirectoryInGuest(string guestPathName)
        {
            _vm.CreateDirectoryInGuest(guestPathName);
        }

        public void CreateDirectoryInGuest2(string guestPathName, int timeoutInSeconds)
        {
            _vm.CreateDirectoryInGuest(guestPathName, timeoutInSeconds);
        }

        public string CreateTempFileInGuest()
        {
            return _vm.CreateTempFileInGuest();
        }

        public string CreateTempFileInGuest2(int timeoutInSeconds)
        {
            return _vm.CreateTempFileInGuest(timeoutInSeconds);
        }

        public IGuestFileInfo GetFileInfoInGuest(string guestPathName)
        {
            return new GuestFileInfo(_vm.GetFileInfoInGuest(guestPathName));
        }

        public IGuestFileInfo GetFileInfoInGuest2(string guestPathName, int timeoutInSeconds)
        {
            return new GuestFileInfo(_vm.GetFileInfoInGuest(guestPathName, timeoutInSeconds));
        }

        public IProcess RunProgramInGuest(string guestProgramName, string commandLineArgs)
        {
            return new Process(_vm.RunProgramInGuest(guestProgramName, commandLineArgs));
        }

        public IProcess RunProgramInGuest2(string guestProgramName, string commandLineArgs, int options, int timeoutInSeconds)
        {
            return new Process(_vm.RunProgramInGuest(guestProgramName, commandLineArgs, options, timeoutInSeconds));
        }

        public IProcess DetachProgramInGuest(string guestProgramName, string commandLineArgs)
        {
            return new Process(_vm.DetachProgramInGuest(guestProgramName, commandLineArgs));
        }

        public IProcess DetachProgramInGuest2(string guestProgramName, string commandLineArgs, int timeoutInSeconds)
        {
            return new Process(_vm.DetachProgramInGuest(guestProgramName, commandLineArgs, timeoutInSeconds));
        }

        public IProcess RunScriptInGuest(string interpreter, string scriptText)
        {
            return new Process(_vm.RunScriptInGuest(interpreter, scriptText));
        }

        public IProcess RunScriptInGuest2(string interpreter, string scriptText, int options, int timeoutInSeconds)
        {
            return new Process(_vm.RunScriptInGuest(interpreter, scriptText, options, timeoutInSeconds));
        }

        public IProcess DetachScriptInGuest(string interpreter, string scriptText)
        {
            return new Process(_vm.DetachScriptInGuest(interpreter, scriptText));
        }

        public IProcess DetachScriptInGuest2(string interpreter, string scriptText, int timeoutInSeconds)
        {
            return new Process(_vm.DetachScriptInGuest(interpreter, scriptText, timeoutInSeconds));
        }

        public void OpenUrlInGuest(string url)
        {
            _vm.OpenUrlInGuest(url);
        }

        public void OpenUrlInGuest2(string url, int timeoutInSeconds)
        {
            _vm.OpenUrlInGuest(url, timeoutInSeconds);
        }

        public bool FileExistsInGuest(string guestPathName)
        {
            return _vm.FileExistsInGuest(guestPathName);
        }

        public bool FileExistsInGuest2(string guestPathName, int timeoutInSeconds)
        {
            return _vm.FileExistsInGuest(guestPathName, timeoutInSeconds);
        }

        public bool DirectoryExistsInGuest(string guestPathName)
        {
            return _vm.DirectoryExistsInGuest(guestPathName);
        }

        public bool DirectoryExistsInGuest2(string guestPathName, int timeoutInSeconds)
        {
            return _vm.DirectoryExistsInGuest(guestPathName, timeoutInSeconds);
        }

        public void LogoutFromGuest()
        {
            _vm.LogoutFromGuest();
        }

        public void LogoutFromGuest2(int timeoutInSeconds)
        {
            _vm.LogoutFromGuest(timeoutInSeconds);
        }

        public void PowerOff()
        {
            _vm.PowerOff();
        }

        public void PowerOff2(int powerOffOptions, int timeoutInSeconds)
        {
            _vm.PowerOff(powerOffOptions, timeoutInSeconds);
        }

        public void ShutdownGuest()
        {
            _vm.ShutdownGuest();
        }

        public void ShutdownGuest2(int timeoutInSeconds)
        {
            _vm.ShutdownGuest(timeoutInSeconds);
        }

        public void Reset()
        {
            _vm.Reset();
        }

        public void Reset2(int resetOptions, int timeoutInSeconds)
        {
            _vm.Reset(resetOptions, timeoutInSeconds);
        }

        public void Suspend()
        {
            _vm.Suspend();
        }

        public void Suspend2(int timeoutInSeconds)
        {
            _vm.Suspend(timeoutInSeconds);
        }

        public void Pause()
        {
            _vm.Pause();
        }

        public void Pause2(int timeoutInSeconds)
        {
            _vm.Pause(timeoutInSeconds);
        }

        public void Unpause()
        {
            _vm.Unpause();
        }

        public void Unpause2(int timeoutInSeconds)
        {
            _vm.Unpause(timeoutInSeconds);
        }

        public string[] ListDirectoryInGuest(string pathName, bool recurse)
        {
            return _vm.ListDirectoryInGuest(pathName, recurse).ToArray();
        }

        public string[] ListDirectoryInGuest2(string pathName, bool recurse, int timeoutInSeconds)
        {
            return _vm.ListDirectoryInGuest(pathName, recurse, timeoutInSeconds).ToArray();
        }

        public IVariableIndexer GuestEnvironmentVariables
        {
            get
            {
                return new VariableIndexer(_vm.GuestEnvironmentVariables);
            }
        }

        public IVariableIndexer GuestVariables
        {
            get
            {
                return new VariableIndexer(_vm.GuestVariables);
            }
        }

        public IVariableIndexer RuntimeConfigVariables
        {
            get
            {
                return new VariableIndexer(_vm.RuntimeConfigVariables);
            }
        }

        public IVMWareSharedFolderCollection SharedFolders
        {
            get
            {
                return new VMWareSharedFolderCollection(_vm.SharedFolders);
            }
        }

        /*
        public Image CaptureScreenImage()
        {
        }

        public Dictionary<long, Process> GuestProcesses
        {
            get
            {
            }
        }
        */

        public bool IsRecording
        {
            get
            {
                return _vm.IsRecording;
            }
        }

        public bool IsReplaying
        {
            get
            {
                return _vm.IsReplaying;
            }
        }

        public IVMWareSnapshot BeginRecording(string name, string description)
        {
            return new VMWareSnapshot(_vm.BeginRecording(name, description));
        }

        public IVMWareSnapshot BeginRecording2(string name, string description, int timeoutInSeconds)
        {
            return new VMWareSnapshot(_vm.BeginRecording(name, description, timeoutInSeconds));
        }

        public void EndRecording()
        {
            _vm.EndRecording();
        }

        public void EndRecording2(int timeoutInSeconds)
        {
            _vm.EndRecording(timeoutInSeconds);
        }

        public void UpgradeVirtualHardware()
        {
            _vm.UpgradeVirtualHardware();
        }

        public void UpgradeVirtualHardware2(int timeoutInSeconds)
        {
            _vm.UpgradeVirtualHardware(timeoutInSeconds);
        }

        /*
        public void Clone(VMWareVirtualMachineCloneType cloneType, string destConfigPathName)
        {
        }

        public void Clone(VMWareVirtualMachineCloneType cloneType, string destConfigPathName, int timeoutInSeconds)
        {
        }
        */

        public void Delete()
        {
            _vm.Delete();
        }

        public void Delete2(int deleteOptions, int timeoutInSeconds)
        {
            _vm.Delete(deleteOptions, timeoutInSeconds);
        }

        public void InstallTools()
        {
            _vm.InstallTools();
        }

        public void InstallTools2(int timeoutInSeconds)
        {
            _vm.InstallTools(timeoutInSeconds);
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    [ComVisible(true)]
    [Guid("9CE66DE2-9BDA-430f-92BC-B9171D5F2A26")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IVMWareVirtualMachine
    {
        void CopyFileFromGuestToHost(string guestPathName, string hostPathName);
        void CopyFileFromGuestToHost2(string guestPathName, string hostPathName, int timeoutInSeconds);
        void CopyFileFromHostToGuest(string hostPathName, string guestPathName);
        void CopyFileFromHostToGuest2(string hostPathName, string guestPathName, int timeoutInSeconds);
        int CPUCount { get; }
        void CreateDirectoryInGuest(string guestPathName);
        void CreateDirectoryInGuest2(string guestPathName, int timeoutInSeconds);
        string CreateTempFileInGuest();
        string CreateTempFileInGuest2(int timeoutInSeconds);
        void Delete();
        void Delete2(int deleteOptions, int timeoutInSeconds);
        void DeleteDirectoryFromGuest(string guestPathName);
        void DeleteDirectoryFromGuest2(string guestPathName, int timeoutInSeconds);
        void DeleteFileFromGuest(string guestPathName);
        void DeleteFileFromGuest2(string guestPathName, int timeoutInSeconds);
        IProcess DetachProgramInGuest(string guestProgramName, string commandLineArgs);
        IProcess DetachProgramInGuest2(string guestProgramName, string commandLineArgs, int timeoutInSeconds);
        IProcess DetachScriptInGuest(string interpreter, string scriptText);
        IProcess DetachScriptInGuest2(string interpreter, string scriptText, int timeoutInSeconds);
        bool DirectoryExistsInGuest(string guestPathName);
        bool DirectoryExistsInGuest2(string guestPathName, int timeoutInSeconds);
        void EndRecording();
        void EndRecording2(int timeoutInSeconds);
        bool FileExistsInGuest(string guestPathName);
        bool FileExistsInGuest2(string guestPathName, int timeoutInSeconds);
        IGuestFileInfo GetFileInfoInGuest(string guestPathName);
        IGuestFileInfo GetFileInfoInGuest2(string guestPathName, int timeoutInSeconds);
        IVariableIndexer GuestEnvironmentVariables { get; }
        IVariableIndexer GuestVariables { get; }
        void InstallTools();
        void InstallTools2(int timeoutInSeconds);
        bool IsPaused { get; }
        bool IsRecording { get; }
        bool IsReplaying { get; }
        bool IsRunning { get; }
        bool IsSuspended { get; }
        string[] ListDirectoryInGuest(string pathName, bool recurse);
        string[] ListDirectoryInGuest2(string pathName, bool recurse, int timeoutInSeconds);
        void LoginInGuest(string username, string password);
        void LoginInGuest2(string username, string password, int options, int timeoutInSeconds);
        void LogoutFromGuest();
        void LogoutFromGuest2(int timeoutInSeconds);
        int MemorySize { get; }
        void OpenUrlInGuest(string url);
        void OpenUrlInGuest2(string url, int timeoutInSeconds);
        string PathName { get; }
        void Pause();
        void Pause2(int timeoutInSeconds);
        void PowerOff();
        void PowerOff2(int powerOffOptions, int timeoutInSeconds);
        void PowerOn();
        void PowerOn2(int powerOnOptions, int timeoutInSeconds);
        int PowerState { get; }
        void Reset();
        void Reset2(int resetOptions, int timeoutInSeconds);
        IProcess RunProgramInGuest(string guestProgramName, string commandLineArgs);
        IProcess RunProgramInGuest2(string guestProgramName, string commandLineArgs, int options, int timeoutInSeconds);
        IProcess RunScriptInGuest(string interpreter, string scriptText);
        IProcess RunScriptInGuest2(string interpreter, string scriptText, int options, int timeoutInSeconds);
        IVariableIndexer RuntimeConfigVariables { get; }
        void ShutdownGuest();
        void Suspend();
        void Suspend2(int timeoutInSeconds);
        void Unpause();
        void Unpause2(int timeoutInSeconds);
        void UpgradeVirtualHardware();
        void UpgradeVirtualHardware2(int timeoutInSeconds);
        void WaitForToolsInGuest();
        void WaitForToolsInGuest2(int timeoutInSeconds);
        IVMWareRootSnapshotCollection Snapshots { get; }
        IVMWareSnapshot BeginRecording(string name, string description);
        IVMWareSnapshot BeginRecording2(string name, string description, int timeoutInSeconds);
    }
}

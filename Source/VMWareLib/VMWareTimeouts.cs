using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A collection of default timeouts used in VMWareTasks functions exposed without a timeout parameter.
    /// </summary>
    public class VMWareTimeouts
    {
        /// <summary>
        /// Maximum time, in seconds, to establish a connection to a VMWare host.
        /// </summary>
        public int ConnectTimeout;
        /// <summary>
        /// Maximum time, in seconds, to open a file in a VMWare guest operating system.
        /// </summary>
        public int OpenFileTimeout;
        /// <summary>
        /// Maximum time, in seconds, to revert a snapshot.
        /// </summary>
        public int RevertToSnapshotTimeout;
        /// <summary>
        /// Maximum time, in seconds, to remove (delete) a snapshot.
        /// </summary>
        public int RemoveSnapshotTimeout;
        /// <summary>
        /// Maximum time, in seconds, to create a snapshot.
        /// </summary>
        public int CreateSnapshotTimeout;
        /// <summary>
        /// The maximum operational time, in seconds, to bring the power to/from the vm, not to boot it
        /// </summary>
        public int PowerOnTimeout;
        /// <summary>
        /// The maximum time, in seconds, to power off a virtual machine.
        /// </summary>
        public int PowerOffTimeout;
        /// <summary>
        /// The maximum time, in seconds, to wait for tools in a guest operating system.
        /// </summary>
        public int WaitForToolsTimeout;
        /// <summary>
        /// The maximum time, in seconds, to wait for a log-in to a guest operating system.
        /// </summary>
        public int LoginTimeout;
        /// <summary>
        /// Maximum time, in seconds, to copy a file from guest to host and from host to guest.
        /// <remarks>
        /// Copy is very slow, see http://communities.vmware.com/thread/184489.
        /// </remarks>
        /// </summary>
        public int CopyFileTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait for a file to be deleted in the guest operating system.
        /// </summary>
        public int DeleteFileTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait for a directory to be deleted in the guest operating system.
        /// </summary>
        public int DeleteDirectoryTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait for a pogram to start in the guest operating system.
        /// </summary>
        public int RunProgramTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait to check whether a file exists in the guest operating system.
        /// </summary>
        public int FileExistsTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait to check whether a directory exists in the guest operating system.
        /// </summary>
        public int DirectoryExistsTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait for a logout from a guest operating system to complete.
        /// </summary>
        public int LogoutTimeout;
        /// <summary>
        /// Maximum time, in seconds, to list the contents of a directory in the guest operating system.
        /// </summary>
        public int ListDirectoryTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait to read a remote variable.
        /// </summary>
        public int ReadVariableTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait to write a remote variable.
        /// </summary>
        public int WriteVariableTimeout;
        /// <summary>
        /// Maximum time, in seconds, to wait to fetch the list of shared folders.
        /// </summary>
        public int GetSharedFoldersTimeout;
        /// <summary>
        /// Maximum time, in seconds, to add/remove a shared folder.
        /// </summary>
        public int AddRemoveSharedFolderTimeout;
        /// <summary>
        /// Maximum time, in seconds, to capture a screen image.
        /// </summary>
        public int CaptureScreenImageTimeout;
        /// <summary>
        /// Maximum time, in seconds, to create a directory in the guest operating system.
        /// </summary>
        public int CreateDirectoryTimeout;
        /// <summary>
        /// Maximum time, in seconds, to create a temporary file in the guest operating system.
        /// </summary>
        public int CreateTempFileTimeout;
        /// <summary>
        /// Maximum time, in seconds, to list processes in the guest operating system.
        /// </summary>
        public int ListProcessesTimeout;
        /// <summary>
        /// Maximum time, in seconds, to fetch a collection of items in find operations.
        /// </summary>
        public int FindItemsTimeout;
        /// <summary>
        /// Maximum time, in seconds, to kill a process in the guest operating system.
        /// </summary>
        public int KillProcessTimeout;

        /// <summary>
        /// A collection of timeouts based on a default 60-seconds base timeout.
        /// </summary>
        public VMWareTimeouts()
            : this(60)
        {
        }

        /// <summary>
        /// A collection of timeouts based on a configurable base timeout.
        /// </summary>
        /// <param name="baseTimeout">a base timeout</param>
        public VMWareTimeouts(int baseTimeout)
        {
            ConnectTimeout = baseTimeout;
            OpenFileTimeout = baseTimeout;
            RevertToSnapshotTimeout = baseTimeout;
            RemoveSnapshotTimeout = baseTimeout * 10;
            CreateSnapshotTimeout = baseTimeout * 10;
            PowerOnTimeout = baseTimeout;
            PowerOffTimeout = baseTimeout;
            WaitForToolsTimeout = 5 * baseTimeout;
            LoginTimeout = baseTimeout;
            CopyFileTimeout = 20 * baseTimeout;
            DeleteFileTimeout = baseTimeout;
            DeleteDirectoryTimeout = baseTimeout;
            CreateDirectoryTimeout = baseTimeout;
            RunProgramTimeout = 5 * baseTimeout;
            FileExistsTimeout = baseTimeout;
            DirectoryExistsTimeout = baseTimeout;
            LogoutTimeout = baseTimeout;
            ListDirectoryTimeout = baseTimeout;
            ReadVariableTimeout = baseTimeout;
            WriteVariableTimeout = baseTimeout;
            GetSharedFoldersTimeout = baseTimeout;
            AddRemoveSharedFolderTimeout = baseTimeout;
            CaptureScreenImageTimeout = baseTimeout;
            CreateTempFileTimeout = baseTimeout;
            ListProcessesTimeout = baseTimeout;
            FindItemsTimeout = baseTimeout;
            KillProcessTimeout = baseTimeout;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib
{
    public class VMWareTimeouts
    {
        public int ConnectTimeout;
        public int OpenFileTimeout;
        public int RevertToSnapshotTimeout;
        public int RemoveSnapshotTimeout;
        public int CreateSnapshotTimeout;
        /// <summary>
        /// the operational time to bring the power to/from the vm, not to boot it
        /// </summary>
        public int PowerOnTimeout;
        public int PowerOffTimeout;
        public int WaitForToolsTimeout;
        /// <summary>
        /// the time to actually boot the machine
        /// </summary>
        public int LoginTimeout;
        /// <summary>
        /// copy is very slow, see http://communities.vmware.com/thread/184489
        /// </summary>
        public int CopyFileTimeout;
        public int DeleteFileTimeout;
        public int DeleteDirectoryTimeout;
        public int RunProgramTimeout;
        public int FileExistsTimeout;
        public int DirectoryExistsTimeout;
        public int LogoutTimeout;
        public int ListDirectoryTimeout;
        public int ReadVariableTimeout;
        public int WriteVariableTimeout;
        public int GetSharedFoldersTimeout;
        public int AddRemoveSharedFolderTimeout;
        public int CaptureScreenImageTimeout;
        public int CreateDirectoryTimeout;
        public int CreateTempFileTimeout;
        public int ListProcessesTimeout;
        public int FindItemsTimeout;

        public VMWareTimeouts()
            : this(60)
        {
        }

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
        }
    }
}

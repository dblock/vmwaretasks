using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib
{
    public abstract class VMWareTimeouts
    {
        public const int defaultConnectTimeout = 60;
        public const int defaultOpenFileTimeout = 60;
        public const int defaultRevertToSnapshotTimeout = 60;
        /// <summary>
        /// the operational time to bring the power to/from the vm, not to boot it
        /// </summary>
        public const int defaultPowerOnTimeout = 60;
        public const int defaultPowerOffTimeout = 60;
        public const int defaultWaitForToolsInGuestTimeout = 5 * 60;
        /// <summary>
        /// the time to actually boot the machine
        /// </summary>
        public const int defaultLoginTimeout = 60;
        /// <summary>
        /// copy is very slow, see http://communities.vmware.com/thread/184489
        /// </summary>
        public const int defaultCopyFileTimeout = 60 * 20;
        public const int defaultDeleteTimeout = 30;
        public const int defaultExecuteTimeout = 60 * 20;
        public const int defaultFileExistsTimeout = 60;
        public const int defaultLogoutTimeout = 60;
        public const int defaultListDirectoryInGuestTimeout = 60;
    }
}

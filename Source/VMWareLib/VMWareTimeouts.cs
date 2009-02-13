using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib
{
    public abstract class VMWareTimeouts
    {
        public const int defaultTimeout = 60;
        public const int defaultConnectTimeout = defaultTimeout;
        public const int defaultOpenFileTimeout = defaultTimeout;
        public const int defaultRevertToSnapshotTimeout = defaultTimeout;
        /// <summary>
        /// the operational time to bring the power to/from the vm, not to boot it
        /// </summary>
        public const int defaultPowerOnTimeout = defaultTimeout;
        public const int defaultPowerOffTimeout = defaultTimeout;
        public const int defaultWaitForToolsInGuestTimeout = 5 * defaultTimeout;
        /// <summary>
        /// the time to actually boot the machine
        /// </summary>
        public const int defaultLoginTimeout = defaultTimeout;
        /// <summary>
        /// copy is very slow, see http://communities.vmware.com/thread/184489
        /// </summary>
        public const int defaultCopyFileTimeout = 20 * defaultTimeout;
        public const int defaultDeleteTimeout = defaultTimeout;
        public const int defaultExecuteTimeout = 5 * defaultTimeout;
        public const int defaultFileExistsTimeout = defaultTimeout;
        public const int defaultLogoutTimeout = defaultTimeout;
        public const int defaultListDirectoryInGuestTimeout = defaultTimeout;
    }
}

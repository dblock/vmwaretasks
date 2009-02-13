using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A VMWare Shared Folder.
    /// A shared folder is a local mount point in the guest file system which mounts a shared folder exported by the host.
    /// Shared folders are not supported for the following guest operating systems: 
    /// Windows ME, Windows 98, Windows 95, Windows 3.x, and DOS. 
    /// </summary>
    public class VMWareSharedFolder
    {
        private string _shareName;
        private string _hostPath;
        private int _flags;

        /// <summary>
        /// A shared folder defined by share name and host path.
        /// </summary>
        /// <param name="shareName">share name</param>
        /// <param name="hostPath">host path</param>
        public VMWareSharedFolder(string shareName, string hostPath)
            : this(shareName, hostPath, 0)
        {

        }

        /// <summary>
        /// A shared folder defined by share name, host path and additional flags.
        /// </summary>
        /// <param name="shareName">share name</param>
        /// <param name="hostPath">host path</param>
        /// <param name="flags">additional flags</param>
        public VMWareSharedFolder(string shareName, string hostPath, int flags)
        {
            _shareName = shareName;
            _hostPath = hostPath;
            _flags = flags;
        }

        /// <summary>
        /// The name of the folder.
        /// </summary>
        public string ShareName
        {
            get
            {
                return _shareName;
            }
        }

        /// <summary>
        /// Host path this folder is mounted from.
        /// Only absolute paths should be used for files in the guest; the resolution of relative paths is not specified. 
        /// </summary>
        public string HostPath
        {
            get
            {
                return _hostPath;
            }
        }

        /// <summary>
        /// Shared folder flags, one of the following.
        /// <list type="bullet">
        ///  <item>VIX_SHAREDFOLDER_WRITE_ACCESS: allow write access</item>
        /// </list>
        /// </summary>
        public int Flags
        {
            get
            {
                return _flags;
            }
        }
    }
}

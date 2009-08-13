using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    /// <summary>
    /// The default implementation of the <see cref="Vestris.VMWareComLib.IVMWareSharedFolder" /> COM interface.
    /// </summary>
    [ComVisible(true)]
    [ComDefaultInterface(typeof(IVMWareSharedFolder))]
    [ProgId("VMWareComLib.VMWareSharedFolder")]
    [Guid("7D371600-E174-406d-B32D-DF9A1C6C0A83")]
    public class VMWareSharedFolder : IVMWareSharedFolder
    {
        private Vestris.VMWareLib.VMWareSharedFolder _folder = null;

        public VMWareSharedFolder()
        {

        }

        public VMWareSharedFolder(Vestris.VMWareLib.VMWareSharedFolder folder)
        {
            _folder = folder;
        }

        public string ShareName
        {
            get
            {
                return _folder.ShareName;
            }
        }

        public string HostPath
        {
            get
            {
                return _folder.HostPath;
            }
        }

        public int Flags
        {
            get
            {
                return _folder.Flags;
            }
        }
    }
}

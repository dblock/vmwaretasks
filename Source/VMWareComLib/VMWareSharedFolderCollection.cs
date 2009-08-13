using System;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib
{
    /// <summary>
    /// The default implementation of the <see cref="Vestris.VMWareComLib.IVMWareSharedFolderCollection" /> COM interface.
    /// </summary>
    [ComVisible(true)]
    [ComDefaultInterface(typeof(IVMWareSharedFolderCollection))]
    [Guid("A5C7F11C-6869-4e2d-8418-79A9E00E32FA")]
    [ProgId("VMWareComLib.VMWareSharedFolderCollection")]
    public class VMWareSharedFolderCollection : IVMWareSharedFolderCollection
    {
        private Vestris.VMWareLib.VMWareSharedFolderCollection _coll = null;

        public VMWareSharedFolderCollection()
        {

        }

        public VMWareSharedFolderCollection(Vestris.VMWareLib.VMWareSharedFolderCollection coll)
        {
            _coll = coll;
        }

        public void Clear()
        {
            _coll.Clear();
        }

        public int Count
        {
            get
            {
                return _coll.Count;
            }
        }

        public bool Enabled
        {
            set
            {
                _coll.Enabled = value;
            }
        }

        public IVMWareSharedFolder this[int index]
        {
            get
            {
                return new VMWareSharedFolder(_coll[index]);
            }
        }
    }
}

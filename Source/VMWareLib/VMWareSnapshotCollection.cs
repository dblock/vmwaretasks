using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VixCOM;
using System.IO;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A collection of snapshots at any snapshot level.
    /// </summary>
    public class VMWareSnapshotCollection : IEnumerable<VMWareSnapshot>
    {
        protected IVM _vm = null;
        protected List<VMWareSnapshot> _snapshots = null;

        public VMWareSnapshotCollection(IVM vm)
        {
            _vm = vm;
        }

        /// <summary>
        /// The list of snapshots.
        /// </summary>
        protected virtual List<VMWareSnapshot> Snapshots
        {
            get
            {
                if (_snapshots == null)
                {
                    _snapshots = new List<VMWareSnapshot>();
                }
                return _snapshots;
            }
        }

        /// <summary>
        /// Find a snapshot.
        /// </summary>
        /// <param name="name">path to a snapshot</param>
        /// <returns>a snapshot, null if not found</returns>
        public VMWareSnapshot FindSnapshot(string pathToSnapshot)
        {
            string[] paths = pathToSnapshot.Split("\\".ToCharArray(), 2);
            
            foreach (VMWareSnapshot snapshot in this)
            {
                // last snapshot in the path
                if (snapshot.DisplayName == paths[0])
                {
                    return (paths.Length == 1)
                        ? snapshot
                        : snapshot.ChildSnapshots.FindSnapshot(paths[1]);
                }
            }
            return null;
        }

        public void CopyTo(VMWareSnapshot[] array, int arrayIndex)
        {
            Snapshots.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns true if this virtual machine has the snapshot specified.
        /// </summary>
        /// <param name="item">snapshot</param>
        /// <returns>true if the virtual machine contains the specified snapshot</returns>
        public bool Contains(VMWareSnapshot item)
        {
            return Snapshots.Contains(item);
        }

        /// <summary>
        /// Number of snapshots.
        /// </summary>
        public int Count
        {
            get
            {
                return Snapshots.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        IEnumerator<VMWareSnapshot> IEnumerable<VMWareSnapshot>.GetEnumerator()
        {
            return Snapshots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Snapshots.GetEnumerator();
        }

        /// <summary>
        /// Add a snapshot to the list.
        /// </summary>
        /// <param name="snapshot">snapshot to add</param>
        public void Add(VMWareSnapshot snapshot)
        {
            Snapshots.Add(snapshot);
        }
    }
}

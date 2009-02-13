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
        /// <summary>
        /// Virtual machine handle.
        /// </summary>
        protected IVM _vm = null;
        /// <summary>
        /// Internal list of snapshots.
        /// </summary>
        protected List<VMWareSnapshot> _snapshots = null;
        private VMWareSnapshot _parent = null;

        /// <summary>
        /// Snapshot collection constructor.
        /// </summary>
        /// <param name="vm">virtual machine</param>
        /// <param name="parent">snapshot parent</param>
        public VMWareSnapshotCollection(IVM vm, VMWareSnapshot parent)
        {
            _vm = vm;
            _parent = parent;
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
        /// <param name="pathToSnapshot">path to a snapshot</param>
        /// <returns>A snapshot, null if not found.</returns>
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

        /// <summary>
        /// Find a snapshot by name. Unlike GetSnapshotByName this function 
        /// doesn't throw an exception when there're two snapshots of the same name, it returns
        /// the first snapshot found.
        /// </summary>
        /// <param name="name">name of a snapshot</param>
        /// <returns>The first snapshot that matches the name, null if not found.</returns>
        public VMWareSnapshot FindSnapshotByName(string name)
        {
            foreach (VMWareSnapshot snapshot in this)
            {
                if (snapshot.DisplayName == name)
                {
                    return snapshot;
                }

                return snapshot.ChildSnapshots.FindSnapshotByName(name);
            }
            return null;
        }

        /// <summary>
        /// Find all snapshots by name. This can return multiple snapshots
        /// that have the same name.
        /// </summary>
        /// <param name="name">name of a snapshot</param>
        /// <returns>The first snapshot that matches the name, null if not found.</returns>
        public IEnumerable<VMWareSnapshot> FindSnapshotsByName(string name)
        {
            List<VMWareSnapshot> snapshots = new List<VMWareSnapshot>();
            
            foreach (VMWareSnapshot snapshot in this)
            {
                if (snapshot.DisplayName == name)
                {
                    snapshots.Add(snapshot);
                }

                snapshots.AddRange(snapshot.ChildSnapshots.FindSnapshotsByName(name));
            }

            return snapshots;
        }

        /// <summary>
        /// Copy to an array of VMWareSnapshots.
        /// </summary>
        /// <param name="array">target array</param>
        /// <param name="arrayIndex">array index</param>
        public void CopyTo(VMWareSnapshot[] array, int arrayIndex)
        {
            Snapshots.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns true if this virtual machine has the snapshot specified.
        /// </summary>
        /// <param name="item">snapshot</param>
        /// <returns>True if the virtual machine contains the specified snapshot.</returns>
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

        /// <summary>
        /// Returns true if the collection is read-only.
        /// A collection of snapshots is never read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// A snapshot collection enumerator.
        /// </summary>
        /// <returns>Snapshots enumerator.</returns>
        IEnumerator<VMWareSnapshot> IEnumerable<VMWareSnapshot>.GetEnumerator()
        {
            return Snapshots.GetEnumerator();
        }

        /// <summary>
        /// A snapshot collection enumerator.
        /// </summary>
        /// <returns>Snapshots enumerator.</returns>
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
            if (snapshot.Parent != null && snapshot.Parent != _parent)
            {
                throw new InvalidOperationException("Snapshot already belongs to another collection.");
            }

            Snapshots.Add(snapshot);
        }

        /// <summary>
        /// Remove a snapshot from this collection, append orphaned children.
        /// </summary>
        /// <param name="snapshot">removed snapshot</param>
        public void Remove(VMWareSnapshot snapshot)
        {
            _snapshots.Remove(snapshot);
            foreach (VMWareSnapshot childSnapshot in snapshot.ChildSnapshots)
            {
                childSnapshot.Parent = _parent;
                _snapshots.Add(childSnapshot);
            }
        }
    }
}

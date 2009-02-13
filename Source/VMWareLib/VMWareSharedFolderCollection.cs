using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VixCOM;

namespace Vestris.VMWareLib
{
    /// <summary>
    /// A collection of shared folders.
    /// Shared folders will only be accessible inside the guest operating system if shared folders are 
    /// enabled for the virtual machine.
    /// </summary>
    public class VMWareSharedFolderCollection :
        ICollection<VMWareSharedFolder>, IEnumerable<VMWareSharedFolder>
    {
        private IVM _vm = null;
        private List<VMWareSharedFolder> _sharedFolders = null;

        /// <summary>
        /// A collection of shared folders that belong to a virtual machine.
        /// </summary>
        /// <param name="vm">virtual machine</param>
        public VMWareSharedFolderCollection(IVM vm)
        {
            _vm = vm;
        }

        /// <summary>
        /// Add (create) a shared folder.
        /// </summary>
        /// <param name="sharedFolder">the shared folder to add</param>
        public void Add(VMWareSharedFolder sharedFolder)
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_vm.AddSharedFolder(
                sharedFolder.ShareName, sharedFolder.HostPath, sharedFolder.Flags, callback),
                callback);
            job.Wait(VMWareInterop.Timeouts.AddRemoveSharedFolderTimeout);
            _sharedFolders = null;
        }

        /// <summary>
        /// Get shared folders.
        /// </summary>
        /// <returns>A list of shared folders.</returns>
        private List<VMWareSharedFolder> SharedFolders
        {
            get
            {
                if (_sharedFolders == null)
                {
                    List<VMWareSharedFolder> sharedFolders = new List<VMWareSharedFolder>();
                    VMWareJobCallback callback = new VMWareJobCallback();
                    VMWareJob job = new VMWareJob(_vm.GetNumSharedFolders(callback), callback);
                    
                    int nSharedFolders = job.Wait<int>(
                        Constants.VIX_PROPERTY_JOB_RESULT_SHARED_FOLDER_COUNT, 
                        VMWareInterop.Timeouts.GetSharedFoldersTimeout);
                    
                    for (int i = 0; i < nSharedFolders; i++)
                    {
                        VMWareJobCallback getSharedfolderCallback = new VMWareJobCallback();
                        VMWareJob sharedFolderJob = new VMWareJob(
                            _vm.GetSharedFolderState(i, getSharedfolderCallback), 
                            getSharedfolderCallback);

                        object[] sharedFolderProperties = { 
                            Constants.VIX_PROPERTY_JOB_RESULT_ITEM_NAME,
                            Constants.VIX_PROPERTY_JOB_RESULT_SHARED_FOLDER_HOST,
                            Constants.VIX_PROPERTY_JOB_RESULT_SHARED_FOLDER_FLAGS
                        };

                        object[] sharedFolderPropertyValues = sharedFolderJob.Wait<object[]>(
                            sharedFolderProperties, VMWareInterop.Timeouts.GetSharedFoldersTimeout);

                        VMWareSharedFolder sharedFolder = new VMWareSharedFolder(
                            (string)sharedFolderPropertyValues[0],
                            (string)sharedFolderPropertyValues[1],
                            (int)sharedFolderPropertyValues[2]);

                        sharedFolders.Add(sharedFolder);
                    }
                    _sharedFolders = sharedFolders;
                }

                return _sharedFolders;
            }
        }

        /// <summary>
        /// Delete all shared folders.
        /// </summary>
        public void Clear() 
        {
            while (SharedFolders.Count > 0)
            {
                Remove(SharedFolders[0]);
            }
        }

        /// <summary>
        /// A function to copy shared folder objects between arrays.
        /// Don't use externally.
        /// </summary>
        /// <param name="array">target array</param>
        /// <param name="arrayIndex">array index</param>
        public void CopyTo(VMWareSharedFolder[] array, int arrayIndex) 
        { 
            SharedFolders.CopyTo(array, arrayIndex); 
        }

        /// <summary>
        /// Returns true if this virtual machine has the folder specified.
        /// </summary>
        /// <param name="item">shared folder</param>
        /// <returns>True if the virtual machine contains the specified shared folder.</returns>
        public bool Contains(VMWareSharedFolder item)
        {
            return SharedFolders.Contains(item);
        }

        /// <summary>
        /// Delete a shared folder.
        /// </summary>
        /// <param name="item">shared folder to delete</param>
        /// <returns>True if the folder was deleted.</returns>
        public bool Remove(VMWareSharedFolder item) 
        {
            VMWareJobCallback callback = new VMWareJobCallback();
            VMWareJob job = new VMWareJob(_vm.RemoveSharedFolder(
                item.ShareName, 0, callback), 
                callback);
            job.Wait(VMWareInterop.Timeouts.AddRemoveSharedFolderTimeout);
            return SharedFolders.Remove(item); 
        }

        /// <summary>
        /// Number of shared folders.
        /// </summary>
        public int Count 
        { 
            get 
            { 
                return SharedFolders.Count; 
            } 
        }

        /// <summary>
        /// Returns true if the collection is read-only.
        /// Shared folder collections are never read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// A shared folder enumerator.
        /// </summary>
        /// <returns>Shared folders enumerator.</returns>
        IEnumerator<VMWareSharedFolder> IEnumerable<VMWareSharedFolder>.GetEnumerator() 
        { 
            return SharedFolders.GetEnumerator(); 
        }

        /// <summary>
        /// A shared folder enumerator.
        /// </summary>
        /// <returns>Shared folders enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return SharedFolders.GetEnumerator();
        }
    }
}

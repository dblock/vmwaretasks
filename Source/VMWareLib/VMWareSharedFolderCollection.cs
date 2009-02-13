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

        public VMWareSharedFolderCollection(IVM vm)
        {
            _vm = vm;
        }

        /// <summary>
        /// Add a shared folder.
        /// </summary>
        /// <param name="sharedFolder">the shared folder to add</param>
        public void Add(VMWareSharedFolder sharedFolder)
        {
            VMWareJob job = new VMWareJob(_vm.AddSharedFolder(sharedFolder.ShareName, sharedFolder.HostPath, sharedFolder.Flags, null));
            job.Wait(VMWareInterop.Timeouts.AddRemoveSharedFolderTimeout);
            _sharedFolders.Add(sharedFolder);
        }

        /// <summary>
        /// Get shared folders.
        /// </summary>
        /// <returns>a list of shared folders</returns>
        private List<VMWareSharedFolder> SharedFolders
        {
            get
            {
                if (_sharedFolders == null)
                {
                    List<VMWareSharedFolder> sharedFolders = new List<VMWareSharedFolder>();
                    VMWareJob job = new VMWareJob(_vm.GetNumSharedFolders(null));
                    
                    int nSharedFolders = job.Wait<int>(
                        Constants.VIX_PROPERTY_JOB_RESULT_SHARED_FOLDER_COUNT, 
                        VMWareInterop.Timeouts.GetSharedFoldersTimeout);
                    
                    for (int i = 0; i < nSharedFolders; i++)
                    {
                        VMWareJob sharedFolderJob = new VMWareJob(_vm.GetSharedFolderState(i, null));

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

        public void CopyTo(VMWareSharedFolder[] array, int arrayIndex) 
        { 
            SharedFolders.CopyTo(array, arrayIndex); 
        }

        /// <summary>
        /// Returns true if this virtual machine has the folder specified.
        /// </summary>
        /// <param name="item">shared folder</param>
        /// <returns>true if the virtual machine contains the specified shared folder</returns>
        public bool Contains(VMWareSharedFolder item)
        {
            return SharedFolders.Contains(item);
        }

        /// <summary>
        /// Delete a shared folder.
        /// </summary>
        /// <param name="item">shared folder to delete</param>
        /// <returns>true if the folder was deleted</returns>
        public bool Remove(VMWareSharedFolder item) 
        {
            VMWareJob job = new VMWareJob(_vm.RemoveSharedFolder(item.ShareName, 0, null));
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

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        IEnumerator<VMWareSharedFolder> IEnumerable<VMWareSharedFolder>.GetEnumerator() 
        { 
            return SharedFolders.GetEnumerator(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SharedFolders.GetEnumerator();
        }
    }
}

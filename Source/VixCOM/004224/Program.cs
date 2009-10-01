using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VixCOM;

namespace VMWareCrash
{
    
    class Program
    {
        private static VixCOM.VixLib vix = new VixLib();

        /// <summary>
        /// AV demo
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                string uri = "https://tubbs.nycapt35k.com/sdk";
                string vmx = "[adpro-1] snowtest-w2k8/snowtest-w2k8.vmx";
                string username = "vmuser";
                string password = "admin123";

                // connect to a VI host
                Console.WriteLine("Connecting to {0}", uri);
                IJob connectJob = vix.Connect(Constants.VIX_API_VERSION, Constants.VIX_SERVICEPROVIDER_VMWARE_VI_SERVER,
                    uri, 0, username, password, 0, null, null);
                object[] connectProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                object hosts = null;
                ulong rc = connectJob.Wait(connectProperties, ref hosts);
                if (vix.ErrorIndicatesFailure(rc))
                {
                    ((IVixHandle2)connectJob).Close();
                    throw new Exception(vix.GetErrorText(rc, "en-US"));
                }

                IHost host = (IHost)((object[])hosts)[0];

                {
                    // open a vm
                    Console.WriteLine("Opening {0}", vmx);
                    IJob openJob = host.OpenVM(vmx, null);
                    object[] openProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                    object openResults = null;
                    rc = openJob.Wait(openProperties, ref openResults);
                    if (vix.ErrorIndicatesFailure(rc))
                    {
                        ((IVixHandle2)openJob).Close();
                        throw new Exception(vix.GetErrorText(rc, "en-US"));
                    }
                    Console.WriteLine("Opened {0}", vmx);
                    IVM2 vm = (IVM2)((object[])openResults)[0];
                    ((IVixHandle2)openJob).Close();
                    // create two snapshots
                    for (int i = 0; i < 2; i++)
                    {
                        // create a snapshot
                        string snapshotName = Guid.NewGuid().ToString();
                        VMWareJobCallback callback = new VMWareJobCallback();
                        IJob createSnapshotJob = vm.CreateSnapshot(snapshotName, snapshotName, 0, null, callback);
                        object[] createSnapshotProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                        object createSnapshotResults = null;
                        rc = createSnapshotJob.Wait(createSnapshotProperties, ref createSnapshotResults);
                        if (vix.ErrorIndicatesFailure(rc))
                        {
                            ((IVixHandle2)createSnapshotJob).Close();
                            throw new Exception(vix.GetErrorText(rc, "en-US"));
                        }
                        ISnapshot createdSnapshot = (ISnapshot)((object[])createSnapshotResults)[0];
                        ((IVixHandle2)createSnapshotJob).Close();
                        ((IVixHandle2)createdSnapshot).Close();
                    }
                    // collect all snapshots
                    Console.WriteLine("Fetching all snapshots");
                    int snapshotCount = 0;
                    rc = vm.GetNumRootSnapshots(out snapshotCount);
                    if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                    Console.WriteLine("Snapshots: {0}", snapshotCount);
                    // get all snapshots
                    for (int snapshotIndex = 0; snapshotIndex < snapshotCount; snapshotIndex++)
                    {
                        ISnapshot snapshotAtIndex = null;
                        rc = vm.GetRootSnapshot(snapshotIndex, out snapshotAtIndex);
                        if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                        ((IVixHandle2)snapshotAtIndex).Close();
                    }
                    // get root snapshot
                    Console.WriteLine("Fetching root snapshot");
                    ISnapshot snapshot = null;
                    rc = vm.GetRootSnapshot(0, out snapshot);
                    if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                    Console.WriteLine("Reverting to snapshot");
                    // revert to the snapshot
                    IJob revertJob = vm.RevertToSnapshot(snapshot, Constants.VIX_VMPOWEROP_NORMAL, null, null);
                    rc = revertJob.WaitWithoutResults();
                    ((IVixHandle2)snapshot).Close();
                    ((IVixHandle2)revertJob).Close();
                    if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                }

                // disconnect
                Console.WriteLine("Disconnecting");
                host.Disconnect();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
        }
    }
}

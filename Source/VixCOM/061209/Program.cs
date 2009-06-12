using System;
using System.Collections.Generic;
using System.Text;
using VixCOM;

namespace VMWareCrash
{
    class Program
    {
        /// <summary>
        /// Connect to both VMWare VI and ESX demo, see http://communities.vmware.com/thread/186655.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                VixCOM.VixLib vix = new VixLib();

                {
                    // connect to a VI host
                    Console.WriteLine("Connecting to VI host");
                    IJob connectJob = vix.Connect(Constants.VIX_API_VERSION, Constants.VIX_SERVICEPROVIDER_VMWARE_VI_SERVER,
                        "https://linc.nycapt35k.com/sdk/", 0, "vmuser", "admin123", 0, null, null);
                    object[] connectProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                    object hosts = null;
                    ulong rc = connectJob.Wait(connectProperties, ref hosts);
                    if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                    IHost host = (IHost)((object[])hosts)[0];

                    // open a vm
                    Console.WriteLine("Opening VM");
                    IJob openJob = host.OpenVM("[dbprotect-1] ddoub-red/ddoub-red.vmx", null);
                    object[] openProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                    object openResults = null;
                    rc = openJob.Wait(openProperties, ref openResults);
                    if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                    IVM2 vm = (IVM2) ((object[]) openResults)[0];

                    // number of snapshots
                    int nSnapshots = 0;
                    vm.GetNumRootSnapshots(out nSnapshots);
                    Console.WriteLine("Root snapshots: {0}", nSnapshots);
                    List<ISnapshot> snapshots = new List<ISnapshot>();
                    for (int i = 0; i < nSnapshots; i++)
                    {
                        Console.WriteLine("Fetching snapshot: {0}", i);
                        ISnapshot snapshot = null;
                        rc = vm.GetRootSnapshot(i, out snapshot);
                        snapshots.Add(snapshot);
                        if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                    }

                    // create a snapshot
                    Console.WriteLine("Creating snapshot");
                    VMWareJobCallback jobDoneCallback = new VMWareJobCallback();
                    IJob createSnapshotJob = vm.CreateSnapshot(
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                        0, null, jobDoneCallback);
                    jobDoneCallback.WaitForCompletion(10000);
                    rc = createSnapshotJob.WaitWithoutResults();
                    if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                    snapshots = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    // disconnect
                    Console.WriteLine("Disconnecting");
                    host.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

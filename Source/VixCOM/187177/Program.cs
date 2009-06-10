using System;
using System.Collections.Generic;
using System.Text;
using VixCOM;

namespace VMWareCrash
{
    class Program
    {
        /// <summary>
        /// AV demo, see http://communities.vmware.com//thread/187177?tstart=0.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    VixCOM.VixLib vix = new VixLib();

                    // connect to a VI host
                    Console.WriteLine("Connecting to host");
                    IJob connectJob = vix.Connect(Constants.VIX_API_VERSION, Constants.VIX_SERVICEPROVIDER_VMWARE_VI_SERVER,
                        "https://linc.nycapt35k.com/sdk/", 0, "vmuser", "admin123", 0, null, null);
                    object[] connectProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                    object hosts = null;
                    ulong rc = connectJob.Wait(connectProperties, ref hosts);
                    if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                    IHost host = (IHost)((object[])hosts)[0];

                    {
                        // open a vm
                        Console.WriteLine("Opening VM");
                        IJob openJob = host.OpenVM("[dbprotect-1] ddoub-red/ddoub-red.vmx", null);
                        object[] openProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                        object openResults = null;
                        rc = openJob.Wait(openProperties, ref openResults);
                        if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                        IVM2 vm = (IVM2)((object[])openResults)[0];
                        // get root snapshot
                        Console.WriteLine("Fetching root snapshot");
                        ISnapshot snapshot = null;
                        rc = vm.GetRootSnapshot(0, out snapshot);
                        if (vix.ErrorIndicatesFailure(rc)) throw new Exception(vix.GetErrorText(rc, "en-US"));
                        Console.WriteLine("Reverting to snapshot");
                        // revert to the snapshot
                        IJob revertJob = vm.RevertToSnapshot(snapshot, Constants.VIX_VMPOWEROP_NORMAL, null, null);
                        revertJob.WaitWithoutResults();
                    }

                    // disconnect
                    Console.WriteLine("Disconnecting");
                    host.Disconnect();

                    Console.WriteLine("GC");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

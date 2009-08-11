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
            int iterationNo = 1;
            while (true)
            {
                IHost host = null;
                IVM2 vm = null;

                try
                {
                    VixCOM.VixLib vix = new VixLib();
                    Console.WriteLine("**** Iteration: " + iterationNo + "*******");
                    iterationNo++;
                    // connect to a VI host
                    Console.WriteLine("Connecting to host");
                    IJob connectJob = vix.Connect(Constants.VIX_API_VERSION, Constants.VIX_SERVICEPROVIDER_VMWARE_VI_SERVER,
                        "https://linc.nycapt35k.com/sdk/", 0, "vmuser", "admin123", 0, null, null);

                    object[] connectProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                    object hosts = null;
                    ulong rc = connectJob.Wait(connectProperties, ref hosts);
                    if (vix.ErrorIndicatesFailure(rc))
                    {
                        ((IVixHandle2) connectJob).Close();
                        throw new Exception(vix.GetErrorText(rc, "en-US"));
                    }

                    host = (IHost)((object[])hosts)[0];
                    ((IVixHandle2)connectJob).Close();

                    {
                        // open a vm
                        Console.WriteLine("Opening VM");
                        IJob openJob = host.OpenVM("[dbprotect-1] ddoub-purple/ddoub-purple.vmx", null);
                        object[] openProperties = { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE };
                        object openResults = null;
                        rc = openJob.Wait(openProperties, ref openResults);
                        if (vix.ErrorIndicatesFailure(rc))
                        {
                            ((IVixHandle2) openJob).Close();
                            throw new Exception(vix.GetErrorText(rc, "en-US"));
                        }

                        vm = (IVM2)((object[])openResults)[0];
                        ((IVixHandle2)openJob).Close();

                        // get root snapshot
                        Console.WriteLine("Fetching root snapshot");
                        ISnapshot snapshot = null;
                        rc = vm.GetRootSnapshot(0, out snapshot);
                        if (vix.ErrorIndicatesFailure(rc))
                        {
                            ((IVixHandle2)openJob).Close();
                            throw new Exception(vix.GetErrorText(rc, "en-US"));
                        }
                        Console.WriteLine("Reverting to snapshot");
                        // revert to the snapshot
                        IJob revertJob = vm.RevertToSnapshot(snapshot, Constants.VIX_VMPOWEROP_NORMAL, null, null);
                        revertJob.WaitWithoutResults();
                        ((IVixHandle2)revertJob).Close();
                        ((IVixHandle2)snapshot).Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (null != vm)
                    {
						Console.WriteLine("Closing VM");
                        ((IVixHandle2) vm).Close();
                    }

                    if (null != host)
                    {
                        Console.WriteLine("Disconnecting");
                        host.Disconnect();
                    }

                    Console.WriteLine("GC");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                }
            }
        }
    }
}

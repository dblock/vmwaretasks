using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Vestris.VMWareLib.Tools.Windows
{
    /// <summary>
    /// A shell wrapper capable of executing remote commands on Microsoft Windows.
    /// </summary>
    public class Shell : GuestOS
    {
        /// <summary>
        /// Shell output.
        /// </summary>
        public struct ShellOutput
        {
            /// <summary>
            /// Standard output.
            /// </summary>
            public string StdOut;
            /// <summary>
            /// Standard error.
            /// </summary>
            public string StdErr;
        }

        /// <summary>
        /// New instance of a shell wrapper object.
        /// </summary>
        /// <param name="vm">Powered virtual machine.</param>
        public Shell(VMWareVirtualMachine vm)
            : base(vm)
        {

        }

        /// <summary>
        /// Use RunProgramInGuest to execute cmd.exe /C "guestCommandLine" > file and parse the result.
        /// </summary>
        /// <param name="guestCommandLine">Guest command line, argument passed to cmd.exe.</param>
        /// <returns>Standard output.</returns>
        public ShellOutput RunCommandInGuest(string guestCommandLine)
        {
            string guestFilename = string.Format("VMWareTools.{0}", Guid.NewGuid());
            string guestStdOutFilename = _vm.CreateTempFileInGuest();
            string guestStdErrFilename = _vm.CreateTempFileInGuest();
            string hostCommandBatch = Path.GetTempFileName();
            StringBuilder hostCommand = new StringBuilder();
            hostCommand.AppendLine("@echo off");
            hostCommand.AppendLine(guestCommandLine);
            File.WriteAllText(hostCommandBatch, hostCommand.ToString());
            string guestTempPath = _vm.GuestEnvironmentVariables["tmp"];
            string guestCommandBatch = Path.Combine(guestTempPath, guestFilename + ".bat");
            try
            {
                _vm.CopyFileFromHostToGuest(hostCommandBatch, guestCommandBatch);
                string cmdArgs = string.Format("> \"{0}\" 2>\"{1}\"", guestStdOutFilename, guestStdErrFilename);
                _vm.RunProgramInGuest(guestCommandBatch, cmdArgs);
                ShellOutput output = new ShellOutput();
                output.StdOut = ReadFile(guestStdOutFilename);
                output.StdErr = ReadFile(guestStdErrFilename);
                return output;
            }
            finally
            {
                File.Delete(hostCommandBatch);
                _vm.DeleteFileFromGuest(guestCommandBatch);
                _vm.DeleteFileFromGuest(guestStdOutFilename);
                _vm.DeleteFileFromGuest(guestStdErrFilename);
            }
        }

        /// <summary>
        /// Returns environment variables parsed from the output of a set command.
        /// </summary>
        /// <returns>Environment variables.</returns>
        /// <example>
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using Vestris.VMWareLib;
        /// using Vestris.VMWareLib.Tools.Windows;
        /// 
        /// VMWareVirtualHost virtualHost = new VMWareVirtualHost();
        /// virtualHost.ConnectToVMWareWorkstation();
        /// VMWareVirtualMachine virtualMachine = virtualHost.Open("C:\Virtual Machines\xp\xp.vmx");
        /// virtualMachine.PowerOn();
        /// virtualMachine.WaitForToolsInGuest();
        /// virtualMachine.LoginInGuest("Administrator", "password");
        /// Shell guestShell = new Shell(poweredVirtualMachine);
        /// Dictionary&lt;string, string&gt; guestEnvironmentVariables = guestShell.GetEnvironmentVariables();
        /// Console.WriteLine(guestEnvironmentVariables["ProgramFiles"]);
        /// </code>
        /// </example>
        public Dictionary<string, string> GetEnvironmentVariables()
        {
            Dictionary<string, string> environmentVariables = new Dictionary<string, string>();
            StringReader sr = new StringReader(RunCommandInGuest("set").StdOut);
            string line = null;
            while (! string.IsNullOrEmpty(line = sr.ReadLine()))
            {
                string[] nameValuePair = line.Split("=".ToCharArray(), 2);
                if (nameValuePair.Length != 2)
                {
                    throw new Exception(string.Format("Invalid environment string: \"{0}\"", line));
                }

                environmentVariables[nameValuePair[0]] = nameValuePair[1];
            }
            return environmentVariables;
        }
    }
}

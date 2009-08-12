using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib.Tools
{
    [ComVisible(true)]
    [Guid("CF4D25D6-611F-4de2-8C18-69F4E25E5FF0")]
    [ComDefaultInterface(typeof(IWindowsShell))]
    [ProgId("VMWareComTools.WindowsShell")]
    public class WindowsShell : IWindowsShell
    {
        [ComVisible(true)]
        [Guid("34165343-568C-4ad4-83AA-A9E9A873DFFD")]
        [ComDefaultInterface(typeof(IShellOutput))]
        [ProgId("VMWareComTools.WindowsShell.ShellOutput")]
        public class ShellOutput : IShellOutput
        {
            private string _stdout;
            private string _stderr;

            public ShellOutput(string stdout, string stderr)
            {
                _stdout = stdout;
                _stderr = stderr;
            }

            public string StdOut 
            { 
                get
                {
                    return _stdout;
                }
            }

            public string StdErr
            {
                get
                {
                    return _stderr;
                }
            }
        }

        private GuestOS _guestos = new GuestOS();

        public VMWareComLib.IVMWareVirtualMachine VirtualMachine
        {
            get
            {
                return _guestos.VirtualMachine;
            }
            set
            {
                _guestos.VirtualMachine = value;
            }
        }        

        /// <summary>
        /// Use RunProgramInGuest to execute cmd.exe /C "guestCommandLine" > file and parse the result.
        /// </summary>
        /// <param name="guestCommandLine">Guest command line, argument passed to cmd.exe.</param>
        /// <returns>Standard output.</returns>
        public IShellOutput RunCommandInGuest(string guestCommandLine)
        {
            string guestFilename = string.Format("VMWareComTools.{0}", Guid.NewGuid());
            string guestStdOutFilename = _guestos.VirtualMachine.CreateTempFileInGuest();
            string guestStdErrFilename = _guestos.VirtualMachine.CreateTempFileInGuest();
            string hostCommandBatch = Path.GetTempFileName();
            StringBuilder hostCommand = new StringBuilder();
            hostCommand.AppendLine("@echo off");
            hostCommand.AppendLine(guestCommandLine);
            File.WriteAllText(hostCommandBatch, hostCommand.ToString());
            string guestTempPath = _guestos.VirtualMachine.GuestEnvironmentVariables["tmp"];
            string guestCommandBatch = Path.Combine(guestTempPath, guestFilename + ".bat");
            try
            {
                _guestos.VirtualMachine.CopyFileFromHostToGuest(hostCommandBatch, guestCommandBatch);
                string cmdArgs = string.Format("> \"{0}\" 2>\"{1}\"", guestStdOutFilename, guestStdErrFilename);
                _guestos.VirtualMachine.RunProgramInGuest(guestCommandBatch, cmdArgs);
                ShellOutput output = new ShellOutput(
                    _guestos.ReadFile(guestStdOutFilename), 
                    _guestos.ReadFile(guestStdErrFilename));
                return output;
            }
            finally
            {
                File.Delete(hostCommandBatch);
                _guestos.VirtualMachine.DeleteFileFromGuest(guestCommandBatch);
                _guestos.VirtualMachine.DeleteFileFromGuest(guestStdOutFilename);
                _guestos.VirtualMachine.DeleteFileFromGuest(guestStdErrFilename);
            }
        }
    }
}

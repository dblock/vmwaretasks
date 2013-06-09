using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Interop.VixCOM;
using System.Diagnostics;

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
            return RunCommandInGuest(guestCommandLine, VMWareInterop.Timeouts.RunProgramTimeout);
        }

        /// <summary>
        /// Use RunProgramInGuest to execute cmd.exe /C "guestCommandLine" > file and parse the result.
        /// </summary>
        /// <param name="guestCommandLine">Guest command line, argument passed to cmd.exe.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>Standard output.</returns>
        public ShellOutput RunCommandInGuest(string guestCommandLine, int timeoutInSeconds)
        {
            return RunCommandInGuest(guestCommandLine, Constants.VIX_RUNPROGRAM_ACTIVATE_WINDOW,
                timeoutInSeconds);
        }

        /// <summary>
        /// Use RunProgramInGuest to execute cmd.exe /C "guestCommandLine" > file and parse the result.
        /// </summary>
        /// <param name="guestCommandLine">The guest command line.</param>
        /// <param name="options">The options.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns></returns>
        public ShellOutput RunCommandInGuest(string guestCommandLine, int options, int timeoutInSeconds)
        {
            string guestStdOutFilename = _vm.CreateTempFileInGuest();
            string guestStdErrFilename = _vm.CreateTempFileInGuest();
            string guestCommandBatch = _vm.CreateTempFileInGuest() + ".bat";
            string hostCommandBatch = Path.GetTempFileName();
            StringBuilder hostCommand = new StringBuilder();
            hostCommand.AppendLine("@echo off");
            hostCommand.AppendLine(guestCommandLine);
            File.WriteAllText(hostCommandBatch, hostCommand.ToString());

            bool exceptionThrown = false;

            try
            {
                _vm.CopyFileFromHostToGuest(hostCommandBatch, guestCommandBatch);
                string cmdArgs = string.Format("> \"{0}\" 2>\"{1}\"", guestStdOutFilename, guestStdErrFilename);
                _vm.RunProgramInGuest(guestCommandBatch, cmdArgs, options, timeoutInSeconds);
                ShellOutput output = new ShellOutput();
                output.StdOut = ReadFile(guestStdOutFilename);
                output.StdErr = ReadFile(guestStdErrFilename);
                return output;
            }
            catch (Exception)
            {
                exceptionThrown = true;
                throw;
            }
            finally
            {
                File.Delete(hostCommandBatch);

                //do not swallow exceptions
                RunOptionallyThrowingException(() =>
                {
                    _vm.DeleteFileFromGuest(guestCommandBatch);
                }, !exceptionThrown);

                RunOptionallyThrowingException(() =>
                {
                    _vm.DeleteFileFromGuest(guestStdOutFilename);
                }, !exceptionThrown);

                RunOptionallyThrowingException(() =>
                {
                    _vm.DeleteFileFromGuest(guestStdErrFilename);
                }, !exceptionThrown);
            }
        }

        private delegate void AnonymousMethod();

        private void RunOptionallyThrowingException(AnonymousMethod action, bool throwException)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());

                if (throwException)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns environment variables parsed from the output of a set command.
        /// </summary>
        /// <returns>Environment variables.</returns>
        /// <example>
        /// <para>
        /// The following example retrieves the ProgramFiles environment variable from the guest operating system.
        /// <code language="cs" source="..\Source\VMWareToolsSamples\WindowsShellSamples.cs" region="Example: Enumerating Environment Variables on the GuestOS without VixCOM" />
        /// </para>
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
                    throw new VMWareInvalidEnvironmentStringException(line);
                }

                environmentVariables[nameValuePair[0]] = nameValuePair[1];
            }
            return environmentVariables;
        }
    }
}

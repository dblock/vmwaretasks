using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Execute a program in the guest operating system.
    /// </summary>
    public class VirtualMachineRunProgramInGuest : VirtualMachineLoginGuest
    {
        private int _runProgramTimeout = VMWareInterop.Timeouts.RunProgramTimeout;
        private string _guestProgramName;
        private string _commandLineArgs;
        private VMWareVirtualMachine.Process _process;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int RunProgramTimeout
        {
            get
            {
                return _runProgramTimeout;
            }
            set
            {
                _runProgramTimeout = value;
            }
        }

        /// <summary>
        /// Program name on the guest operating system to run.
        /// </summary>
        public string GuestProgramName
        {
            get
            {
                return _guestProgramName;
            }
            set
            {
                _guestProgramName = value;
            }
        }

        /// <summary>
        /// Command-line arguments.
        /// </summary>
        public string CommandLineArgs
        {
            get
            {
                return _commandLineArgs;
            }
            set
            {
                _commandLineArgs = value;
            }
        }

        /// <summary>
        /// Process ID.
        /// </summary>
        [Output]
        public long ProcessId
        {
            get
            {
                return _process.Id;
            }
        }

        /// <summary>
        /// Process name.
        /// </summary>
        [Output]
        public string ProcessName
        {
            get
            {
                return _process.Name;
            }
        }

        /// <summary>
        /// Process owner.
        /// </summary>
        [Output]
        public string ProcessOwner
        {
            get
            {
                return _process.Owner;
            }
        }
        
        /// <summary>
        /// Process start date/time.
        /// </summary>
        [Output]
        public string ProcessStartDateTime
        {
            get
            {
                return _process.StartDateTime.ToString();
            }
        }

        /// <summary>
        /// Process command line.
        /// </summary>
        [Output]
        public string ProcessCommand
        {
            get
            {
                return _process.Command;
            }
        }

        /// <summary>
        /// True if process is being debugged.
        /// </summary>
        [Output]
        public bool IsProcessBeingDebugged
        {
            get
            {
                return _process.IsBeingDebugged;
            }
        }
        
        /// <summary>
        /// Process exit code for finished processes.
        /// </summary>
        [Output]
        public int ProcessExitCode
        {
            get
            {
                return _process.ExitCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    LoginGuest(virtualMachine);
                    Log.LogMessage(string.Format("Running '{0}{1}'", _guestProgramName, 
                        string.IsNullOrEmpty(_commandLineArgs)
                            ? string.Empty 
                            : " " + _commandLineArgs));
                    _process = virtualMachine.RunProgramInGuest(_guestProgramName, _commandLineArgs, 0, _runProgramTimeout);
                }
            }

            return true;
        }
    }
}

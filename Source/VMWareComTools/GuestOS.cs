using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Vestris.VMWareComLib.Tools
{
    /// <summary>
    /// Default implementation of the <see cref="Vestris.VMWareComLib.Tools.IGuestOS" /> COM interface.
    /// </summary>
    [ComVisible(true)]
    [Guid("B344E97A-57E1-41d9-BFE0-28213BA5A419")]
    [ComDefaultInterface(typeof(IGuestOS))]
    [ProgId("VMWareComTools.GuestOS")]
    public class GuestOS : IGuestOS
    {
        private Vestris.VMWareComLib.IVMWareVirtualMachine _vm = null;

        public GuestOS()
        {
        }

        public VMWareComLib.IVMWareVirtualMachine VirtualMachine
        {
            get
            {
                return _vm;
            }
            set
            {
                _vm = value;
            }
        }

        /// <summary>
        /// Remote IP address.
        /// </summary>
        public string IpAddress
        {
            get
            {
                return _vm.GuestVariables["ip"];
            }
        }

        /// <summary>
        /// Read a file in the guest operating system.
        /// </summary>
        /// <param name="guestFilename">File in the guest operating system.</param>
        /// <returns>File contents as a string.</returns>
        public string ReadFile(string guestFilename)
        {
            return ReadFile(guestFilename, Encoding.Default);
        }

        /// <summary>
        /// Read a file in the guest operating system.
        /// </summary>
        /// <param name="guestFilename">File in the guest operating system.</param>
        /// <param name="encoding">Encoding applied to the file contents.</param>
        /// <returns>File contents as a string.</returns>
        public string ReadFile(string guestFilename, Encoding encoding)
        {
            string tempFilename = Path.GetTempFileName();
            try
            {
                _vm.CopyFileFromGuestToHost(guestFilename, tempFilename);
                return File.ReadAllText(tempFilename, encoding);
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }

        /// <summary>
        /// Read a file in the guest operating system.
        /// </summary>
        /// <param name="guestFilename">File in the guest operating system.</param>
        /// <returns>File contents as bytes.</returns>
        public byte[] ReadFileBytes(string guestFilename)
        {
            string tempFilename = Path.GetTempFileName();
            try
            {
                _vm.CopyFileFromGuestToHost(guestFilename, tempFilename);
                return File.ReadAllBytes(tempFilename);
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }

        /// <summary>
        /// Read a file in the guest operating system.
        /// </summary>
        /// <param name="guestFilename">File in the guest operating system.</param>
        /// <returns>File contents, line-by-line.</returns>
        public string[] ReadFileLines(string guestFilename)
        {
            return ReadFileLines(guestFilename, Encoding.Default);
        }

        /// <summary>
        /// Read a file in the guest operating system.
        /// </summary>
        /// <param name="guestFilename">File in the guest operating system.</param>
        /// <param name="encoding">Encoding applied to the file contents.</param>
        /// <returns>File contents, line-by-line.</returns>
        public string[] ReadFileLines(string guestFilename, Encoding encoding)
        {
            string tempFilename = Path.GetTempFileName();
            try
            {
                _vm.CopyFileFromGuestToHost(guestFilename, tempFilename);
                return File.ReadAllLines(tempFilename, encoding);
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }
    }
}

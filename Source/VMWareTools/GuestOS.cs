using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Vestris.VMWareLib.Tools
{
    /// <summary>
    /// Generic remote operating system wrapper.
    /// </summary>
    public class GuestOS
    {
        /// <summary>
        /// Virtual Machine instance.
        /// </summary>
        protected VMWareVirtualMachine _vm = null;

        /// <summary>
        /// New instance of a guest operating system wrapper.
        /// </summary>
        /// <param name="vm">Powered virtual machine.</param>
        public GuestOS(VMWareVirtualMachine vm)
        {
            _vm = vm;
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

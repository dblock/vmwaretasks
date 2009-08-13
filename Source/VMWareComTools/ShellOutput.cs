using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.VMWareComLib.Tools
{
    /// <summary>
    /// Default implementation of the <see cref="Vestris.VMWareComLib.Tools.IShellOutput" /> COM interface.
    /// </summary>
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
}

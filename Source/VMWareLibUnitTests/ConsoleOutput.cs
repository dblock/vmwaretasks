using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Vestris.VMWareLibUnitTests
{
    /// <summary>
    /// A thread-safe console output appender that identifies output threads.
    /// The first thread (main thread) is output without ID or with a default id.
    /// </summary>
    public static class ConsoleOutput
    {
        private static ReaderWriterLock _threadIDsLock = new ReaderWriterLock();
        private static Dictionary<int, int> _threadIDs = new Dictionary<int, int>();
        private static object _lock = new object();
        private static bool _showThreadID = true;

        /// <summary>
        /// Show thread ID.
        /// </summary>
        public static bool ShowThreadID
        {
            get
            {
                return _showThreadID;
            }
            set
            {
                _showThreadID = value;
            }
        }

        /// <summary>
        /// The relative thread ID, starting at zero.
        /// </summary>
        private static int GetThreadID()
        {
            _threadIDsLock.AcquireReaderLock(-1);
            try
            {
                int id = 0;
                if (!_threadIDs.TryGetValue(Thread.CurrentThread.ManagedThreadId, out id))
                {
                    LockCookie cookie = _threadIDsLock.UpgradeToWriterLock(-1);
                    id = _threadIDs.Count;
                    try
                    {
                        _threadIDs[Thread.CurrentThread.ManagedThreadId] = id;
                    }
                    finally
                    {
                        _threadIDsLock.DowngradeFromWriterLock(ref cookie);
                    }
                }

                return id;
            }
            finally
            {
                _threadIDsLock.ReleaseReaderLock();
            }
        }

        private static string StringFormat(string s)
        {
            int id = GetThreadID();
            if (id == 0 || ! _showThreadID) return s;
            return string.Format("{0}: {1}", id, s);
        }

        public static void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public static void WriteLine(string s)
        {
            lock (_lock)
            {
                Console.WriteLine(StringFormat(s));
            }
        }

        public static void WriteLine(string format, params object[] arg)
        {
            WriteLine(string.Format(format, arg));
        }
    }
}

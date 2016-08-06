using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace JG.Utility
{
    /// <summary>
    ///     This class facilitates finding open windows whether they be in WPF or Winforms, since it would normally be
    ///     difficult to find them from one another.
    /// </summary>
    public class WindowInfo
    {
        private const uint WmGettext = 0xd;
        private readonly IntPtr _handle;
        private readonly string _title;

        public WindowInfo(IntPtr handle, string title)
        {
            _handle = handle;
            _title = title;
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }

        public string Title
        {
            get { return _title; }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadWindowsDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, StringBuilder lParam);

        public static IEnumerable<WindowInfo> EnumerateProcessWindows(int processId)
        {
            var windows = new List<WindowInfo>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
            {
                EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                {
                    if (IsWindowVisible(hWnd))
                    {
                        var message = new StringBuilder(1000);

                        SendMessage(hWnd, WmGettext, message.Capacity, message);

                        var title = message.ToString();
                        if (!string.IsNullOrWhiteSpace(title))
                        {
                            windows.Add(new WindowInfo(hWnd, title));
                        }
                    }
                    return true;
                }, IntPtr.Zero);
            }

            return windows;
        }

        private delegate bool EnumThreadWindowsDelegate(IntPtr hWnd, IntPtr lParam);
    }
}
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JG.Forms
{
    public partial class AppBar : Form
    {
        private readonly FormBorderStyle defaultFormBorderStyle;
        private int extendedWindowStyle;
        private bool previousMaximizeBox;
        private bool previousMinimizeBox;
        private FormWindowState previousWindowState;

        public AppBar()
        {
            InitializeComponent();

            if (UseCustomFormBorderStyle)
            {
                FormBorderStyle = FormBorderStyle.None;
                previousWindowState = WindowState;
            }

            defaultFormBorderStyle = FormBorderStyle;

            if (UseCustomFormBorderStyle && Resizable)
            {
                DoubleBuffered = true;
                SetStyle(ControlStyles.ResizeRedraw, true);
            }
        }

        protected virtual bool UseCustomFormBorderStyle
        {
            get { return false; }
        }

        protected virtual bool Resizable
        {
            get { return true; }
        }

        // We won't paint a custom title bar so that the inheriting form has full control over it, but the size will specify the area within which we handle the events as if it were a real title bar
        protected virtual int CustomTitleBarSize
        {
            get { return 32; }
        }

        protected virtual int CustomFormCurveRadius
        {
            get { return 5; }
        }

        protected virtual int CustomFormMargin
        {
            get { return 0; }
        }

        private void AppBar_Load(object sender, EventArgs e)
        {
            if (UseCustomFormBorderStyle)
            {
                // MaximumSize is needed when UseCustomFormBorderStyle==true because otherwise the window will not respect the taskbar and will fill the entire screen.
                // We will also set it every time a move is finished in case the window was moved to a different screen.
                MaximumSize = Screen.FromHandle(Handle).WorkingArea.Size;
            }
        }

        protected void DockWindow(Edge edge)
        {
            WindowState = FormWindowState.Normal;

            if (!appBarIsRegistered)
            {
                if (edge == Edge.Left || edge == Edge.Right)
                    previousHeightOrWidth = Height;
                else
                    previousHeightOrWidth = Width;

                previousMaximizeBox = MaximizeBox;
                previousMinimizeBox = MinimizeBox;
            }
            if (!appBarIsRegistered || currentEdge != edge)
            {
                if (
                    ((currentEdge == Edge.Left || currentEdge == Edge.Right) &&
                     (edge == Edge.Top || edge == Edge.Bottom))
                    ||
                    ((currentEdge == Edge.Top || currentEdge == Edge.Bottom) &&
                     (edge == Edge.Left || edge == Edge.Right))
                    )
                {
                    // If switching between being docked left/right <-> top/bottom then we need to unregister first, because the appbar rectangle is changing and Windows needs to allocate new space for it.
                    UnregisterBar();
                }

                if (!UseCustomFormBorderStyle)
                {
                    // For some reason Windows only seems to allow appbars which are a ToolWindow
                    if (Resizable)
                        FormBorderStyle = FormBorderStyle.SizableToolWindow;
                    else
                        FormBorderStyle = FormBorderStyle.FixedToolWindow;

                    // Maximizing while docked doesn't work quite right when we're using a normal form style. So just disable it while docked.
                    MaximizeBox = false;
                    MinimizeBox = false;
                }
                else
                {
                    // Re-set our Extended Window Styles, with all but WS_EX_TOOLWINDOW because it would prevent our window from moving into the registered appbar space.
                    // Fortunately the taskbar icon (which is the reason I'm setting WS_EX_TOOLWINDOW in CreateParams) somehow continues to remain and work after this. Yippee!
                    SetWindowLong(Handle, -20, extendedWindowStyle); // -20 == GWL_EXSTYLE
                }

                RegisterBar(edge);
            }
        }

        protected void UndockWindow()
        {
            if (appBarIsRegistered)
            {
                UnregisterBar();

                if (!UseCustomFormBorderStyle)
                {
                    FormBorderStyle = defaultFormBorderStyle;
                }

                if (currentEdge == Edge.Left || currentEdge == Edge.Right)
                    Height = previousHeightOrWidth;
                else
                    Width = previousHeightOrWidth;

                MaximizeBox = previousMaximizeBox;
                MinimizeBox = previousMinimizeBox;
            }
        }

        protected virtual void OnWindowStateChanged()
        {
        }

        protected enum Edge
        {
            Left = 0,
            Top,
            Right,
            Bottom
        }

        #region Resizing, Moving and Painting (including border and title bar)

        private const int gripSize = 16;
        private bool moveInProgress;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;

                if (UseCustomFormBorderStyle)
                {
                    cp.Style &= ~0x00C00000; // WS_CAPTION                    
                    cp.Style &= ~0x00800000; // WS_BORDER
                    cp.ExStyle = 0x00000080; // WS_EX_TOOLWINDOW

                    // Set extendedWindowStyle with all extended window styles except for WS_EX_APPWINDOW, because we have to remove WS_EX_APPWINDOW before we dock the AppBar.
                    extendedWindowStyle = cp.ExStyle;

                    cp.ExStyle &= 0x00040000; // WS_EX_APPWINDOW
                }

                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (UseCustomFormBorderStyle)
            {
                if (CustomFormCurveRadius > 0 || CustomFormMargin > 0)
                {
                    // Rounded corners
                    Region =
                        Region.FromHrgn(CreateRoundRectRgn(CustomFormMargin, CustomFormMargin, Width - CustomFormMargin,
                            Height - CustomFormMargin, CustomFormCurveRadius, CustomFormCurveRadius));
                }
            }
        }

        private void AppBar_ResizeEnd(object sender, EventArgs e)
        {
            // Note that Resize_End gets called when a Move is finished, not just a Resize

            if (appBarIsRegistered)
            {
                if (moveInProgress)
                {
                    // If they dragged the window away from the docked area, unregister the appbar.
                    UndockWindow();
                }
                else
                {
                    // We allowed the user to resize while the form is docked, so we need to re-register the appbar for Windows to allocate the right space
                    UnregisterBar();
                    RegisterBar(currentEdge);
                }
            }

            if (moveInProgress && UseCustomFormBorderStyle)
            {
                // MaximumSize is needed when UseCustomFormBorderStyle==true because otherwise the window will not respect the taskbar and will fill the entire screen.
                // Set it every time a move is finished in case the window was moved to a different screen.
                MaximumSize = Screen.FromHandle(Handle).WorkingArea.Size;
            }

            moveInProgress = false;
        }

        private void AppBar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (appBarIsRegistered)
            {
                UnregisterBar();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0216) // WM_MOVING
            {
                // We need to detect when the window is being moved so that we handle it properly in Resize_End which is called when it's finished moving
                moveInProgress = true;
            }

            else if (m.Msg == 0x0005) // WM_SIZE
            {
                // If the WindowState changed then unregister the appbar
                // This is intended for when it gets maximized while docked; there's no reason for it to stay docked as well as maximized.
                if (previousWindowState != WindowState)
                {
                    previousWindowState = WindowState;
                    if (appBarIsRegistered)
                    {
                        UndockWindow();
                    }
                    OnWindowStateChanged();
                }
            }

            else if (m.Msg == 0x84) // WM_NCHITTEST
            {
                var pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = PointToClient(pos);

                if (UseCustomFormBorderStyle && Resizable)
                {
                    // We have to manually tell Windows to use a resize cursor if the mouse is within the appropriate bounds, and also if it makes sense to allow it depending on which edge we are docked on.
                    // For example we won't allow resizing by the dragging the left edge when we are docked on the left side; in that case you should only be able to resize from the right.

                    if (pos.X <= gripSize)
                    {
                        // Left edge or corners
                        if (pos.Y <= gripSize)
                        {
                            // Top left
                            if (!appBarIsRegistered)
                            {
                                m.Result = (IntPtr) HitTest.HTTOPLEFT;
                                return;
                            }
                            switch (currentEdge)
                            {
                                case Edge.Right:
                                    m.Result = (IntPtr) HitTest.HTLEFT;
                                    return;
                                case Edge.Bottom:
                                    m.Result = (IntPtr) HitTest.HTTOP;
                                    return;
                            }
                        }

                        else if (pos.Y >= ClientSize.Height - gripSize)
                        {
                            // Bottom left
                            if (!appBarIsRegistered)
                            {
                                m.Result = (IntPtr) HitTest.HTBOTTOMLEFT;
                                return;
                            }
                            switch (currentEdge)
                            {
                                case Edge.Right:
                                    m.Result = (IntPtr) HitTest.HTLEFT;
                                    return;
                                case Edge.Top:
                                    m.Result = (IntPtr) HitTest.HTBOTTOM;
                                    return;
                            }
                        }

                        else
                        {
                            // Left edge
                            if (!appBarIsRegistered)
                            {
                                m.Result = (IntPtr) HitTest.HTLEFT;
                                return;
                            }
                            if (currentEdge == Edge.Right)
                            {
                                m.Result = (IntPtr) HitTest.HTLEFT;
                                return;
                            }
                        }
                    }

                    else if (pos.X >= ClientSize.Width - gripSize)
                    {
                        // Right edge or corners
                        if (pos.Y <= gripSize)
                        {
                            // Top right
                            if (!appBarIsRegistered)
                            {
                                m.Result = (IntPtr) HitTest.HTTOPRIGHT;
                                return;
                            }
                            switch (currentEdge)
                            {
                                case Edge.Left:
                                    m.Result = (IntPtr) HitTest.HTRIGHT;
                                    return;
                                case Edge.Bottom:
                                    m.Result = (IntPtr) HitTest.HTTOP;
                                    return;
                            }
                        }

                        else if (pos.Y >= ClientSize.Height - gripSize)
                        {
                            // Bottom right
                            if (!appBarIsRegistered)
                            {
                                m.Result = (IntPtr) HitTest.HTBOTTOMRIGHT;
                                return;
                            }
                            switch (currentEdge)
                            {
                                case Edge.Left:
                                    m.Result = (IntPtr) HitTest.HTRIGHT;
                                    return;
                                case Edge.Top:
                                    m.Result = (IntPtr) HitTest.HTBOTTOM;
                                    return;
                            }
                        }

                        else
                        {
                            // Right edge
                            if (!appBarIsRegistered)
                            {
                                m.Result = (IntPtr) HitTest.HTRIGHT;
                                return;
                            }
                            if (currentEdge == Edge.Left)
                            {
                                m.Result = (IntPtr) HitTest.HTRIGHT;
                                return;
                            }
                        }
                    }

                    else if (pos.Y <= gripSize)
                    {
                        // Top edge
                        if (!appBarIsRegistered)
                        {
                            m.Result = (IntPtr) HitTest.HTTOP;
                            return;
                        }
                        if (currentEdge == Edge.Bottom)
                        {
                            m.Result = (IntPtr) HitTest.HTTOP;
                            return;
                        }
                    }

                    else if (pos.Y >= ClientSize.Height - gripSize)
                    {
                        // Bottom edge
                        if (!appBarIsRegistered)
                        {
                            m.Result = (IntPtr) HitTest.HTBOTTOM;
                            return;
                        }
                        if (currentEdge == Edge.Top)
                        {
                            m.Result = (IntPtr) HitTest.HTBOTTOM;
                            return;
                        }
                    }
                }

                if (UseCustomFormBorderStyle && CustomTitleBarSize > 0)
                {
                    if (pos.Y < CustomTitleBarSize)
                    {
                        m.Result = (IntPtr) HitTest.HTCAPTION;
                        return;
                    }
                }
            }

            base.WndProc(ref m);
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        /// <summary>
        ///     Indicates the position of the cursor hot spot.
        /// </summary>
        private enum HitTest
        {
            /// <summary>
            ///     On the screen background or on a dividing line between windows (same as HTNOWHERE, except that the DefWindowProc
            ///     function produces a system beep to indicate an error).
            /// </summary>
            HTERROR = -2,

            /// <summary>
            ///     In a window currently covered by another window in the same thread (the message will be sent to underlying windows
            ///     in the same thread until one of them returns a code that is not HTTRANSPARENT).
            /// </summary>
            HTTRANSPARENT = -1,

            /// <summary>
            ///     On the screen background or on a dividing line between windows.
            /// </summary>
            HTNOWHERE = 0,

            /// <summary>
            ///     In a client area.
            /// </summary>
            HTCLIENT = 1,

            /// <summary>
            ///     In a title bar.
            /// </summary>
            HTCAPTION = 2,

            /// <summary>
            ///     In a window menu or in a Close button in a child window.
            /// </summary>
            HTSYSMENU = 3,

            /// <summary>
            ///     In a size box (same as HTSIZE).
            /// </summary>
            HTGROWBOX = 4,

            /// <summary>
            ///     In a size box (same as HTGROWBOX).
            /// </summary>
            HTSIZE = 4,

            /// <summary>
            ///     In a menu.
            /// </summary>
            HTMENU = 5,

            /// <summary>
            ///     In a horizontal scroll bar.
            /// </summary>
            HTHSCROLL = 6,

            /// <summary>
            ///     In the vertical scroll bar.
            /// </summary>
            HTVSCROLL = 7,

            /// <summary>
            ///     In a Minimize button.
            /// </summary>
            HTMINBUTTON = 8,

            /// <summary>
            ///     In a Minimize button.
            /// </summary>
            HTREDUCE = 8,

            /// <summary>
            ///     In a Maximize button.
            /// </summary>
            HTMAXBUTTON = 9,

            /// <summary>
            ///     In a Maximize button.
            /// </summary>
            HTZOOM = 9,

            /// <summary>
            ///     In the left border of a resizable window (the user can click the mouse to resize the window horizontally).
            /// </summary>
            HTLEFT = 10,

            /// <summary>
            ///     In the right border of a resizable window (the user can click the mouse to resize the window horizontally).
            /// </summary>
            HTRIGHT = 11,

            /// <summary>
            ///     In the upper-horizontal border of a window.
            /// </summary>
            HTTOP = 12,

            /// <summary>
            ///     In the upper-left corner of a window border.
            /// </summary>
            HTTOPLEFT = 13,

            /// <summary>
            ///     In the upper-right corner of a window border.
            /// </summary>
            HTTOPRIGHT = 14,

            /// <summary>
            ///     In the lower-horizontal border of a resizable window (the user can click the mouse to resize the window
            ///     vertically).
            /// </summary>
            HTBOTTOM = 15,

            /// <summary>
            ///     In the lower-left corner of a border of a resizable window (the user can click the mouse to resize the window
            ///     diagonally).
            /// </summary>
            HTBOTTOMLEFT = 16,

            /// <summary>
            ///     In the lower-right corner of a border of a resizable window (the user can click the mouse to resize the window
            ///     diagonally).
            /// </summary>
            HTBOTTOMRIGHT = 17,

            /// <summary>
            ///     In the border of a window that does not have a sizing border.
            /// </summary>
            HTBORDER = 18,

            /// <summary>
            ///     In a Close button.
            /// </summary>
            HTCLOSE = 20,

            /// <summary>
            ///     In a Help button.
            /// </summary>
            HTHELP = 21
        }

        #endregion

        #region Private AppBar Docking Code

        private struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private struct AppBarData
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public Rect rc;
            public IntPtr lParam;
        }

        private enum AppBarMessage
        {
            New = 0,
            Remove = 1,
            QueryPos = 2,
            SetPos = 3,
            GetState = 4,
            GetTaskBarPos = 5,
            Activate = 6,
            GetAutoHideBar = 7,
            SetAutoHideBar = 8,
            WindowPosChanged = 9,
            SetState = 10
        }

        [DllImport("shell32.dll")]
        private static extern uint SHAppBarMessage(int dwMessage, [In] ref AppBarData pData);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int smIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private bool appBarIsRegistered;
        private Edge currentEdge = Edge.Right;
        private int previousHeightOrWidth;

        private void RegisterBar(Edge edge)
        {
            var abd = new AppBarData();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = Handle;

            abd.uCallbackMessage = RegisterWindowMessage("AppBarMessage");

            var ret = SHAppBarMessage((int) AppBarMessage.New, ref abd);
            appBarIsRegistered = true;
            currentEdge = edge;

            abd.uEdge = (int) edge;

            var currentScreen = Screen.FromHandle(Handle);

            if (abd.uEdge == (int) Edge.Left || abd.uEdge == (int) Edge.Right)
            {
                abd.rc.top = currentScreen.WorkingArea.Top;
                abd.rc.bottom = currentScreen.WorkingArea.Bottom;
                if (abd.uEdge == (int) Edge.Left)
                {
                    abd.rc.left = currentScreen.WorkingArea.Left;
                    abd.rc.right = abd.rc.left + Size.Width;
                }
                else
                {
                    abd.rc.right = currentScreen.WorkingArea.Right;
                    abd.rc.left = abd.rc.right - Size.Width;
                }
            }
            else
            {
                abd.rc.left = 0;
                abd.rc.right = currentScreen.WorkingArea.Width;
                if (abd.uEdge == (int) Edge.Top)
                {
                    abd.rc.top = 0;
                    abd.rc.bottom = Size.Height;
                }
                else
                {
                    abd.rc.bottom = currentScreen.WorkingArea.Height;
                    abd.rc.top = abd.rc.bottom - Size.Height;
                }
            }

            // Query the system for an approved size and position. 
            SHAppBarMessage((int) AppBarMessage.QueryPos, ref abd);

            // Pass the final bounding rectangle to the system. 
            SHAppBarMessage((int) AppBarMessage.SetPos, ref abd);

            // Move and size the appbar so that it conforms to the 
            // bounding rectangle passed to the system. 
            MoveWindow(abd.hWnd, abd.rc.left, abd.rc.top, abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top, true);
        }

        private void UnregisterBar()
        {
            if (appBarIsRegistered)
            {
                var abd = new AppBarData();
                abd.cbSize = Marshal.SizeOf(abd);
                abd.hWnd = Handle;
                SHAppBarMessage((int) AppBarMessage.Remove, ref abd);
                appBarIsRegistered = false;
            }
        }

        #endregion
    }
}
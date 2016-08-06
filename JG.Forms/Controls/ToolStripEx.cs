using System;
using System.Windows.Forms;

namespace JG.Forms.Controls
{
    // This class adds on to the functionality provided in System.Windows.Forms.ToolStrip to honor item clicks when the containing form does not have input focus.
    // http://blogs.msdn.com/rickbrew/archive/2006/01/09/511003.aspx
    public class ToolStripEx : ToolStrip
    {
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_ACTIVATE = 1;
        private const int MA_ACTIVATEANDEAT = 2;
        private const int MA_NOACTIVATE = 3;
        private const int MA_NOACTIVATEANDEAT = 4;

        public ToolStripEx()
        {
        }

        public ToolStripEx(params ToolStripItem[] items)
            : base(items)
        {
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_MOUSEACTIVATE && m.Result == (IntPtr) MA_ACTIVATEANDEAT)
            {
                m.Result = (IntPtr) MA_ACTIVATE;
            }
        }
    }
}
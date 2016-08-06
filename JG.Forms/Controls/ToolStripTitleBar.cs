using System;
using System.Drawing;
using System.Windows.Forms;

namespace JG.Forms.Controls
{
    // This class adds on to the functionality provided in ToolStripEx to pass click events to the parent so that dragging and double-clicking are handled as if the toolstrip were a proper window title bar.
    // Since it uses this.Parent, the ToolStripTitleBar must be added directly to a Form, otherwise it will not behave as intended.
    public class ToolStripTitleBar : ToolStripEx
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_LBUTTONDBLCLK = 0x0203;

        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int WM_NCLBUTTONUP = 0x00A2;
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;

        private const int HTCAPTION = 2;

        public ToolStripTitleBar()
        {
        }

        public ToolStripTitleBar(params ToolStripItem[] items)
            : base(items)
        {
        }

        private bool PointIsInAToolStripLabel(Point pt)
        {
            foreach (ToolStripItem item in Items)
            {
                if (item is ToolStripLabel)
                {
                    if (item.Bounds.Contains(pt))
                        return true;
                }
            }

            return false;
        }

        protected override void WndProc(ref Message m)
        {
            // Typically we want to go ahead and call base.WndProc(ref m) first, but if a ToolStripLabel was clicked then we want to treat that
            // as if it was a click directly on the ToolStrip itself, because otherwise sending the message first to the label interferes.

            Point pos;

            switch (m.Msg)
            {
                case WM_LBUTTONDOWN:
                    // Check where the click took place, and if it wasn't within a label then process the message like normal first
                    pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                    if (!PointIsInAToolStripLabel(pos))
                        base.WndProc(ref m);

                    if (Parent != null)
                    {
                        // Send the message on to the parent form as if it happened in the title bar
                        var newMsg = Message.Create(Parent.Handle, WM_NCLBUTTONDOWN, (IntPtr) HTCAPTION, IntPtr.Zero);
                        base.WndProc(ref newMsg);
                    }
                    break;

                case WM_LBUTTONUP:
                    base.WndProc(ref m);

                    if (Parent != null)
                    {
                        // Send the message on to the parent form as if it happened in the title bar
                        var newMsg = Message.Create(Parent.Handle, WM_NCLBUTTONUP, (IntPtr) HTCAPTION, IntPtr.Zero);
                        base.WndProc(ref newMsg);
                    }
                    break;

                case WM_LBUTTONDBLCLK:
                    // Check where the click took place, and if it wasn't within a label then process the message like normal first
                    pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                    if (!PointIsInAToolStripLabel(pos))
                        base.WndProc(ref m);

                    if (Parent != null)
                    {
                        // Send the message on to the parent form as if it happened in the title bar
                        var newMsg = Message.Create(Parent.Handle, WM_NCLBUTTONDBLCLK, (IntPtr) HTCAPTION, IntPtr.Zero);
                        base.WndProc(ref newMsg);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
using System;

namespace JG.Forms.Demo.AppBar
{
    public partial class AppBarForm : Forms.AppBar
    {
        public AppBarForm()
        {
            InitializeComponent();
        }

        protected override bool UseCustomFormBorderStyle
        {
            get { return false; }
        }

        protected override bool Resizable
        {
            get { return true; }
        }

        protected override int CustomTitleBarSize
        {
            get { return 32; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DockWindow(Edge.Left);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DockWindow(Edge.Right);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DockWindow(Edge.Top);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DockWindow(Edge.Bottom);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UndockWindow();
        }
    }
}
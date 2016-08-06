namespace JG.Forms
{
    partial class AppBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AppBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.MinimumSize = new System.Drawing.Size(16, 38);
            this.Name = "AppBar";
            this.Text = "AppBar";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppBar_FormClosing);
            this.Load += new System.EventHandler(this.AppBar_Load);
            this.ResizeEnd += new System.EventHandler(this.AppBar_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
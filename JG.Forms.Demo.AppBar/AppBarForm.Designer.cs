namespace JG.Forms.Demo.AppBar
{
    partial class AppBarForm
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
            this.btnDockLeft = new System.Windows.Forms.Button();
            this.btnDockRight = new System.Windows.Forms.Button();
            this.btnUndock = new System.Windows.Forms.Button();
            this.btnDockBottom = new System.Windows.Forms.Button();
            this.btnDockTop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDockLeft
            // 
            this.btnDockLeft.Location = new System.Drawing.Point(11, 81);
            this.btnDockLeft.Name = "btnDockLeft";
            this.btnDockLeft.Size = new System.Drawing.Size(85, 23);
            this.btnDockLeft.TabIndex = 1;
            this.btnDockLeft.Text = "Dock Left";
            this.btnDockLeft.UseVisualStyleBackColor = true;
            this.btnDockLeft.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDockRight
            // 
            this.btnDockRight.Location = new System.Drawing.Point(209, 81);
            this.btnDockRight.Name = "btnDockRight";
            this.btnDockRight.Size = new System.Drawing.Size(85, 23);
            this.btnDockRight.TabIndex = 3;
            this.btnDockRight.Text = "Dock Right";
            this.btnDockRight.UseVisualStyleBackColor = true;
            this.btnDockRight.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnUndock
            // 
            this.btnUndock.Location = new System.Drawing.Point(110, 81);
            this.btnUndock.Name = "btnUndock";
            this.btnUndock.Size = new System.Drawing.Size(85, 23);
            this.btnUndock.TabIndex = 2;
            this.btnUndock.Text = "Undock";
            this.btnUndock.UseVisualStyleBackColor = true;
            this.btnUndock.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnDockBottom
            // 
            this.btnDockBottom.Location = new System.Drawing.Point(110, 119);
            this.btnDockBottom.Name = "btnDockBottom";
            this.btnDockBottom.Size = new System.Drawing.Size(85, 23);
            this.btnDockBottom.TabIndex = 4;
            this.btnDockBottom.Text = "Dock Bottom";
            this.btnDockBottom.UseVisualStyleBackColor = true;
            this.btnDockBottom.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnDockTop
            // 
            this.btnDockTop.Location = new System.Drawing.Point(110, 43);
            this.btnDockTop.Name = "btnDockTop";
            this.btnDockTop.Size = new System.Drawing.Size(85, 23);
            this.btnDockTop.TabIndex = 0;
            this.btnDockTop.Text = "Dock Top";
            this.btnDockTop.UseVisualStyleBackColor = true;
            this.btnDockTop.Click += new System.EventHandler(this.button5_Click);
            // 
            // AppBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 184);
            this.Controls.Add(this.btnDockTop);
            this.Controls.Add(this.btnDockBottom);
            this.Controls.Add(this.btnUndock);
            this.Controls.Add(this.btnDockRight);
            this.Controls.Add(this.btnDockLeft);
            this.MinimumSize = new System.Drawing.Size(130, 38);
            this.Name = "AppBarForm";
            this.Text = "AppBar Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDockLeft;
        private System.Windows.Forms.Button btnDockRight;
        private System.Windows.Forms.Button btnUndock;
        private System.Windows.Forms.Button btnDockBottom;
        private System.Windows.Forms.Button btnDockTop;
    }
}


namespace test1
{
    partial class PluginForm
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
            this.cmd1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmd1
            // 
            this.cmd1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cmd1.Location = new System.Drawing.Point(87, 71);
            this.cmd1.Name = "cmd1";
            this.cmd1.Size = new System.Drawing.Size(203, 40);
            this.cmd1.TabIndex = 0;
            this.cmd1.Text = "Построить";
            this.cmd1.UseVisualStyleBackColor = true;
            this.cmd1.Click += new System.EventHandler(this.cmd1_Click);
            // 
            // PluginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 197);
            this.Controls.Add(this.cmd1);
            this.Name = "PluginForm";
            this.Text = "PluginForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PluginForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmd1;
    }
}
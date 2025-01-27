namespace UpdateApp
{
    partial class UpdateForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnStartUpdate = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(12, 12);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(360, 100);
            this.txtStatus.TabIndex = 0;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 118);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(360, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // btnStartUpdate
            // 
            this.btnStartUpdate.Location = new System.Drawing.Point(12, 147);
            this.btnStartUpdate.Name = "btnStartUpdate";
            this.btnStartUpdate.Size = new System.Drawing.Size(360, 23);
            this.btnStartUpdate.TabIndex = 2;
            this.btnStartUpdate.Text = "Güncellemeyi Başlat";
            this.btnStartUpdate.UseVisualStyleBackColor = true;
            this.btnStartUpdate.Click += new System.EventHandler(this.btnStartUpdate_Click);
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 181);
            this.Controls.Add(this.btnStartUpdate);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UpdateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Logo Güncelleme";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnStartUpdate;
    }
}
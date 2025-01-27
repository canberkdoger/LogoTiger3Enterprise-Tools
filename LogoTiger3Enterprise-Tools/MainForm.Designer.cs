namespace UpdateApp
{
    partial class MainForm
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
            this.btnVersionUpdate = new System.Windows.Forms.Button();
            this.btnLicenseUpdate = new System.Windows.Forms.Button();
            this.btnLogoNotStarting = new System.Windows.Forms.Button();
            this.btnConnectNotStarting = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();

            // Grup kutusu ekleyelim
            this.groupProblems = new System.Windows.Forms.GroupBox();
            this.groupSettings = new System.Windows.Forms.GroupBox();

            this.SuspendLayout();
            this.groupProblems.SuspendLayout();
            this.groupSettings.SuspendLayout();

            // Ana İşlemler Grup Kutusu
            this.groupProblems.Text = "Logo İşlemleri";
            this.groupProblems.Location = new System.Drawing.Point(12, 12);
            this.groupProblems.Size = new System.Drawing.Size(380, 190);
            this.groupProblems.Name = "groupProblems";

            // Ayarlar Grup Kutusu
            this.groupSettings.Text = "Ayarlar";
            this.groupSettings.Location = new System.Drawing.Point(12, 208);
            this.groupSettings.Size = new System.Drawing.Size(380, 70);
            this.groupSettings.Name = "groupSettings";

            // Logo Sürüm Güncelleme Butonu
            this.btnVersionUpdate.Location = new System.Drawing.Point(10, 20);
            this.btnVersionUpdate.Name = "btnVersionUpdate";
            this.btnVersionUpdate.Size = new System.Drawing.Size(360, 40);
            this.btnVersionUpdate.TabIndex = 0;
            this.btnVersionUpdate.Text = "Logo Sürüm Güncelleme";
            this.btnVersionUpdate.UseVisualStyleBackColor = true;
            this.btnVersionUpdate.Click += new System.EventHandler(this.btnVersionUpdate_Click);

            // Logo Lisans Yenileme Butonu
            this.btnLicenseUpdate.Location = new System.Drawing.Point(10, 65);
            this.btnLicenseUpdate.Name = "btnLicenseUpdate";
            this.btnLicenseUpdate.Size = new System.Drawing.Size(360, 40);
            this.btnLicenseUpdate.TabIndex = 1;
            this.btnLicenseUpdate.Text = "Logo Lisans Yenileme";
            this.btnLicenseUpdate.UseVisualStyleBackColor = true;
            this.btnLicenseUpdate.Click += new System.EventHandler(this.btnLicenseUpdate_Click);

            // Logo Açılmıyor Butonu
            this.btnLogoNotStarting.Location = new System.Drawing.Point(10, 110);
            this.btnLogoNotStarting.Name = "btnLogoNotStarting";
            this.btnLogoNotStarting.Size = new System.Drawing.Size(360, 35);
            this.btnLogoNotStarting.TabIndex = 2;
            this.btnLogoNotStarting.Text = "Logo Açılmıyor";
            this.btnLogoNotStarting.UseVisualStyleBackColor = true;
            this.btnLogoNotStarting.Click += new System.EventHandler(this.btnLogoNotStarting_Click);

            // Connect Açılmıyor Butonu
            this.btnConnectNotStarting.Location = new System.Drawing.Point(10, 150);
            this.btnConnectNotStarting.Name = "btnConnectNotStarting";
            this.btnConnectNotStarting.Size = new System.Drawing.Size(360, 35);
            this.btnConnectNotStarting.TabIndex = 3;
            this.btnConnectNotStarting.Text = "Connect Açılmıyor";
            this.btnConnectNotStarting.UseVisualStyleBackColor = true;
            this.btnConnectNotStarting.Click += new System.EventHandler(this.btnConnectNotStarting_Click);

            // Ayarlar Butonu
            this.btnSettings.Location = new System.Drawing.Point(10, 20);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(360, 35);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "Klasör Ayarları";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);

            // Grup kutularına butonları ekle
            this.groupProblems.Controls.Add(this.btnVersionUpdate);
            this.groupProblems.Controls.Add(this.btnLicenseUpdate);
            this.groupProblems.Controls.Add(this.btnLogoNotStarting);
            this.groupProblems.Controls.Add(this.btnConnectNotStarting);

            this.groupSettings.Controls.Add(this.btnSettings);

            // Ana Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 291);
            this.Controls.Add(this.groupProblems);
            this.Controls.Add(this.groupSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Logo Yardımcı Araçlar";

            this.groupProblems.ResumeLayout(false);
            this.groupSettings.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnVersionUpdate;
        private System.Windows.Forms.Button btnLicenseUpdate;
        private System.Windows.Forms.Button btnLogoNotStarting;
        private System.Windows.Forms.Button btnConnectNotStarting;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.GroupBox groupProblems;
        private System.Windows.Forms.GroupBox groupSettings;
    }
}
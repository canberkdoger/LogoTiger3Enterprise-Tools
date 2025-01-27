using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ConfigManager.LoadConfig();
        }

        #region Button Click Events

        private void btnVersionUpdate_Click(object sender, EventArgs e)
        {
            using (var updateForm = new UpdateForm())
            {
                updateForm.ShowDialog();
            }
        }

        private async void btnLicenseUpdate_Click(object sender, EventArgs e)
        {
            using (var form = new LicenseUpdateForm())
            {
                try
                {
                    if (!Logger.InitializeLog("license"))
                    {
                        MessageBox.Show(
                            "Ýþlem iptal edildi.",
                            "Ýþlem Ýptali",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }

                    form.Show();
                    await ProcessLicenseUpdate(form);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Lisans güncelleme hatasý", ex);
                    MessageBox.Show(
                        $"Hata oluþtu: {ex.Message}",
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                finally
                {
                    form.Close();
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ConfigManager.OpenConfigFile();

            DialogResult result = MessageBox.Show(
                "Yapýlandýrma deðiþikliklerinin etkili olmasý için uygulamayý yeniden baþlatmanýz gerekiyor.\n\n" +
                "Uygulamayý þimdi yeniden baþlatmak ister misiniz?",
                "Yapýlandýrma Deðiþikliði",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        private void btnLogoNotStarting_Click(object sender, EventArgs e)
        {
            try
            {
                KillSpecificProcess("LENGINE3");
                MessageBox.Show(
                    "Logo Engine kapatýldý. Þimdi Logo'yu çalýþtýrabilirsiniz.",
                    "Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Hata: {ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnConnectNotStarting_Click(object sender, EventArgs e)
        {
            try
            {
                KillSpecificProcess("LogoConnect");
                MessageBox.Show(
                    "Logo Connect kapatýldý. Þimdi Connect'i çalýþtýrabilirsiniz.",
                    "Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Hata: {ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion

        #region Helper Methods

        private async Task ProcessLicenseUpdate(LicenseUpdateForm form)
        {
            form.UpdateProgress(0);
            form.UpdateStatus("Lisans güncelleme iþlemi baþlatýlýyor...");
            await Task.Delay(500);

            form.UpdateStatus("Açýk uygulamalar kontrol ediliyor...");
            form.UpdateProgress(20);
            KillProcesses();
            form.UpdateStatus("Açýk uygulamalar kapatýldý.");
            await Task.Delay(500);

            form.UpdateStatus("Lisans dosyalarý kopyalanýyor...");
            form.UpdateProgress(40);
            await Task.Run(() => CopyLicenseFiles(form));
            form.UpdateStatus("Lisans dosyalarý baþarýyla kopyalandý.");
            form.UpdateProgress(100);
            await Task.Delay(1000);

            MessageBox.Show(
                "Lisans dosyalarý baþarýyla güncellendi!",
                "Bilgi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void KillProcesses()
        {
            foreach (var processName in new[] { "LENGINE3", "LogoConnect" })
            {
                KillSpecificProcess(processName);
            }
        }

        private void KillSpecificProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                    Logger.LogMessage($"{processName} kapatýldý");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{processName} kapatýlamadý", ex);
                }
            }
        }

        private void CopyLicenseFiles(LicenseUpdateForm form)
        {
            string sourceDir = ConfigManager.SourceDir;
            string targetDir = ConfigManager.TargetDir;

            string[] licenseFiles = { "License.Dat", "LicenseExt.dat" };
            int totalFiles = licenseFiles.Length;
            int copiedFiles = 0;

            foreach (string file in licenseFiles)
            {
                try
                {
                    string sourcePath = Path.Combine(sourceDir, file);
                    string targetPath = Path.Combine(targetDir, file);

                    form.UpdateStatus($"Kopyalanýyor: {file}");
                    File.Copy(sourcePath, targetPath, true);

                    copiedFiles++;
                    int percentage = (copiedFiles * 100) / totalFiles;
                    form.UpdateProgress(40 + (percentage * 60 / 100));

                    Logger.LogMessage($"Lisans dosyasý kopyalandý: {file}");
                    form.UpdateStatus($"Kopyalandý: {file}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Lisans dosyasý kopyalanamadý: {file}", ex);
                    throw new Exception($"Lisans dosyasý kopyalanýrken hata oluþtu: {file}", ex);
                }
            }
        }

        #endregion
    }
}
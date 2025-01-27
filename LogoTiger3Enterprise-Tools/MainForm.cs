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
                            "��lem iptal edildi.",
                            "��lem �ptali",
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
                    Logger.LogError("Lisans g�ncelleme hatas�", ex);
                    MessageBox.Show(
                        $"Hata olu�tu: {ex.Message}",
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
                "Yap�land�rma de�i�ikliklerinin etkili olmas� i�in uygulamay� yeniden ba�latman�z gerekiyor.\n\n" +
                "Uygulamay� �imdi yeniden ba�latmak ister misiniz?",
                "Yap�land�rma De�i�ikli�i",
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
                    "Logo Engine kapat�ld�. �imdi Logo'yu �al��t�rabilirsiniz.",
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
                    "Logo Connect kapat�ld�. �imdi Connect'i �al��t�rabilirsiniz.",
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
            form.UpdateStatus("Lisans g�ncelleme i�lemi ba�lat�l�yor...");
            await Task.Delay(500);

            form.UpdateStatus("A��k uygulamalar kontrol ediliyor...");
            form.UpdateProgress(20);
            KillProcesses();
            form.UpdateStatus("A��k uygulamalar kapat�ld�.");
            await Task.Delay(500);

            form.UpdateStatus("Lisans dosyalar� kopyalan�yor...");
            form.UpdateProgress(40);
            await Task.Run(() => CopyLicenseFiles(form));
            form.UpdateStatus("Lisans dosyalar� ba�ar�yla kopyaland�.");
            form.UpdateProgress(100);
            await Task.Delay(1000);

            MessageBox.Show(
                "Lisans dosyalar� ba�ar�yla g�ncellendi!",
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
                    Logger.LogMessage($"{processName} kapat�ld�");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{processName} kapat�lamad�", ex);
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

                    form.UpdateStatus($"Kopyalan�yor: {file}");
                    File.Copy(sourcePath, targetPath, true);

                    copiedFiles++;
                    int percentage = (copiedFiles * 100) / totalFiles;
                    form.UpdateProgress(40 + (percentage * 60 / 100));

                    Logger.LogMessage($"Lisans dosyas� kopyaland�: {file}");
                    form.UpdateStatus($"Kopyaland�: {file}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Lisans dosyas� kopyalanamad�: {file}", ex);
                    throw new Exception($"Lisans dosyas� kopyalan�rken hata olu�tu: {file}", ex);
                }
            }
        }

        #endregion
    }
}
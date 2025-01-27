using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UpdateApp
{

    public class NetworkRetry
    {
        public static bool IsNetworkPathAccessible(string networkPath)
        {
            try
            {
                return Directory.Exists(networkPath);
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> WaitForNetworkConnection(string networkPath, int timeoutSeconds = 30)
        {
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
            {
                if (IsNetworkPathAccessible(networkPath))
                    return true;

                await Task.Delay(1000); // 1 saniye bekle
            }
            return false;
        }
    }
    public partial class UpdateForm : Form
    {
        private readonly string sourceDir;
        private readonly string targetDir;

        public UpdateForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            // Config'den değerleri al
            sourceDir = ConfigManager.SourceDir;
            targetDir = ConfigManager.TargetDir;

            // İlk başta ağ bağlantısını kontrol et
            if (!NetworkRetry.IsNetworkPathAccessible(sourceDir))
            {
                MessageBox.Show(
                    "Ağ klasörüne erişilemiyor. Lütfen ağ bağlantınızı kontrol edin ve tekrar deneyin.",
                    "Bağlantı Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }


        private async void btnStartUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Log başlatma kontrolü
                if (!Logger.InitializeLog("update"))
                {
                    MessageBox.Show(
                        "İşlem iptal edildi.",
                        "İşlem İptali",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                btnStartUpdate.Enabled = false;
                progressBar1.Value = 0;

                // Ağ bağlantısını kontrol et
                if (!NetworkRetry.IsNetworkPathAccessible(sourceDir))
                {
                    UpdateStatus("Ağ bağlantısı kontrol ediliyor...");
                    bool networkAvailable = await NetworkRetry.WaitForNetworkConnection(sourceDir);
                    if (!networkAvailable)
                    {
                        throw new Exception("Ağ klasörüne erişilemiyor. Lütfen ağ bağlantınızı kontrol edin.");
                    }
                }

                // Uygulamaları kapat
                UpdateStatus("Açık uygulamalar kapatılıyor...");
                KillProcesses();
                Logger.LogMessage("Uygulamalar kapatıldı");
                UpdateStatus("Açık uygulamalar kapatıldı.");

                // Hedef klasörü temizle
                UpdateStatus("Hedef klasör temizleniyor...");
                await ClearTargetDirectoryAsync();
                UpdateStatus("Hedef klasör temizlendi.");

                // Dosyaları kopyala
                UpdateStatus("Dosyalar kopyalanıyor...");
                await CopyFilesAsync();

                // İşlem başarılı
                progressBar1.Value = 100;
                UpdateStatus("Güncelleme işlemi başarıyla tamamlandı!");
                Logger.LogMessage("Güncelleme işlemi başarıyla tamamlandı");

                MessageBox.Show(
                    "Güncelleme işlemi başarıyla tamamlandı!",
                    "Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                string errorMessage = "Güncelleme işlemi sırasında bir hata oluştu. " +
                                    "Lütfen tüm Logo programlarını kapatıp tekrar deneyin.\n\n" +
                                    "Hata detayı: " + ex.Message;

                MessageBox.Show(errorMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("Güncelleme işlemi sırasında hata oluştu", ex);
            }
            finally
            {
                btnStartUpdate.Enabled = true;
            }
        }

        private void UpdateStatus(string message)
        {
            txtStatus.AppendText(message + Environment.NewLine);
            txtStatus.ScrollToCaret();
        }

        private void KillProcesses()
        {
            string[] processNames = { "LENGINE3", "LogoConnect" };
            foreach (string processName in processNames)
            {
                foreach (var process in Process.GetProcessesByName(processName))
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch { }
                }
            }
        }

        private async Task ClearTargetDirectoryAsync()
        {
            await Task.Run(() =>
            {
                if (Directory.Exists(targetDir))
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(targetDir, "*.*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                File.Delete(file);
                                Logger.LogMessage($"Dosya silindi: {file}");
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError($"Dosya silinemedi: {file}", ex);
                            }
                        }

                        foreach (string dir in Directory.GetDirectories(targetDir))
                        {
                            try
                            {
                                Directory.Delete(dir, true);
                                Logger.LogMessage($"Klasör silindi: {dir}");
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError($"Klasör silinemedi: {dir}", ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Klasör temizleme işlemi sırasında hata", ex);
                    }
                }

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                    Logger.LogMessage("Hedef klasör oluşturuldu");
                }
            });
        }

        private async Task CopyFilesAsync()
        {
            const int MAX_RETRIES = 3;
            const int RETRY_DELAY_SECONDS = 5;
            List<string> failedFiles = new List<string>();
            int totalRetries = 0;

            await Task.Run(async () =>
            {
                while (totalRetries < MAX_RETRIES)
                {
                    try
                    {
                        if (!NetworkRetry.IsNetworkPathAccessible(sourceDir))
                        {
                            UpdateStatus("Ağ bağlantısı kontrol ediliyor...");
                            bool networkAvailable = await NetworkRetry.WaitForNetworkConnection(sourceDir);
                            if (!networkAvailable)
                            {
                                throw new Exception("Ağ klasörüne erişilemiyor. Lütfen ağ bağlantınızı kontrol edin.");
                            }
                        }

                        string[] files;
                        try
                        {
                            files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Kaynak klasör okunamıyor: {ex.Message}");
                        }

                        int totalFiles = files.Length;
                        int copiedFiles = 0;

                        foreach (string sourceFile in files)
                        {
                            if (failedFiles.Contains(sourceFile)) // Önceden başarısız olanları tekrar dene
                            {
                                try
                                {
                                    string relativePath = sourceFile.Substring(sourceDir.Length);
                                    string targetFile = Path.Combine(targetDir, relativePath.TrimStart('\\'));
                                    string targetFileDir = Path.GetDirectoryName(targetFile);

                                    if (!string.IsNullOrEmpty(targetFileDir) && !Directory.Exists(targetFileDir))
                                    {
                                        Directory.CreateDirectory(targetFileDir);
                                    }

                                    File.Copy(sourceFile, targetFile, true);
                                    failedFiles.Remove(sourceFile); // Başarılı kopyalamadan sonra listeden çıkar
                                    Logger.LogMessage($"Başarıyla tekrar kopyalandı: {relativePath}");
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError($"Dosya tekrar kopyalanamadı: {sourceFile}", ex);
                                    continue;
                                }
                            }
                            else // İlk kez denenen dosyalar
                            {
                                try
                                {
                                    string relativePath = sourceFile.Substring(sourceDir.Length);
                                    string targetFile = Path.Combine(targetDir, relativePath.TrimStart('\\'));
                                    string targetFileDir = Path.GetDirectoryName(targetFile);

                                    if (!string.IsNullOrEmpty(targetFileDir) && !Directory.Exists(targetFileDir))
                                    {
                                        Directory.CreateDirectory(targetFileDir);
                                    }

                                    File.Copy(sourceFile, targetFile, true);
                                    copiedFiles++;
                                    Logger.LogMessage($"Dosya kopyalandı: {relativePath}");

                                    int percentage = (int)((double)copiedFiles / totalFiles * 100);
                                    progressBar1.Value = percentage;
                                    UpdateStatus($"Dosyalar kopyalanıyor... (%{percentage})");
                                }
                                catch (Exception ex)
                                {
                                    failedFiles.Add(sourceFile);
                                    Logger.LogError($"Dosya kopyalanamadı: {sourceFile}", ex);
                                    UpdateStatus($"Uyarı: Bazı dosyalar kopyalanamadı. Yeniden deneniyor...");
                                }
                            }
                        }

                        if (failedFiles.Count == 0)
                        {
                            return; // Tüm dosyalar başarıyla kopyalandı
                        }
                        else
                        {
                            totalRetries++;
                            if (totalRetries < MAX_RETRIES)
                            {
                                UpdateStatus($"Bazı dosyalar kopyalanamadı. {RETRY_DELAY_SECONDS} saniye sonra tekrar deneniyor... (Deneme {totalRetries}/{MAX_RETRIES})");
                                await Task.Delay(RETRY_DELAY_SECONDS * 1000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        totalRetries++;
                        if (totalRetries >= MAX_RETRIES)
                        {
                            throw new Exception($"Maksimum deneme sayısına ulaşıldı. Son hata: {ex.Message}");
                        }

                        UpdateStatus($"Bağlantı hatası. {RETRY_DELAY_SECONDS} saniye sonra tekrar deneniyor... (Deneme {totalRetries}/{MAX_RETRIES})");
                        await Task.Delay(RETRY_DELAY_SECONDS * 1000);
                    }
                }

                if (failedFiles.Count > 0)
                {
                    string failedFilesList = string.Join("\n", failedFiles);
                    Logger.LogError($"Bazı dosyalar kopyalanamadı:\n{failedFilesList}", new Exception("Maksimum deneme sayısına ulaşıldı"));
                    throw new Exception($"Bazı dosyalar kopyalanamadı. Detaylar için log dosyasını kontrol edin.");
                }
            });
        }
    }
}
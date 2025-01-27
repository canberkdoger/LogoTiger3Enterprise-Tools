using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace UpdateApp
{
    public static class Logger
    {
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static string? _currentLogFile;
        private static readonly object _lockObject = new object();
        private static bool _canWriteLogs = true; // Log yazma durumunu takip etmek için

        public static bool InitializeLog(string operationType)
        {
            try
            {
                // Önce log klasörüne yazma yetkimiz var mı test edelim
                if (!CheckLogWritePermission())
                {
                    DialogResult result = MessageBox.Show(
                        "Log dosyası oluşturulamıyor. Yazma yetkiniz bulunmuyor.\n\n" +
                        "İşleme log kaydı olmadan devam etmek istiyor musunuz?",
                        "Log Yazma Hatası",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.No)
                    {
                        return false; // Kullanıcı işlemi iptal etmek istiyor
                    }

                    _canWriteLogs = false; // Log yazma devre dışı bırakılıyor
                    return true; // Kullanıcı log olmadan devam etmek istiyor
                }

                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                string computerName = Environment.MachineName;
                string dateStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                _currentLogFile = Path.Combine(LogDirectory, $"{operationType}-{computerName}-{dateStr}.log");

                WriteToLog($"=== İşlem Başlangıç ===");
                WriteToLog($"İşlem Tipi: {operationType}");
                WriteToLog($"Bilgisayar: {computerName}");
                WriteToLog($"Kullanıcı: {Environment.UserName}");
                WriteToLog($"Tarih: {DateTime.Now}");
                WriteToLog("========================\n");

                return true;
            }
            catch (Exception)
            {
                DialogResult result = MessageBox.Show(
                    "Log dosyası oluşturulurken bir hata oluştu.\n\n" +
                    "İşleme log kaydı olmadan devam etmek istiyor musunuz?",
                    "Log Hatası",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                _canWriteLogs = false;
                return result == DialogResult.Yes;
            }
        }

        private static bool CheckLogWritePermission()
        {
            try
            {
                // Log klasörü yoksa oluşturmayı dene
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                // Test dosyası oluşturup yazma yetkisi kontrol et
                string testFile = Path.Combine(LogDirectory, "test.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void WriteToLog(string content)
        {
            if (!_canWriteLogs || string.IsNullOrEmpty(_currentLogFile)) return;

            try
            {
                lock (_lockObject)
                {
                    File.AppendAllText(_currentLogFile, content + Environment.NewLine);
                }
            }
            catch
            {
                // Log yazma hatası durumunda sessizce devam et
            }
        }

        public static void LogMessage(string message)
        {
            if (!_canWriteLogs) return;

            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            WriteToLog(logEntry);
        }

        public static void LogError(string message, Exception ex)
        {
            if (!_canWriteLogs) return;

            string errorDetails = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] HATA: {message}\n" +
                                $"Hata Detayı: {ex.Message}\n" +
                                $"Stack Trace: {ex.StackTrace}\n";
            WriteToLog(errorDetails);
        }
    }
}
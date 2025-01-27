using System;
using System.IO;

namespace UpdateApp
{
    public static class ConfigManager
    {
        private static readonly string ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
        private static string _sourceDir = @"\\sunucu\TIGER3ENT"; // Varsayılan değer
        private static string _targetDir = @"C:\TIGER3ENT"; // Varsayılan değer

        public static string SourceDir => _sourceDir;
        public static string TargetDir => _targetDir;

        public static bool LoadConfig()
        {
            try
            {
                // Config dosyası yoksa oluştur
                if (!File.Exists(ConfigFile))
                {
                    CreateDefaultConfig();
                    MessageBox.Show(
                        $"Yapılandırma dosyası oluşturuldu:\n{ConfigFile}\n\n" +
                        "Kaynak ve hedef klasörleri düzenleyebilirsiniz.",
                        "Bilgi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return true;
                }

                // Config dosyasını oku
                string[] lines = File.ReadAllLines(ConfigFile);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    string[] parts = line.Split('=');
                    if (parts.Length != 2)
                        continue;

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key.ToLower())
                    {
                        case "sourcedir":
                            _sourceDir = value;
                            break;
                        case "targetdir":
                            _targetDir = value;
                            break;
                    }
                }

                // Değerleri kontrol et
                if (string.IsNullOrEmpty(_sourceDir) || string.IsNullOrEmpty(_targetDir))
                {
                    throw new Exception("Kaynak veya hedef klasör yapılandırması eksik!");
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Yapılandırma dosyası okuma hatası:\n{ex.Message}\n\n" +
                    "Varsayılan değerler kullanılacak.",
                    "Yapılandırma Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
        }

        private static void CreateDefaultConfig()
        {
            string config =
                "# Logo Güncelleme Yapılandırma Dosyası\n" +
                "# Kaynak ve hedef klasörleri buradan düzenleyebilirsiniz\n" +
                "# Örnek:\n" +
                "# SourceDir = \\\\sunucu\\klasor\n" +
                "# TargetDir = C:\\klasor\n\n" +
                $"SourceDir = {_sourceDir}\n" +
                $"TargetDir = {_targetDir}";

            File.WriteAllText(ConfigFile, config);
        }

        public static void OpenConfigFile()
        {
            try
            {
                if (!File.Exists(ConfigFile))
                {
                    CreateDefaultConfig();
                }

                System.Diagnostics.Process.Start("notepad.exe", ConfigFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Yapılandırma dosyası açılamadı:\n{ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
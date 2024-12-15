using RPToolkid.myutils;
using System.Diagnostics;
using System.Net;

namespace RPToolkid.src
{
    internal class RPUtils
    {
        private static readonly string FiveMPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM");
        private static readonly string DownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        private static readonly string FiveMExe = Path.Combine(DownloadPath, "FiveM.exe");
        private const string FiveMUrl = "https://runtime.fivem.net/client/FiveM.exe";

        public static bool IsFiveMRunning() => Process.GetProcessesByName("FiveM").Length > 0;

        //UPDATED!
        public static void EnsureFiveMNotRunning()
        {
            while (IsFiveMRunning())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[WARNUNG] FiveM läuft derzeit. Bitte schließen Sie FiveM, um fortzufahren.");
                Console.ResetColor();
                Console.WriteLine("Warten auf das Schließen von FiveM...");
                Thread.Sleep(3000);
            }
            Console.WriteLine("[INFO] FiveM ist geschlossen. Sie können jetzt fortfahren.");
        }

        public static void ClearCache()
        {
            if (!ConfirmAction("Sind Sie sicher, dass Sie den Cache löschen möchten? (y/n)"))
            {
                Console.WriteLine("[INFO] Abgebrochen. Der Cache wurde nicht gelöscht.");
                return;
            }

            if (!Directory.Exists(FiveMPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Der FiveM-Ordner wurde nicht gefunden: {FiveMPath}");
                Console.ResetColor();
                return;
            }

            string[] cachePaths = {
                Path.Combine(FiveMPath, "FiveM.app", "data", "cache"),
                Path.Combine(FiveMPath, "FiveM.app", "data", "server-cache"),
                Path.Combine(FiveMPath, "FiveM.app", "data", "server-cache-priv")
            };

            Console.WriteLine("\nLösche den Cache...");
            foreach (var path in cachePaths)
            {
                TryDeleteAndRecreate(path);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[SUCCESS] Cache wurde erfolgreich gelöscht!");
            Console.ResetColor();
        }

        public static async Task ReinstallFiveM()
        {
            Console.WriteLine("\nCopyright by RiCI");
            Console.WriteLine("Wollen Sie FiveM wirklich neu installieren?");
            Console.WriteLine("Dadurch gehen Ihre FiveM-Daten verloren. Möchten Sie fortfahren? (J/N)");

            if (!ConfirmAction(null))
            {
                Console.WriteLine("\n[INFO] Installation abgebrochen.");
                return;
            }

            await DeleteOldFiveM();
            await DownloadFiveM();
            StartFiveMInstaller();
        }

        private static async Task DeleteOldFiveM()
        {
            if (!Directory.Exists(FiveMPath))
            {
                Console.WriteLine("\n[INFO] Der FiveM-Ordner wurde nicht gefunden.");
                return;
            }

            Console.WriteLine("\n[INFO] Lösche den alten FiveM-Ordner...");
            try
            {
                Directory.Delete(FiveMPath, true);
                Console.WriteLine("[SUCCESS] Der FiveM-Ordner wurde erfolgreich gelöscht.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Fehler beim Löschen des FiveM-Ordners: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static async Task DownloadFiveM()
        {
            if (File.Exists(FiveMExe))
            {
                Console.WriteLine("\n[INFO] FiveM.exe wurde bereits heruntergeladen.");
                return;
            }

            Console.WriteLine("\n[INFO] Lade FiveM.exe herunter...");
            using (WebClient client = new WebClient())
            {
                try
                {
                    await client.DownloadFileTaskAsync(new Uri(FiveMUrl), FiveMExe);
                    Console.WriteLine("[SUCCESS] Download abgeschlossen.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR] Fehler beim Herunterladen von FiveM.exe: {ex.Message}");
                    Console.ResetColor();
                    Environment.Exit(1);
                }
            }
        }

        private static void StartFiveMInstaller()
        {
            if (!File.Exists(FiveMExe))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] FiveM.exe konnte nicht gefunden werden.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("\n[INFO] Starte die FiveM-Installation...");
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = FiveMExe,
                    UseShellExecute = true
                });
                Console.WriteLine("[SUCCESS] FiveM-Installation gestartet.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Fehler beim Starten der FiveM-Installation: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static bool ConfirmAction(string message)
        {
            if (message != null) Console.WriteLine(message);
            string input = Console.ReadLine()?.Trim().ToLower();
            return input == "y";
        }

        private static void TryDeleteAndRecreate(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Console.WriteLine($"[INFO] Gelöscht: {path}");
                }

                Directory.CreateDirectory(path);
                Console.WriteLine($"[INFO] Neu erstellt: {path}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Fehler bei {path}: {ex.Message}");
                Console.ResetColor();
            }
        }

        //UPDATED!
        public static void UpdateSettings(string key, string value)
        {
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "CitizenFX.ini");

            if (!File.Exists(configPath))
            {
                Mycstools.LogMessage("[ERROR] CitizenFX.ini konnte nicht gefunden werden.", ConsoleColor.Red);
                return;
            }

            string tempPath = configPath + ".tmp";

            try
            {
                string[] lines = File.ReadAllLines(configPath);
                bool keyFound = false;

                using (StreamWriter sw = new StreamWriter(tempPath))
                {
                    foreach (string line in lines)
                    {
                        if (line.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                        {
                            sw.WriteLine($"{key}={value}");
                            keyFound = true;
                        }
                        else
                        {
                            sw.WriteLine(line);
                        }
                    }

                    if (!keyFound)
                    {
                        sw.WriteLine($"{key}={value}");
                    }
                }

                File.Replace(tempPath, configPath, null);
                Mycstools.LogMessage($"[SUCCESS] {key} wurde auf {value} gesetzt.", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                Mycstools.LogMessage($"[ERROR] Fehler beim Aktualisieren der Einstellungen: {ex.Message}", ConsoleColor.Red);
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }

        public static void DisplaySettings()
        {
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "CitizenFX.ini");

            if (!File.Exists(configPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] CitizenFX.ini konnte nicht gefunden werden.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("\n[INFO] Aktuelle Einstellungen in CitizenFX.ini:");
            string[] lines = File.ReadAllLines(configPath);
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

        //Updatet!
        public static void StartFiveM()
        {
            string fiveMPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.exe");

            if (!File.Exists(fiveMPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] FiveM.exe konnte nicht gefunden werden.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("\n[INFO] Überprüfe, ob FiveM bereits läuft...");

            if (Process.GetProcessesByName("FiveM").Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[INFO] FiveM läuft bereits!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("[INFO] Starte FiveM...");

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = fiveMPath,
                    UseShellExecute = true
                });
                Console.WriteLine("[SUCCESS] FiveM wurde gestartet.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Fehler beim Starten von FiveM: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
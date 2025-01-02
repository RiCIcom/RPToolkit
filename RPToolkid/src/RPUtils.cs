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

        //UPDATED (Timeout for bether Performance)!
        public static void EnsureFiveMNotRunning(int timeoutInSeconds = 300)
        {
            var startTime = DateTime.Now;
            while (IsFiveMRunning())
            {
                if ((DateTime.Now - startTime).TotalSeconds > timeoutInSeconds)
                {
                    Logger.LogError("Timeout: FiveM wurde nicht rechtzeitig geschlossen.");
                    return;
                }
                Logger.LogInfo("FiveM läuft noch. Bitte schließen.");
                Thread.Sleep(3000);
            }
            Logger.LogInfo("FiveM ist geschlossen.");
        }

        public static void ClearCache()
        {
            if (!ConfirmAction("Sind Sie sicher, dass Sie den Cache löschen möchten? (Y/N)"))
            {
                Logger.LogInfo("Abgebrochen. Der Cache wurde nicht gelöscht.");
                return;
            }

            if (!Directory.Exists(FiveMPath))
            {
                Logger.LogError($"[ERROR] Der FiveM-Ordner wurde nicht gefunden: {FiveMPath}");
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

            Logger.LogSuccess("Cache wurde erfolgreich gelöscht!");
        }

        public static async Task ReinstallFiveM()
        {
            Console.Clear();
            UI.DisplayHeader();
            Console.WriteLine("\nCopyright by RiCI");
            Console.WriteLine("Wollen Sie FiveM wirklich neu installieren?");
            Console.WriteLine("Dadurch gehen Ihre FiveM-Daten verloren. Möchten Sie fortfahren? (J/N)");

            if (!ConfirmAction(null))
            {
                Logger.LogInfo("Installation abgebrochen.");
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
                Logger.LogError("Der FiveM-Ordner wurde nicht gefunden.");
                return;
            }

            Logger.LogInfo("Lösche den alten FiveM-Ordner...");

            try
            {
                await Task.Run(() =>
                {
                    DeleteDirectoryContents(FiveMPath);
                    Directory.Delete(FiveMPath, true);
                });
                Logger.LogSuccess("[SUCCESS] Der FiveM-Ordner wurde erfolgreich gelöscht.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ERROR] Fehler beim Löschen des FiveM-Ordners: {ex.Message}");
            }
        }

        private static void DeleteDirectoryContents(string directoryPath)
        {
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"[ERROR] Fehler beim Löschen der Datei {file}: {ex.Message}");
                }
            }

            foreach (var dir in Directory.GetDirectories(directoryPath))
            {
                try
                {
                    DeleteDirectoryContents(dir);
                    Directory.Delete(dir);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"[ERROR] Fehler beim Löschen des Verzeichnisses {dir}: {ex.Message}");
                }
            }
        }

        private static async Task DownloadFiveM()
        {
            if (File.Exists(FiveMExe))
            {
                Logger.LogInfo("FiveM.exe wurde bereits heruntergeladen.");
                return;
            }

            Logger.LogInfo("Lade FiveM.exe herunter...");
            using (WebClient client = new WebClient())
            {
                try
                {
                    await client.DownloadFileTaskAsync(new Uri(FiveMUrl), FiveMExe);
                    Logger.LogInfo("[SUCCESS] Download abgeschlossen.");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Fehler beim Herunterladen von FiveM.exe: {ex.Message}");
                    Environment.Exit(1);
                }
            }
        }

        private static void StartFiveMInstaller()
        {
            if (!File.Exists(FiveMExe))
            {
                Logger.LogError("FiveM.exe konnte nicht gefunden werden.");
                return;
            }

            Logger.LogInfo("Starte die FiveM-Installation...");
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = FiveMExe,
                    UseShellExecute = true
                });
                Logger.LogSuccess("FiveM-Installation gestartet.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Fehler beim Starten der FiveM-Installation: {ex.Message}");
            }
        }

        //UPDATED! Performance fix!
        public static bool ConfirmAction(string message, int timeoutSeconds = 30)
        {
            if (!string.IsNullOrEmpty(message)) Logger.LogInfo(message);

            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalSeconds < timeoutSeconds)
            {
                string input = Console.ReadLine()?.Trim().ToLower();
                if (input == "y") return true;
                if (input == "n") return false;
                Logger.LogWarning("Ungültige Eingabe. Bitte 'Y' oder 'N' eingeben.");
            }

            Logger.LogWarning("Timeout erreicht. Standardantwort: 'Nein'.");
            return false;
        }

        private static void TryDeleteAndRecreate(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Logger.LogInfo($"Gelöscht: {path}");
                }

                Directory.CreateDirectory(path);
                Logger.LogInfo($"Neu erstellt: {path}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Fehler bei {path}: {ex.Message}");
            }
        }

        public static void UpdateSettings(string key, string value)
        {
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "CitizenFX.ini");

            if (!File.Exists(configPath))
            {
                Logger.LogError("CitizenFX.ini konnte nicht gefunden werden.");
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
                Logger.LogSuccess($"{key} wurde auf {value} gesetzt.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Fehler beim Aktualisieren der Einstellungen: {ex.Message}");
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
                Logger.LogError("CitizenFX.ini konnte nicht gefunden werden.");
                return;
            }

           Logger.LogInfo("Aktuelle Einstellungen in CitizenFX.ini:");
            string[] lines = File.ReadAllLines(configPath);
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

        public static void StartFiveM()
        {
            string fiveMPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.exe");

            if (!File.Exists(fiveMPath))
            {
                Logger.LogError("FiveM.exe konnte nicht gefunden werden.");
                return;
            }

            Logger.LogInfo("Überprüfe, ob FiveM bereits läuft...");

            if (Process.GetProcessesByName("FiveM").Length > 0)
            {
                Logger.LogInfo("FiveM läuft bereits!");
                return;
            }

            Logger.LogInfo("Starte FiveM...");

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = fiveMPath,
                    UseShellExecute = true
                });
                Logger.LogSuccess("FiveM wurde gestartet.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ERROR] Fehler beim Starten von FiveM: {ex.Message}");
            }
        }
    }
}
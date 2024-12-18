using System;
using System.Diagnostics;
using System.IO;

namespace RPToolkidBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Ricis Custom Compiler ===");

            string solutionDir = FindSolutionDirectory();
            if (solutionDir == null)
            {
                Console.WriteLine("[ERROR] Pfad nicht gefunden!");
                Console.ReadKey();
                return;
            }

            string projectPath = Path.Combine(solutionDir, "RPToolkid", "RPToolkid.csproj");

            string compiledDir = Path.Combine(solutionDir, "Compiled", "Release");
            if (!Directory.Exists(compiledDir))
            {
                Directory.CreateDirectory(compiledDir);
            }

            Console.WriteLine("Lösche letzte Builds...");
            foreach (var file in Directory.GetFiles(compiledDir))
            {
                File.Delete(file);
            }

            if (!File.Exists(projectPath))
            {
                Console.WriteLine($"[ERROR] Projektdatei nicht gefunden: {projectPath}");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Erstelle .exe...");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"publish \"{projectPath}\" -c Release -r win-x64 /p:PublishSingleFile=true /p:SelfContained=true /p:EnableCompressionInSingleFile=true -o \"{compiledDir}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }

            process.WaitForExit();
            string finalExe = Path.Combine(compiledDir, "RPToolkid.exe");

            if (File.Exists(finalExe))
            {
                Console.WriteLine($"\n.exe wurde erstellt: {finalExe}");
                RemovePdbFile(compiledDir);
            }
            else
            {
                Console.WriteLine("\nFehler: Die .exe wurde nicht erstellt!");
            }

            Console.WriteLine("Fertig. Drücke eine Taste zum Beenden...");
            Console.ReadKey();
        }

        static string FindSolutionDirectory()
        {
            string currentDir = Directory.GetCurrentDirectory();

            while (!string.IsNullOrEmpty(currentDir))
            {
                if (Directory.GetFiles(currentDir, "*.sln").Length > 0)
                {
                    return currentDir;
                }

                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            return null;
        }

        static void RemovePdbFile(string compiledDir)
        {
            string pdbFile = Path.Combine(compiledDir, "RPToolkid.pdb");
            if (File.Exists(pdbFile))
            {
                Console.WriteLine("Entferne die .pdb-Datei...");
                try
                {
                    File.Delete(pdbFile);
                    Console.WriteLine("Erfolgreich!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Fehler : {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Debug Datei nicht gefunden. Setze fort...");
            }
        }
    }
}

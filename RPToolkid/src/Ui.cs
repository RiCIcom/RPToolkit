using RPToolkid.myutils;

namespace RPToolkid.src
{
    public static class UI
    {
        public static void DisplayHeader()
        {
            Mycstools.SetColor("Magenta");
            Console.WriteLine("  _____  _____    _______ ____   ____  _      _  _______ _______ \r\n" +
                              " |  __ \\|  __ \\  |__   __/ __ \\ / __ \\| |    | |/ /_   _|__   __|\r\n" +
                              " | |__) | |__) |    | | | |  | | |  | | |    | ' /  | |    | |   \r\n" +
                              " |  _  /|  ___/     | | | |  | | |  | | |    |  <   | |    | |  \r\n" +
                              " | | \\ \\| |         | | | |__| | |__| | |____| . \\ _| |_   | |   \r\n" +
                              " |_|  \\_\\_|         |_|  \\____/ \\____/|______|_|\\_\\_____|  |_|  V1.7.5 \r\n");
            Console.ResetColor();
        }

        private static void ShowMenu()
        {
                Console.Clear();
                DisplayHeader();

                Console.WriteLine("Willkommen bei RPToolkid! Wählen Sie eine Option aus:\n");
                Console.WriteLine("1. Cache löschen");
                Console.WriteLine("2. FiveM Neu Installieren");
                Console.WriteLine("3. FiveM Start Config");
                Console.WriteLine("4. Exit");
                Console.Write("\nBitte geben Sie die Nummer Ihrer Wahl ein: ");
        }

        public static void Start()
        {
            bool exit = false;

            while (!exit)
            {
                ShowMenu();
                string input = Console.ReadLine()?.Trim();

                switch (input)
                {
                    case "1":
                        RPUtils.EnsureFiveMNotRunning();
                        RPUtils.ClearCache();
                        break;
                    case "2":
                        RPUtils.EnsureFiveMNotRunning();
                        RPUtils.ReinstallFiveM();
                        break;
                    case "3":
                        exit = false;
                        Console.WriteLine("\nFiveM Start Konfiguration");
                        ConfigureFiveMStart();
                        break;
                    case "4":
                        exit = true;
                        Console.WriteLine("\nProgramm wird beendet...");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nUngültige Auswahl! Bitte versuchen Sie es erneut.");
                        Console.ResetColor();
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nDrücken Sie eine beliebige Taste, um zum Menü zurückzukehren...");
                    Console.ReadKey();
                }
            }
        }

        private static void ConfigureFiveMStart()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("\nWelche Aktion möchten Sie durchführen?");
            Console.WriteLine("1. FiveM starten");
            Console.WriteLine("2. UpdateChannel ändern");
            Console.WriteLine("3. Konfiguration anzeigen");
            Console.WriteLine("4. Zurück zum Hauptmenü");
            Console.Write("\nOption wählen: ");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    RPUtils.StartFiveM();
                    break;

                case "2":
                    Console.Write("Neuen UpdateChannel (release/beta/canary) eingeben: ");
                    Console.WriteLine("\n - 'release': Stabile Version (empfohlen)");
                    Console.WriteLine("\n - 'beta': Testversion mit neuen Funktionen");
                    Console.WriteLine("\n - 'canary': Experimentelle Version für Entwickler");

                    string channel = Console.ReadLine();
                    RPUtils.UpdateSettings("UpdateChannel", channel);

                    break;

                case "3":
                    RPUtils.DisplaySettings();
                    break;

                case "4":
                    Console.WriteLine("Zurück zum Hauptmenü...");
                    break;

                default:
                    Console.WriteLine("Ungültige Auswahl. Bitte erneut versuchen.");
                    break;
            }
        }
    }
}
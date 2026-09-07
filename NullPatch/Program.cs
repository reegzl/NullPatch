using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

class Program
{
    static int idColStart = -1;
    static int verColStart = -1;
    static int availColStart = -1;
    static int sourceColStart = -1;
    static int pinTypeColStart = -1;
    static bool inTable = false;
    static string tableSeparator = string.Empty;

    static void Main(string[] args)
    {
        Console.Title = "NullPatch v1.01";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();

        DrawBanner();
        ShowMenu();
    }

    static void DrawBanner()
    {
        Console.WriteLine(@"
   _   _       _ _ ____       _       _     
  | \ | |_   _| | |  _ \ __ _| |_ ___| |__  
  |  \| | | | | | | |_) / _` | __/ __| '_ \ 
  | |\  | |_| | | |  __/ (_| | || (__| | | |
  |_| \_|\__,_|_|_|_|   \__,_|\__\___|_| |_|
                                            ");
        Console.WriteLine("==============================================");
        Console.WriteLine("  SYSTEM UPDATE ENGINE // SECURE CLI UTILITY  ");
        Console.WriteLine("               CREATED BY REEGZL              ");
        Console.WriteLine("==============================================\n");
    }

    static void ShowMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[SELECT AN OPERATION]");
            Console.ResetColor();

            PrintMenuOption("1", "Scan & Update All Apps", "--include-unknown");
            PrintSimpleMenuOption("2", "Check Available Updates Only");
            PrintComplexMenuOption("3", "Manage Pinned Apps", "Exclude", "Include");
            PrintSimpleMenuOption("4", "Force Reset Winget Source Cache");
            PrintSimpleMenuOption("5", "Exit");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nroot@nullpatch:\\> ");
            Console.ForegroundColor = ConsoleColor.White;

            string choice = Console.ReadLine() ?? string.Empty;
            Console.ResetColor();

            switch (choice.Trim())
            {
                case "1":
                    RunWingetCommand("upgrade --all --include-unknown");
                    break;
                case "2":
                    RunWingetCommand("upgrade --include-unknown");
                    break;
                case "3":
                    ShowPinManagerMenu();
                    break;
                case "4":
                    ResetWingetCache();
                    break;
                case "5":
                    ExitSequence();
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid selection. Try again.");
                    Console.ResetColor();
                    break;
            }
        }
    }

    static void ShowPinManagerMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[PIN MANAGER // EXCLUSIONS]");
            Console.ResetColor();

            PrintSimpleMenuOption("1", "List Active Pins");
            PrintComplexMenuOption("2", "Quick Pin from Updates", "Select from list", null);
            PrintComplexMenuOption("3", "Quick Remove Pin", "Select from active pins", null);
            PrintComplexMenuOption("4", "Pin Package ID", "Ignore Updates", null);
            PrintComplexMenuOption("5", "Remove Pin ID", "Allow Updates Again", null);
            PrintSimpleMenuOption("6", "Return to Main Menu");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nroot@nullpatch:pins\\> ");
            Console.ForegroundColor = ConsoleColor.White;

            string choice = Console.ReadLine() ?? string.Empty;
            Console.ResetColor();

            switch (choice.Trim())
            {
                case "1":
                    RunWingetCommand("pin list");
                    break;
                case "2":
                    QuickPinInteractive();
                    break;
                case "3":
                    QuickRemovePinInteractive();
                    break;
                case "4":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Enter Package ID to pin (e.g., REALiX.HWInfo): ");
                    Console.ForegroundColor = ConsoleColor.White;
                    string pinId = Console.ReadLine() ?? string.Empty;
                    Console.ResetColor();

                    if (!string.IsNullOrWhiteSpace(pinId))
                    {
                        RunWingetCommand($"pin add --id {pinId.Trim()}");
                    }
                    break;
                case "5":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Enter Package ID to unpin: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    string unpinId = Console.ReadLine() ?? string.Empty;
                    Console.ResetColor();

                    if (!string.IsNullOrWhiteSpace(unpinId))
                    {
                        RunWingetCommand($"pin remove --id {unpinId.Trim()}");
                    }
                    break;
                case "6":
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid selection.");
                    Console.ResetColor();
                    break;
            }
        }
    }

    static void PrintSimpleMenuOption(string number, string text)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"  {number}. ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    static void PrintMenuOption(string number, string text, string highlight)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"  {number}. ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{text} (");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(highlight);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(")");
        Console.ResetColor();
    }

    static void PrintComplexMenuOption(string number, string text, string opt1, string? opt2)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"  {number}. ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{text} (");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(opt1);
        if (opt2 != null)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("/");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(opt2);
        }
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(")");
        Console.ResetColor();
    }

    static void QuickPinInteractive()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n[!] Scanning available updates...");
        Console.ResetColor();

        var packages = new List<(string Name, string Id)>();
        int idStart = -1;
        int verStart = -1;
        bool parsingTable = false;

        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            Arguments = "upgrade --include-unknown",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        using (var process = Process.Start(startInfo))
        {
            if (process != null)
            {
                using (var reader = process.StandardOutput)
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("Name") && line.Contains("Id") && line.Contains("Version"))
                        {
                            idStart = line.IndexOf("Id");
                            verStart = line.IndexOf("Version");
                            parsingTable = true;
                        }
                        else if (line.StartsWith("---"))
                        {
                            // separator reached, skip
                        }
                        else if (parsingTable)
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.Contains("package(s) have pins") || line.Contains("upgrades available"))
                            {
                                parsingTable = false;
                                continue;
                            }

                            try
                            {
                                int p1 = idStart;
                                int p2 = verStart;
                                if (p1 > 0 && p2 > p1 && line.Length >= p2)
                                {
                                    string name = line.Substring(0, p1).Trim();
                                    string id = line.Substring(p1, p2 - p1).Trim();
                                    if (!string.IsNullOrEmpty(id) && !name.Equals("Name"))
                                    {
                                        packages.Add((name, id));
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                process.WaitForExit();
            }
        }

        if (packages.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nNo pending updates found to pin.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n[SELECT AN APP TO PIN & IGNORE]");
        Console.ResetColor();

        for (int i = 0; i < packages.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"  [{i + 1}] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(packages[i].Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" (");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(packages[i].Id);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(")");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\nEnter number to pin, type ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("all");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" (0 to cancel): ");
        Console.ForegroundColor = ConsoleColor.White;
        string input = (Console.ReadLine() ?? string.Empty).Trim().ToLower();
        Console.ResetColor();

        if (input == "all")
        {
            foreach (var pkg in packages)
            {
                RunWingetCommand($"pin add --id {pkg.Id}");
            }
        }
        else if (int.TryParse(input, out int selection) && selection > 0 && selection <= packages.Count)
        {
            string targetId = packages[selection - 1].Id;
            RunWingetCommand($"pin add --id {targetId}");
        }
    }

    static void QuickRemovePinInteractive()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n[!] Fetching active pins...");
        Console.ResetColor();

        var pins = new List<(string Name, string Id)>();
        int idStart = -1;
        int verStart = -1;
        bool parsingTable = false;

        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            Arguments = "pin list",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        using (var process = Process.Start(startInfo))
        {
            if (process != null)
            {
                using (var reader = process.StandardOutput)
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("Name") && line.Contains("Id") && line.Contains("Version"))
                        {
                            idStart = line.IndexOf("Id");
                            verStart = line.IndexOf("Version");
                            parsingTable = true;
                        }
                        else if (line.StartsWith("---"))
                        {
                            // separator reached, skip
                        }
                        else if (parsingTable)
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.Contains("There are no pins configured"))
                            {
                                parsingTable = false;
                                continue;
                            }

                            try
                            {
                                int p1 = idStart;
                                int p2 = verStart;
                                if (p1 > 0 && p2 > p1 && line.Length >= p2)
                                {
                                    string name = line.Substring(0, p1).Trim();
                                    string id = line.Substring(p1, p2 - p1).Trim();
                                    if (!string.IsNullOrEmpty(id) && !name.Equals("Name"))
                                    {
                                        pins.Add((name, id));
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                process.WaitForExit();
            }
        }

        if (pins.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nNo active pins found to remove.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n[SELECT AN APP TO UNPIN]");
        Console.ResetColor();

        for (int i = 0; i < pins.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"  [{i + 1}] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(pins[i].Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" (");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(pins[i].Id);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(")");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\nEnter number to unpin, type ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("all");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" (0 to cancel): ");
        Console.ForegroundColor = ConsoleColor.White;
        string input = (Console.ReadLine() ?? string.Empty).Trim().ToLower();
        Console.ResetColor();

        if (input == "all")
        {
            foreach (var pin in pins)
            {
                RunWingetCommand($"pin remove --id {pin.Id}");
            }
        }
        else if (int.TryParse(input, out int selection) && selection > 0 && selection <= pins.Count)
        {
            string targetId = pins[selection - 1].Id;
            RunWingetCommand($"pin remove --id {targetId}");
        }
    }

    static void RunWingetCommand(string arguments)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("\nExecuting: ");
        Console.ResetColor();

        // Always explicitly print "winget" in dark gray first so it's always visible in the command output line
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("winget ");
        Console.ResetColor();

        // Print colored command string tokens
        string[] parts = arguments.Split(' ');
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i];
            if (string.IsNullOrEmpty(part)) continue;

            if (part.Equals("pin", StringComparison.OrdinalIgnoreCase) || part.Equals("upgrade", StringComparison.OrdinalIgnoreCase) || part.Equals("remove", StringComparison.OrdinalIgnoreCase) || part.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(part);
            }
            else if (part.StartsWith("--"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(part);
            }
            else
            {
                // Values / arguments like package IDs
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(part);
            }

            if (i < parts.Length - 1)
            {
                Console.Write(" ");
            }
        }
        Console.WriteLine("\n");

        idColStart = -1;
        verColStart = -1;
        availColStart = -1;
        sourceColStart = -1;
        pinTypeColStart = -1;
        inTable = false;
        tableSeparator = string.Empty;

        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        var outputLines = new List<string>();

        using (var process = new Process { StartInfo = startInfo })
        {
            process.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    lock (outputLines)
                    {
                        outputLines.Add(e.Data);
                    }
                }
            };

            process.ErrorDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    lock (outputLines)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Data);
                        Console.ResetColor();
                    }
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        for (int i = 0; i < outputLines.Count; i++)
        {
            string line = outputLines[i];

            if (line.Contains("Name") && line.Contains("Id"))
            {
                idColStart = line.IndexOf("Id");
                verColStart = line.IndexOf("Version");
                availColStart = line.IndexOf("Available");
                sourceColStart = line.IndexOf("Source");
                pinTypeColStart = line.IndexOf("Pin type");

                inTable = true;

                if (i + 1 < outputLines.Count && outputLines[i + 1].StartsWith("---"))
                {
                    tableSeparator = outputLines[i + 1];
                    i++;
                }
                else
                {
                    tableSeparator = new string('-', Math.Min(line.Length, 100));
                }

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(tableSeparator);

                PrintColoredHeaderRow(line);

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(tableSeparator);
                Console.ResetColor();
            }
            else if (inTable)
            {
                if (string.IsNullOrWhiteSpace(line) || line.Contains("package(s) have pins") || line.Contains("upgrades available") || line.Contains("There are no pins configured"))
                {
                    inTable = false;
                    if (!string.IsNullOrEmpty(tableSeparator))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(tableSeparator);
                        Console.ResetColor();
                    }

                    PrintColoredMessage(line);
                }
                else
                {
                    PrintFormattedDataRow(line);
                }
            }
            else
            {
                PrintColoredMessage(line);
            }
        }

        if (inTable)
        {
            inTable = false;
            if (!string.IsNullOrEmpty(tableSeparator))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(tableSeparator);
                Console.ResetColor();
            }
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n[Operation Completed]");
        Console.ResetColor();
    }

    static void PrintColoredMessage(string line)
    {
        if (line.Contains("have pins that prevent upgrade"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[!] {line.Trim()} (Managed via Pin Manager)");
            Console.ResetColor();
            return;
        }

        if (line.TrimStart().StartsWith("("))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (line.Contains("Successfully") || line.Contains("upgrades available") || line.Contains("Pins successfully"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else if (line.Contains("There are no pins configured.") || line.Contains("No installed package found"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (line.StartsWith("Downloading") || line.StartsWith("Found "))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
        }
        else if (line.Contains("Starting package install") || line.Contains("Extracting"))
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }
        else if (line.Contains("failed") || line.Contains("Error") || line.Contains("error") || line.Contains("blocked") || line.Contains("0x80") || line.Contains("not success"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else if (line.Contains("This application is licensed") || line.Contains("Microsoft is not responsible"))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.WriteLine(line);
        Console.ResetColor();
    }

    static void PrintColoredHeaderRow(string line)
    {
        try
        {
            int p1 = idColStart >= 0 ? Math.Min(idColStart, line.Length) : line.Length;
            int p2 = verColStart >= 0 ? Math.Min(verColStart, line.Length) : line.Length;
            int p3 = availColStart >= 0 ? Math.Min(availColStart, line.Length) : line.Length;
            int p4 = sourceColStart >= 0 ? Math.Min(sourceColStart, line.Length) : line.Length;
            int p5 = pinTypeColStart >= 0 ? Math.Min(pinTypeColStart, line.Length) : line.Length;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(line.Substring(0, p1));

            if (idColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(line.Substring(p1, p2 - p1));
            }

            if (verColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                int next = (availColStart >= 0) ? p3 : (sourceColStart >= 0) ? p4 : (pinTypeColStart >= 0) ? p5 : line.Length;
                Console.Write(line.Substring(p2, next - p2));
            }

            if (availColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                int next = (sourceColStart >= 0) ? p4 : (pinTypeColStart >= 0) ? p5 : line.Length;
                Console.Write(line.Substring(p3, next - p3));
            }

            if (sourceColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                int next = (pinTypeColStart >= 0) ? p5 : line.Length;
                Console.Write(line.Substring(p4, next - p4));
            }

            if (pinTypeColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(line.Substring(p5));
            }
            else
            {
                Console.WriteLine();
            }
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(line);
        }
        Console.ResetColor();
    }

    static void PrintFormattedDataRow(string line)
    {
        try
        {
            int p1 = idColStart >= 0 ? Math.Min(idColStart, line.Length) : line.Length;
            int p2 = verColStart >= 0 ? Math.Min(verColStart, line.Length) : line.Length;
            int p3 = availColStart >= 0 ? Math.Min(availColStart, line.Length) : line.Length;
            int p4 = sourceColStart >= 0 ? Math.Min(sourceColStart, line.Length) : line.Length;
            int p5 = pinTypeColStart >= 0 ? Math.Min(pinTypeColStart, line.Length) : line.Length;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(line.Substring(0, p1));

            if (idColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(line.Substring(p1, p2 - p1));
            }

            if (verColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                int next = (availColStart >= 0) ? p3 : (sourceColStart >= 0) ? p4 : (pinTypeColStart >= 0) ? p5 : line.Length;
                Console.Write(line.Substring(p2, next - p2));
            }

            if (availColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                int next = (sourceColStart >= 0) ? p4 : (pinTypeColStart >= 0) ? p5 : line.Length;
                Console.Write(line.Substring(p3, next - p3));
            }

            if (sourceColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                int next = (pinTypeColStart >= 0) ? p5 : line.Length;
                Console.Write(line.Substring(p4, next - p4));
            }

            if (pinTypeColStart >= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(line.Substring(p5));
            }
            else
            {
                Console.WriteLine();
            }
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(line);
        }
        Console.ResetColor();
    }

    static void ResetWingetCache()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\n[!] Resetting winget source cache (");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("--force");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(")...");
        Console.ResetColor();

        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            Arguments = "source reset --force",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(startInfo))
        {
            process?.WaitForExit();
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n[Cache Reset Complete]");
        Console.ResetColor();
    }

    static void ExitSequence()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n[!] Terminating session...");
        Thread.Sleep(400);
        Console.WriteLine("[!] Clearing local session handles...");
        Thread.Sleep(600);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nConnection closed. Goodbye.");
        Console.ResetColor();
        Thread.Sleep(500);
    }
}
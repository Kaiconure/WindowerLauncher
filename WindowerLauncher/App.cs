using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using File = System.IO.File;

namespace WindowerLauncher
{
    internal class App
    {
        private readonly CommandLine commands;
        private readonly Logger logger = Logger.Instance;
        private readonly FileInfo appFile;
        private readonly DirectoryInfo appDirectory;

        public App(CommandLine commands)
        {
            this.commands = commands;
            this.appFile = new FileInfo(Assembly.GetEntryAssembly().Location)
                ?? throw new Exception("Unable to determine the application path.");
            this.appDirectory = this.appFile.Directory;
        }

        /// <summary>
        /// Cleans up files and folders from older versions, which are no longer relevant. If necessary, upgrades
        /// will occur (converting from old schema to new, etc).
        /// </summary>
        private void VersionCleanup()
        {
            try
            {
                var prompCmd = new FileInfo(Path.Combine(this.appDirectory.FullName, "Command Prompt.cmd"));
                if (prompCmd.Exists)
                {
                    prompCmd.Delete();
                }
            }
            catch(Exception ex)
            {
                this.logger.Log("Warning: Version cleanup failed: {0}", ex.ToString());
            }
        }

        public void Run()
        {
            this.VersionCleanup();

            try
            {
                switch (this.commands.Type)
                {
                    case CommandType.None:
                        this.DoNone();
                        break;
                    case CommandType.New:
                        this.DoNew();
                        break;
                    case CommandType.Save:
                        this.DoSave();
                        break;
                    case CommandType.Run:
                    case CommandType.Activate:
                        this.DoRun();
                        break;
                    case CommandType.Minify:
                        this.DoMinify();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("An unexpected error occurred: {0}", ex.ToString());
            }

            logger.Log("");
        }

        private void DoNone()
        {
            Console.WriteLine("No command specified.");
            Console.WriteLine("Usage: WindowerLauncher <command> [options]");
            Console.WriteLine("Commands:");
            Console.WriteLine("  save       Save your current login configuration.");
            Console.WriteLine("  new        Creates a new, blank profile.");
            Console.WriteLine("  run        Run Windower using the specified profile.");
            Console.WriteLine("  activate   Sets up a profile but does not run it (same as run, but Windower is not launched).");
            Console.WriteLine("  minify     Remove old backups.");
        }

        private void DoSave()
        {
            var hasName = this.commands.GetArgumentString("name", out var name);
            var force = this.commands.GetArgumentBool("force");

            if (!hasName || string.IsNullOrWhiteSpace(name))
            {
                logger.Error("You must specify a name for the login profile using -name:<name>.");
                return;
            }

            this.commands.GetArgumentString("locale", out var locale);
            var polPath = GetPolPath(ref locale);
            if (polPath == null)
            {
                logger.Error("Could not find PlayOnline installation.");
                return;
            }

            var loginDir = new DirectoryInfo(Path.Combine(polPath.FullName, "usr", "all"));
            var loginFile = new FileInfo(Path.Combine(loginDir.FullName, "login_w.bin"));
            if (!loginFile.Exists)
            {
                logger.Error("Could not find PlayOnline login file: {0}", loginFile.FullName);
                return;
            }

            var targetFile = new FileInfo(Path.Combine(appDirectory.FullName, "profiles", locale, $"login_w_{name}.bin"));
            if (targetFile.Exists && !force)
            {
                logger.Error($"A login profile with the name '{name}' already exists. Use -force to overwrite.");
                return;
            }

            if (!targetFile.Directory.Exists)
            {
                targetFile.Directory.Create();
            }

            logger.Log($"Saving your current profile as '{name}'...");
            var fi = loginFile.CopyTo(targetFile.FullName, true);
            if (!fi.Exists)
            {
                logger.Error($"Failed to save login profile for '{name}'.");
                return;
            }

            logger.Log("Saving desktop shortcut...");
            this.commands.GetArgumentInt("leave", out var leave, 10);
            CreateProfileShortcut(name, locale, Math.Max(leave, 1));
        }

        private void DoNew()
        {
            this.commands.GetArgumentString("locale", out var locale);
            var polPath = GetPolPath(ref locale);
            if (polPath == null)
            {
                logger.Error("Could not find PlayOnline installation.");
                return;
            }

            var loginDir = new DirectoryInfo(Path.Combine(polPath.FullName, "usr", "all"));
            var loginFile = new FileInfo(Path.Combine(loginDir.FullName, "login_w.bin"));

            if (loginFile.Exists)
            {
                var backupFile = new FileInfo(Path.Combine(loginDir.FullName, ".wlbackups", $"login_w_backup_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bin"));
                logger.Log($"Backing up current login profile to: {backupFile.FullName}");
                if (!backupFile.Directory.Exists)
                {
                    backupFile.Directory.Create();
                }
                var fi = loginFile.CopyTo(backupFile.FullName, true);
                if (!fi.Exists)
                {
                    logger.Error("Failed to create backup of existing login profile. Aborting.");
                    return;
                }
            }
            else
            {
                logger.Log("No existing login profile was found, so no backup was made. Proceeding.");
            }

            File.WriteAllBytes(loginFile.FullName, Resources.login_w);

            if (this.commands.GetArgumentBool("launch"))
            {
                logger.Log("Your current login profile has been reset. Launching Windower now...");
                this.RunWindower();
            }
            else
            {
                logger.Log("Your current login profile has been reset. You can run POL and set up new credentials now.");
            }
        }

        private void DoRun()
        {
            var hasName = this.commands.GetArgumentString("name", out var name);
            if (!hasName || string.IsNullOrWhiteSpace(name))
            {
                logger.Error("You must specify a name for the login profile using -name:<name>.");
                return;
            }

            this.commands.GetArgumentString("locale", out var locale);
            var polPath = GetPolPath(ref locale);
            if (polPath == null)
            {
                logger.Error("Could not find PlayOnline installation.");
                return;
            }

            var windowerFile = new FileInfo(Path.Combine(appDirectory.FullName, "..", "Windower.exe"));
            if (!windowerFile.Exists)
            {
                logger.Error("Could not find Windower executable: {0}", windowerFile.FullName);
                return;
            }

            var loginDir = new DirectoryInfo(Path.Combine(polPath.FullName, "usr", "all"));
            var loginFile = new FileInfo(Path.Combine(loginDir.FullName, "login_w.bin"));
            if (!loginFile.Exists)
            {
                logger.Error("Could not find PlayOnline login file: {0}", loginFile.FullName);
                return;
            }

            var sourceFile = new FileInfo(Path.Combine(appDirectory.FullName, "profiles", locale, $"login_w_{name}.bin"));
            if (!sourceFile.Exists)
            {
                logger.Error($"A login profile with the name '{name}' could not be found for the {locale} locale!");
                return;
            }

            if (!this.AreFilesIdentical(loginFile, sourceFile))
            {
                var backupFile = new FileInfo(Path.Combine(loginDir.FullName, ".wlbackups", $"login_w_backup_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bin"));
                logger.Log($"Backing up current login profile to: {backupFile.FullName}");
                if (!backupFile.Directory.Exists)
                {
                    backupFile.Directory.Create();
                }
                loginFile.CopyTo(backupFile.FullName, true);

                // Perform a minification step after saving the current backup
                this.DoMinify();

                logger.Log($"Activating profile: {name}");
                sourceFile.CopyTo(loginFile.FullName, true);
            }
            else
            {
                logger.Log($"Profile '{name}' is already active, no changes were made. Proceeding...");
            }

            // Launch windower, unless we're running in "Activate" mode (set up but don't launch)
            if (this.commands.Type != CommandType.Activate)
            {
                this.RunWindower();
            }
        }

        private void DoMinify()
        {
            this.commands.GetArgumentString("locale", out var locale);
            var polPath = GetPolPath(ref locale);
            if (polPath == null)
            {
                logger.Error("Could not find PlayOnline installation.");
                return;
            }

            this.commands.GetArgumentInt("leave", out var leaveCount, 10);
            if (leaveCount < 0) leaveCount = 0;

            var loginDir = new DirectoryInfo(Path.Combine(polPath.FullName, "usr", "all"));

            var backupDir = new DirectoryInfo(Path.Combine(loginDir.FullName, ".wlbackups"));
            if (!backupDir.Exists)
            {
                logger.Log("The backup folder does not exist, there is nothing to clean up.");
            }

            var backups = backupDir.GetFiles("login_w_backup_*.bin")
                .OrderBy(f => f.Name)
                .ToArray();

            if (backups.Length <= leaveCount)
            {
                logger.Log("No old backups were found to delete.");
                return;
            }

            var numDeleted = 0;
            for (var i = 0; i < backups.Length - leaveCount; i++)
            {
                var file = backups[i];
                logger.Log($"Deleting old backup: {file.Name}");
                try
                {
                    file.Delete();
                    numDeleted++;
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to delete backup: {0}", ex.Message);
                }
            }

            logger.Log("Deleted {0} old backup(s).", numDeleted);
        }

        /// <summary>
        /// These are all the possible registry paths for PlayOnline installations. It covers
        /// 32-bit and 64-bit Windows, as well as the US, JP, and EU versions.
        /// </summary>
        static readonly string[] PolRegistryPaths = new[]
        {
            @"SOFTWARE\Wow6432Node\PlayOnlineUS\InstallFolder",
            @"SOFTWARE\Wow6432Node\PlayOnlineJP\InstallFolder",
            @"SOFTWARE\Wow6432Node\PlayOnlineEU\InstallFolder",
            @"SOFTWARE\PlayOnlineUS\InstallFolder",
            @"SOFTWARE\PlayOnlineJP\InstallFolder",
            @"SOFTWARE\PlayOnlineEU\InstallFolder",
        };

        private void CreateProfileShortcut(string profileName, string profileLocale, int leave)
        {
            WshShell wsh = new WshShell();

            var shortcutName = $"{profileName} ({profileLocale}) - WindowerLauncher";
            var shortcutFiles = new FileInfo[]
            {
                new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), shortcutName + ".lnk")),
                //new FileInfo(Path.Combine(appDirectory.FullName, "profiles", shortcutName))
            };

            var args = $"run -name:\"{profileName}\" -locale:{profileLocale} -leave:{leave}";

            foreach (var shortcutFile in shortcutFiles)
            {
                if (!shortcutFile.Directory.Exists)
                {
                    shortcutFile.Directory.Create();
                }

                IWshShortcut shortcut = wsh.CreateShortcut(shortcutFile.FullName) as IWshShortcut;
                shortcut.Arguments = args;
                shortcut.TargetPath = appFile.FullName;
                shortcut.WindowStyle = 1;
                shortcut.Description = $"Runs WindowerLauncher for profile '{profileName}' ({profileLocale} locale)";
                shortcut.WorkingDirectory = appFile.Directory.FullName;
                shortcut.IconLocation = Path.Combine(appFile.Directory.FullName, "Icon.ico");
                shortcut.Save();

                // This nonsense is required to make the "Run as administrator" option checked by default. One would
                // think there would be a better way to do this, but apparently not.
                using (var stream = new FileStream(shortcutFile.FullName, FileMode.Open, FileAccess.ReadWrite))
                {
                    stream.Seek(21, SeekOrigin.Begin);  // Move to byte offset 21
                    int b = stream.ReadByte();          // Read the current byte
                    stream.Seek(21, SeekOrigin.Begin);  // Go back to the position
                    stream.WriteByte((byte)(b | 0x20)); // Set the sixth bit (0x20) to enable "Run as administrator"
                }
            }

            var batchFile = new FileInfo(Path.Combine(appDirectory.FullName, "profiles", shortcutName + ".bat"));
            if (!batchFile.Directory.Exists)
            {
                batchFile.Directory.Create();
            }
            File.WriteAllLines(batchFile.FullName, new string[]
            {
                $"@ECHO OFF",
                $"PUSHD %~dp0",
                $@"..\WindowerLauncher.exe {args}",
            });

        }

        private void RunWindower()
        {
            logger.Log("Launching Windower...");
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Combine(this.appDirectory.FullName, "..", "Windower.exe"),
                WorkingDirectory = this.appDirectory.Parent.FullName,
                UseShellExecute = true,
            };
            var process = System.Diagnostics.Process.Start(startInfo);
        }

        private bool AreFilesIdentical(FileInfo f1, FileInfo f2)
        {
            f1.Refresh();
            f2.Refresh();

            if (!f1.Exists || !f2.Exists)
            { 
                return false; 
            }

            if (f1.Length != f2.Length)
            {
                return false;
            }

            var b1 = File.ReadAllBytes(f1.FullName);
            var b2 = File.ReadAllBytes(f2.FullName);

            if (b1.Length != b2.Length)
            {
                return false;
            }

            return b1.SequenceEqual(b2);
        }

        /// <summary>
        /// Get the PlayOnline installation path from the registry.
        /// </summary>
        /// <returns>Returns a DirectoryInfo object if a valid folder is found, or null otherwise.</returns>
        private DirectoryInfo GetPolPath(ref string locale)
        {
            foreach (var path in PolRegistryPaths)
            {
                if (string.IsNullOrWhiteSpace(locale) || path.EndsWith($@"\PlayOnline{locale}\InstallFolder", StringComparison.OrdinalIgnoreCase))
                {
                    var currentLocale =
                        path.EndsWith(@"\PlayOnlineUS\InstallFolder", StringComparison.OrdinalIgnoreCase) ? "US" :
                        path.EndsWith(@"\PlayOnlineJP\InstallFolder", StringComparison.OrdinalIgnoreCase) ? "JP" :
                        path.EndsWith(@"\PlayOnlineEU\InstallFolder", StringComparison.OrdinalIgnoreCase) ? "EU" : null;

                    if (currentLocale != null)
                    {
                        using (var key = Registry.LocalMachine.OpenSubKey(path))
                        {
                            if (key != null)
                            {
                                var polPath = key.GetValue("1000") as string;
                                if (!string.IsNullOrWhiteSpace(polPath))
                                {
                                    var di = new DirectoryInfo(polPath);
                                    if (di.Exists)
                                    {
                                        logger.Log($"Found PlayOnline{currentLocale} install:\n    {di.FullName}\n");
                                        locale = currentLocale;
                                        return di;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}

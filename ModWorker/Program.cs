using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Threading;

namespace ModWorker
{
    internal class Program
    {
        private static string _workingPath = null;
        public static void Main(string[] args)
        {

            _workingPath = Directory.GetCurrentDirectory();
            string modsPath = Path.Combine(_workingPath, "Mods");
            // Because Windows Lol
            if (_workingPath != null)
            {
                try {
                    Process.GetProcessesByName("VRChat.exe").FirstOrDefault()?.Kill();
                }
                catch (Exception)
                {
                    // safely ignore this exception
                }

                string[] compressedMods = GetAllZipFilesInFolder(modsPath);

                foreach (var compressedMod in compressedMods)
                {
                    Console.WriteLine("Extracting {0} to {1}", compressedMod, _workingPath);
                    ExtractZipFile(compressedMod, Directory.GetCurrentDirectory());
                    File.Delete(compressedMod);
                }
            }
            Thread.Sleep(1000);
            
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(),"Plugins", "ModInstaller.dll"));
          
           StartVRChatAgain();
        }

        private static string[] GetAllZipFilesInFolder(string path)
        {
            var filePaths = Directory.GetFiles(path, "*.zip", SearchOption.AllDirectories);

            FileInfo[] files = filePaths.Select(filePath => new FileInfo(filePath)).ToArray();

            foreach (FileInfo file in files)
            {
                Console.WriteLine($"Found zip file: {file.FullName}");
            }
            return filePaths;
        }

        private static void ExtractZipFile(string zipPath, string extractPath)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // Get the full destination path
                        string destinationPath = Path.Combine(extractPath, entry.FullName);

                        string directoryPath = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        if (!string.IsNullOrEmpty(entry.Name))
                        {
                            if (File.Exists(destinationPath))
                            {
                                File.Delete(destinationPath);
                            }
                            entry.ExtractToFile(destinationPath, overwrite: true);
                        }
                    }
                }

                Console.WriteLine("WroteFiles");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void StartVRChatAgain()
        {
            string arguments = "--modWorkerRan";
            foreach (string stringi in Environment.GetCommandLineArgs())
            {
                arguments += $"{stringi} ";
            }
            System.Diagnostics.Process vrcProcess = new System.Diagnostics.Process();
            vrcProcess.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\VRChat.exe";
            vrcProcess.StartInfo.Arguments = arguments;
            vrcProcess.Start();
        }
        
    }
}
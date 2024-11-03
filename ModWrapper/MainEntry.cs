using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using MelonLoader;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
[assembly: MelonInfo(typeof(ModWrapper.MainEntry), "ModInstaller", "1.0.0", "Zuxi", "")]
namespace ModWrapper
{
    public class MainEntry : MelonPlugin
    {
        MainEntry()
        {
            UnpackAndRunInstaller();
            StartInstaller();
            Process.GetCurrentProcess().Kill();
        }
        
        private static void UnpackAndRunInstaller()
        {
            string baseModWorkerPath = Path.Combine(MelonUtils.BaseDirectory, "ModWorker.exe");
            byte[] rawData = ExtractResource("ModWrapper.ModWorker.exe");
            if (File.Exists(baseModWorkerPath))
            {
                File.Delete(baseModWorkerPath);
            }
            File.WriteAllBytes(baseModWorkerPath, rawData); 
            
        }
        
        private static void StartInstaller()
        {
            string arguments = "--modWorkerPath=" + MelonUtils.BaseDirectory;
            foreach (string stringi in Environment.GetCommandLineArgs())
            {
                arguments += $"{stringi} ";
            }
            System.Diagnostics.Process vrcProcess = new System.Diagnostics.Process();
            vrcProcess.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\ModWorker.exe";
            vrcProcess.StartInfo.Arguments = arguments;
            vrcProcess.Start();
        }

        private static byte[] ExtractResource(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream stream = executingAssembly.GetManifestResourceStream(filename);
            if (stream == null)
            {
                return null;
            }
            byte[] array = new byte[stream.Length];
            var _ = stream.Read(array, 0, array.Length);
            return array;
        }
    }
}
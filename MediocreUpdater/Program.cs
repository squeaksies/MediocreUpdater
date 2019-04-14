using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.ComponentModel;
using System.Threading;
using System.Text.RegularExpressions;

namespace MediocreUpdater
{
    class Program
    {
        static string rootdirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string MMRootDir = rootdirectory + "..\\..";
        static string updateFolder = rootdirectory+"..\\..\\Updates";
        static bool downloading = true;
        static bool printing = false;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Downloading Update Files");
            Directory.CreateDirectory(updateFolder);
            if (Directory.Exists(updateFolder + "\\MediocreMapper"))
            {
                Directory.Delete(updateFolder + "\\MediocreMapper", true);
            }
            downloadFile(args[0], updateFolder);
            while (downloading)
                Thread.Sleep(1000);
        }
        private static void downloadFile(string url, string saveFile)
        {
            
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(updateProgress);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompelete);
            client.DownloadFileAsync(new Uri(url), saveFile + "\\updates.zip");
        }
        private static void updateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!printing)
            {
                printing = true;
                long bytesIn = e.BytesReceived;
                long totalBytes = e.TotalBytesToReceive;
                long percentage = bytesIn * 100 / totalBytes;
                string progress = "";
                for(int i = 0; i < 20; i++)
                {
                    if (percentage - i * 5 > 0)
                        progress += "█";
                    else
                        progress += "░";
                }
                progress += " " + String.Format("{0:0}", percentage) + "%";
                Console.Write("\r" + progress + new string(' ', Console.WindowWidth - progress.Length -1) + "\r");
                printing = false;
            }
            
        }
        private static void downloadCompelete(object sender, AsyncCompletedEventArgs e)
        {
            Console.Write("\r" + "████████████████████ 100%" + new string(' ', Console.WindowWidth - "████████████████████ 100%".Length - 1) + "\r");
            Console.WriteLine("\nUpdate Files downloaded");
            Console.WriteLine("Installing update");
            ZipFile.ExtractToDirectory(updateFolder + "/updates.zip", updateFolder);
            string SourcePath = updateFolder + "/MediocreMapper";
            
            
            string DestinationPath = MMRootDir;
            Console.WriteLine("DestinationPath");
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories)
                .Where(path => new Regex(@"^((?!MediocreUpdater.exe).)*$").IsMatch(path)))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);

            Console.WriteLine("Update Installed");
            //Console.WriteLine("Cleaning up");
            //Directory.Delete(updateFolder, true);
            //Console.WriteLine("Cleanup Complete; now launching Mediocre.");
            Console.WriteLine("now launching MediocreMapper.");
            System.Diagnostics.Process.Start(rootdirectory + "MediocreMapper.exe");
            downloading = false;
        }
    }
    
}

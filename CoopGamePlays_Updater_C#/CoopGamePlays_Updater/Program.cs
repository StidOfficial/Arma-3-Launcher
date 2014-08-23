using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.NetworkInformation;
using System.IO;

namespace CoopGamePlays_Updater
{
    class Program
    {
        public static String ServerAddress;
        public static String ArgArmaServeur = "+connect 62.210.141.88 -port=2302 -password=coopgameplays -mod=@CoopGameplays";
        public static String NameMod = "@CoopGameplays";
        public static String NameAddons = "addons";
        public static String DirectoryArma = @"D:\SteamLibrary\SteamApps\common\Arma 3\";
        public static String DirectoryMod = DirectoryArma + NameMod + @"\";
        public static String DirectoryUpdate = DirectoryMod + NameAddons + @"\";
        public static String ArmaCommandLine = String.Join(" ", Environment.GetCommandLineArgs()).Replace(AppDomain.CurrentDomain.FriendlyName.ToString(), "");
        static void Main(string[] args)
        {
            Console.Title = "CoopGameplays Updater - Arma 3";
            Console.WriteLine("CoopGamePlays Arma 3 Updater [Version 0.1.2014 BETA]");
            Console.WriteLine("Copyright (c) 2014 Stid. Tous droits réservés. \n");
            Console.WriteLine("Lancement de la mise a jours...");
            try
            {
                if (!Directory.Exists(DirectoryArma + NameMod))
                {
                    Directory.CreateDirectory(Path.Combine(DirectoryArma, NameMod));
                }

                if (!Directory.Exists(DirectoryMod + NameAddons))
                {
                    Directory.CreateDirectory(Path.Combine(DirectoryMod, NameAddons));
                }
            }catch (Exception e) { Console.WriteLine(e); }
            Console.WriteLine("Récupération de la liste des fichiers");
            Updater("http://coopgameplays.url.ph/@CoopGameplays/list_files.txt");
            Console.WriteLine("Lancement de Arma 3...");
            System.Diagnostics.Process.Start(DirectoryArma + "arma3.exe", ArgArmaServeur + ArmaCommandLine);
            Console.Read();
            Environment.Exit(0);
        }
        public static void Updater(String Link) {
            String NameFile;
            int Download = 0;
            int Update = 0;
            int ErrorDownload = 0;
            WebClient WebServer = new WebClient();
            try
            {
                StreamReader FileRead = new StreamReader(WebServer.OpenRead(Link));
                while ((NameFile = FileRead.ReadLine()) != null)
                {
                    if (File.Exists(DirectoryUpdate + NameFile))
                    {
                        FileInfo FileInfo = new FileInfo(DirectoryUpdate + NameFile);
                        HttpWebRequest WebSendInfo = (HttpWebRequest)WebRequest.Create("http://coopgameplays.url.ph/@CoopGameplays/addons/" + NameFile);
                        WebSendInfo.Method = "HEAD";
                        HttpWebResponse WebFileInfo = (HttpWebResponse)(WebSendInfo.GetResponse());
                        if (!(FileInfo.Length == WebFileInfo.ContentLength))
                        {
                            Console.WriteLine("Mise a jours de " + NameFile);
                            File.Delete(DirectoryUpdate + NameFile);
                            Update++;
                        }
                        WebFileInfo.Close();
                    }
                    if (!File.Exists(DirectoryUpdate + NameFile))
                    {
                        try
                        {
                            Ping pingSender = new Ping();
                            IPHostEntry AdressIP = Dns.GetHostEntry("coopgameplays.url.ph");
                            PingReply reply = pingSender.Send(AdressIP.HostName);

                            if (reply.Status == IPStatus.Success)
                            {
                                Console.WriteLine("Téléchargement de " + NameFile);
                                WebServer.DownloadFileAsync(new Uri("http://coopgameplays.url.ph/@CoopGameplays/addons/" + NameFile), DirectoryUpdate + NameFile);
                                Download++;
                            }
                        }
                        catch (Exception e) { Console.WriteLine(e); ErrorDownload++; }
                    }
                }
                FileRead.Close();
                FileRead.Dispose();
                FileRead = null;
            }
            catch (Exception e) { Console.WriteLine(e); }
            WebServer.Dispose();
            WebServer = null;
            Console.WriteLine("\nMise a jours terminer : Fichies total " + Download + " - Mise a jours : " + Update + " - Erreur : " + ErrorDownload);
            if(ErrorDownload > 0){
                Console.WriteLine("\nImpossible de lancé Arma 3 car le mod " + NameMod + " à des fichiers manquant !");
                Console.Read();
                Environment.Exit(0);
            }
        }
        public static void ServerResolver()
        {
            Ping pingSender = new Ping();
            IPHostEntry AdressIP = Dns.GetHostEntry("coopgameplays.url.ph");
            PingReply reply = pingSender.Send(AdressIP.HostName);

            if (reply.Status == IPStatus.Success)
            {
                ServerAddress = "http://" + AdressIP.HostName;
            }
            else
            {
                //ServerAddress = "ftp://" + 
            }
        }
    }
}

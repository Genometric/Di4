using System;
using System.Configuration;
using System.IO;
using System.Diagnostics;

namespace Di3BCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Di3B: Dynamic intervals inverted index for Bioinformatics";
            KeyValueConfigurationCollection settings = ReadAndSetConfiguration();

            Console.WriteLine("");
            Console.Write("Initializing ...");
            Herald.Initialize(Herald.Destination.Both, settings["LogFile"].Value);
            Orchestrator orchestrator = new Orchestrator(settings["WorkingDirectory"].Value, Path.GetExtension(settings["LogFile"].Value));
            Console.Write(" Done!");

            Console.WriteLine("");
            Console.Write("Press any key to continue ...");
            Console.ReadKey(true);
            Console.Clear();

            Console.WriteLine("");
            Console.WriteLine(".::.   Hi, I'm Di3, ready and at your service.");
            Console.WriteLine("");
            Console.WriteLine(".::.   Working Directory : {0}", settings["WorkingDirectory"].Value);

            do
            {
                Herald.Announce(Herald.MessageType.None, "");
                Console.Write("> ");
            }
            while (!orchestrator.CommandParse(Console.ReadLine()));

            Herald.Dispose();
        }

        static KeyValueConfigurationCollection ReadAndSetConfiguration()
        {
            var confingFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = confingFile.AppSettings.Settings;

            #region .::.     Set Working Directory     .::.

            Uri wdURI = null;
            string wd = "";
            if (settings["WorkingDirectory"] == null)
            {
                Console.Write("Please specify working directory : ");
                while (Uri.TryCreate(Console.ReadLine(), UriKind.Absolute, out wdURI) == false)
                {
                    Console.WriteLine("Error: Incorrect Format!");
                    Console.Write("Please specify working directory : ");
                }
            }
            else
            {
                Uri.TryCreate(settings["WorkingDirectory"].Value, UriKind.Absolute, out wdURI);
            }

            if (Path.GetExtension(wdURI.AbsolutePath).Trim() == "")
                wd = Path.GetDirectoryName(wdURI.AbsolutePath) + Path.GetFileName(wdURI.AbsolutePath) + Path.DirectorySeparatorChar;
            else
                wd = Path.GetDirectoryName(wdURI.AbsolutePath) + Path.DirectorySeparatorChar;

            if (!Directory.Exists(wd))
            {
                Directory.CreateDirectory(wd);
                Console.WriteLine("The directory is created.");
            }

            settings.Add("WorkingDirectory", wd);
            Console.WriteLine("Working directory is successfully set to : {0}", wd);
            Console.WriteLine(" ");

            #endregion

            #region .::.     Set Log File              .::.
            Uri logFileURI = null;
            string logFilePath = "", logFile = "";
            if (settings["LogFile"] == null)
            {
                Console.Write("Please specify the Log file : ");
                while (Uri.TryCreate(Console.ReadLine(), UriKind.Absolute, out logFileURI) == false ||
                    Path.GetFileName(logFileURI.AbsolutePath) == null ||
                    Path.GetFileName(logFileURI.AbsolutePath).Trim() == "" ||
                    Path.GetDirectoryName(logFileURI.AbsolutePath).Trim() == "")
                {
                    Console.WriteLine("Error: Incorrect Format!");
                    Console.Write("Please specify the Log file : ");
                }
            }
            else
            {
                Uri.TryCreate(Console.ReadLine(), UriKind.Absolute, out logFileURI);
            }

            logFilePath = Path.GetDirectoryName(logFileURI.AbsolutePath);
            logFile = Path.GetFileName(logFileURI.AbsolutePath);

            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
                Console.WriteLine("The directory is created.");
            }

            settings.Add("LogFile", logFilePath + Path.DirectorySeparatorChar + logFile);
            Console.WriteLine("Log file is successfully set to : {0}", logFilePath + Path.DirectorySeparatorChar + logFile);

            #endregion

            confingFile.Save();

            return settings;
        }
    }
}

using System;
using System.Configuration;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI3.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Di3B: Dynamic intervals inverted index for Bioinformatics";
            KeyValueConfigurationCollection settings = ReadAndSetConfiguration();
            if (settings == null) return; // Exit application.

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
            bool updateConfiguration = false;
            var confingFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = confingFile.AppSettings.Settings;

            /// This function needs an update. 
            /// If an invalid configuration or Log file is given by the configuration file, 
            /// then the user must manually correct it. It should be updated to allow user
            /// to change the working directory or log file within the program if an invalid
            /// one is provided. That means updating configuration file shall be supported.

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
                updateConfiguration = true;
            }
            else
            {
                if (Uri.TryCreate(settings["WorkingDirectory"].Value, UriKind.Absolute, out wdURI))
                {
                    Console.WriteLine("Configuration defines a valid working directory.");
                }
                else
                {
                    Console.WriteLine("Configuration defines an invalid working directory. Please manually correct configuration file.");
                    Console.WriteLine("Application will exit now.");
                    Console.ReadKey();
                    return null;
                }
                updateConfiguration = false;
            }

            if (Path.GetExtension(wdURI.AbsolutePath).Trim() == "")
                wd = Path.GetDirectoryName(wdURI.AbsolutePath) + Path.DirectorySeparatorChar + Path.GetFileName(wdURI.AbsolutePath) + Path.DirectorySeparatorChar;
            else
                wd = Path.GetDirectoryName(wdURI.AbsolutePath) + Path.DirectorySeparatorChar;

            wd = Path.GetFullPath(wd);

            if (!Directory.Exists(wd))
            {
                Directory.CreateDirectory(wd);
                Console.WriteLine("The directory is created.");
            }

            if (updateConfiguration)            
                settings.Add("WorkingDirectory", wd);
            
            Console.WriteLine("Working directory is successfully set to : {0}", wd);
            Console.WriteLine(" ");

            #endregion

            #region .::.     Set Log File              .::.
            Uri logFileURI = null;
            string logFilePath = "", logFile = "";
            updateConfiguration = false;
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
                updateConfiguration = true;
            }
            else
            {
                if (Uri.TryCreate(settings["LogFile"].Value, UriKind.Absolute, out logFileURI))
                    Console.WriteLine("Configuration defines a valid Log file.");
                else
                {
                    Console.WriteLine("Configuration defines an invalid Log file. Please manually correct configuration file.");
                    Console.WriteLine("Application will exit now.");
                    Console.ReadKey();
                    return null;
                }
                updateConfiguration = false;
            }

            logFilePath = Path.GetDirectoryName(logFileURI.AbsolutePath);
            logFile = Path.GetFileName(logFileURI.AbsolutePath);

            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
                Console.WriteLine("The directory is created.");
            }

            if (updateConfiguration)
                settings.Add("LogFile", logFilePath + Path.DirectorySeparatorChar + logFile);

            Console.WriteLine("Log file is successfully set to : {0}", logFilePath + Path.DirectorySeparatorChar + logFile);

            #endregion

            if (updateConfiguration)
                confingFile.Save();

            return settings;
        }
    }
}

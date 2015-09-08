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
            KeyValueConfigurationCollection settings = null;
            try
            {
                settings = ReadAndSetConfiguration();
            }
            catch(Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine("Error !! Invalid URI.");
            }
            if (settings == null) return; // Exit application.

            Console.Write("Initializing ...");
            Herald.Initialize(Herald.Destination.Both, settings["LogFile"].Value);
            var indexType = new IndexType();
            if (settings["IndexType"].Value == "y")
                indexType = IndexType.Both;
            else indexType = IndexType.OnlyIncremental;
            Orchestrator orchestrator = new Orchestrator(settings["WorkingDirectory"].Value, Path.GetExtension(settings["LogFile"].Value), indexType, Convert.ToInt32(settings["MinBInCache"].Value), Convert.ToInt32(settings["MaxBInCache"].Value));
            Console.Write(" Done!");

            Console.WriteLine("");
            Console.Write("Press any key to continue ...");
            Console.ReadKey(true);
            Console.Clear();

            Console.WriteLine("");
            Console.WriteLine(".::.   Hi, I'm Di3, ready and at your service.");
            Console.WriteLine("");
            Console.WriteLine(".::.   Working Directory : {0}", settings["WorkingDirectory"].Value);

            try
            {
                do
                {
                    Herald.Announce(Herald.MessageType.None, "");
                    Console.Write("> ");
                }
                while (!orchestrator.CommandParse(Console.ReadLine()));
            }
            catch(Exception e)
            {
                Herald.Announce(Herald.MessageType.Error, e.InnerException.Message);
                Herald.Announce(Herald.MessageType.None, "Please restart the application having resolved the error(s).");
            }

            Herald.Dispose();
        }

        static KeyValueConfigurationCollection ReadAndSetConfiguration()
        {
            bool updateConfiguration = false;
            var confingFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = confingFile.AppSettings.Settings;

            /// This function needs an update .  
            /// If an invalid configuration or Log file is given by the configuration file, 
            /// then the user must manually correct it. It should be updated to allow user
            /// to change the working directory or log file within the program if an invalid
            /// one is provided. That means updating configuration file shall be supported.

            #region .::.     Set Working Directory     .::.

            Uri wdURI = null;
            string wd = "";
            if (settings["WorkingDirectory"] == null)
            {
                Console.WriteLine("Current directory: {0}", Environment.CurrentDirectory);
                Console.Write("Working directory not set yet, Please specify working directory : ");
                while (Uri.TryCreate(Console.ReadLine(), UriKind.RelativeOrAbsolute, out wdURI) == false)
                {
                    Console.WriteLine("Error: Incorrect Format!");
                    Console.Write("Please specify working directory : ");
                }
                updateConfiguration = true;
            }
            else
            {
                if (Uri.TryCreate(settings["WorkingDirectory"].Value, UriKind.RelativeOrAbsolute, out wdURI))
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
                wd = Path.GetDirectoryName(wdURI.AbsoluteUri) + Path.DirectorySeparatorChar;

            wd = Path.GetFullPath(wd);

            if (!Directory.Exists(wd))
            {
                Directory.CreateDirectory(wd);
                Console.WriteLine("The directory is created.");
            }

            if (updateConfiguration)            
                settings.Add("WorkingDirectory", wd);
            
            Console.WriteLine("Working directory is successfully set to : {0}", wd);
            Console.WriteLine("");

            #endregion

            #region .::.     Set Log File              .::.
            Uri logFileURI = null;
            string logFilePath = "", logFile = "";
            updateConfiguration = false;
            if (settings["LogFile"] == null)
            {
                Console.Write("Please specify the Log file : ");
                while (Uri.TryCreate(Console.ReadLine(), UriKind.RelativeOrAbsolute, out logFileURI) == false ||
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
                if (Uri.TryCreate(settings["LogFile"].Value, UriKind.RelativeOrAbsolute, out logFileURI))
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
            Console.WriteLine("");

            #endregion

            #region .::.     Set Index Type File       .::.
            if (settings["IndexType"] == null)
            {
                Console.Write("Do you want to enable inverted index ? [y/n] ");
                char key = Console.ReadKey().KeyChar;
                while (key != 'y' && key != 'n')
                {
                    Console.WriteLine("\nError: Incorrect argument!");
                    Console.Write("Please press \"y\" to Enable the inverted index, or \"n\" to Disable [y/n]: ");
                    key = Console.ReadKey().KeyChar;
                }

                Console.WriteLine("");
                settings.Add("IndexType", key.ToString());

                if (key == 'y')
                    Console.WriteLine("Inverted index is successfully enabled !");
                else
                    Console.WriteLine("Inverted index is successfully disabled !");
            }
            else
            {
                Console.WriteLine("Configuration defines a valid inverted index selection.");
                switch (settings["IndexType"].Value)
                {
                    case "y":
                        Console.WriteLine("Inverted index is Enabled !");
                        break;

                    case "n":
                        Console.WriteLine("Inverted index is Disabled !");
                        break;
                }
            }

            Console.WriteLine("");

            #endregion

            #region .::.   Set Minimum cache size      .::.
            if (settings["MinBInCache"] == null)
            {
                updateConfiguration = true;
                Console.Write("Please specify the minimum history size (#bookmarks) per each index instance : ");
                int minHistorySize;

                while (!int.TryParse(Console.ReadLine(), out minHistorySize))
                {
                    Console.WriteLine("\nError: Incorrect number!");
                    Console.Write("Please specify the minimum history size (#bookmarks) per each index instance : ");
                }

                settings.Add("MinBInCache", minHistorySize.ToString());
            }
            else
            {
                Console.WriteLine("Configuration defines a valid minimum history size.");
            }


            Console.WriteLine(string.Format("Minimum history size is successfully set to <{0:N0}> per each index instance.", settings["MinBInCache"].Value));
            Console.WriteLine("");

            #endregion

            #region .::.   Set Maximum cache size      .::.
            if (settings["MaxBInCache"] == null)
            {
                updateConfiguration = true;
                Console.Write("Please specify the maximum history size (#bookmarks) per each index instance : ");
                int maxHistorySize;

                while (!int.TryParse(Console.ReadLine(), out maxHistorySize))
                {
                    Console.WriteLine("\nError: Incorrect number!");
                    Console.Write("Please specify the maximum history size (#bookmarks) per each index instance : ");
                }

                settings.Add("MaxBInCache", maxHistorySize.ToString());
            }
            else
            {
                Console.WriteLine("Configuration defines a valid maximum history size.");
            }


            Console.WriteLine(string.Format("Maximum history size is successfully set to <{0:N0}> per each index instance.", settings["MaxBInCache"].Value));
            Console.WriteLine("");

            #endregion


            if (updateConfiguration)
                confingFile.Save();

            return settings;
        }
    }
}

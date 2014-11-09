using System;
using System.Configuration;
using System.IO;

namespace Di3BMain
{
    class Program
    {
        static void Main(string[] args)
        {
            var confingFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = confingFile.AppSettings.Settings;
            if (settings["WorkingDirectory"] == null)
                settings.Add("WorkingDirectory", @"I:" + Path.DirectorySeparatorChar + "Di3Data");
            if (settings["LogFile"] == null)
                settings.Add("LogFile", @"I:" + Path.DirectorySeparatorChar + "Di3Data" + Path.DirectorySeparatorChar + "Di3.log");
            confingFile.Save();

            Console.WriteLine("");
            Console.WriteLine(".::.   I'm Di3, Welcome.");
            Console.WriteLine("");
            Console.WriteLine(".::.   Running Directory : {0}", Environment.CurrentDirectory);
            Console.WriteLine("");

            Herald.Initialize(Herald.Destination.Both, settings["LogFile"].Value);

            Orchestrator orchestrator = new Orchestrator(settings["WorkingDirectory"].Value);

            string runResult = "";

            while (runResult != "exit")
            {
                Console.Write("> ");

                runResult = orchestrator.CommandParse(Console.ReadLine());
                Herald.Announce(String.Format("-: Done ...    Runtime: {0}", runResult));
                Herald.Announce("");
            }

            Herald.Dispose();
        }
    }
}

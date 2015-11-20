using System;

namespace Polimi.DEIB.VahidJalili.DI4.CLI
{
    class Program
    {
        public static object Paralle { get; private set; }

        static void Main(string[] args)
        {
            Console.Title = "Di4B: Dynamic intervals incremental inverted index for Bio-informatics";
            Console.Clear();
            Console.WriteLine("Following are the environment and initialization parameters ...");

            try
            {
                if (!UserConfigSetup.SetGet()) return; // Exit application.
            }
            catch (Exception)
            {
                Console.WriteLine("");
                Console.WriteLine("Error !! Invalid URI.");
                return;
            }

            Console.Write("Initializing ...");
            Herald.Initialize(Herald.Destination.Both, UserConfig.logFile);
            Orchestrator orchestrator = new Orchestrator();
            Console.Write(" Done!");

            Console.WriteLine("");
            Console.Write("Press any key to continue ...");
            Console.ReadKey(true);
            Console.Clear();

            Console.WriteLine("");
            Console.WriteLine(".::.   Hi, I'm Di4, ready and at your service.");
            Console.WriteLine("");
            Console.WriteLine(".::.   Working Directory : {0}", UserConfig.workingDirectory);

            try
            {
                do
                {
                    Herald.Announce(Herald.MessageType.None, "");
                    Console.Write("> ");
                }
                while (!orchestrator.CommandParse(Console.ReadLine()));
            }
            catch (Exception e)
            {
                if (e.InnerException == null)
                {
                    Herald.Announce(Herald.MessageType.Error,
                        "My apologies for inconvenience, this was not supposed to happen!" +
                        " Please restart Di4 and if you experience the issue again, please contact the support.");
                }
                else
                {
                    Herald.Announce(Herald.MessageType.Error, e.InnerException.Message);
                    Herald.Announce(Herald.MessageType.None, "Please restart the application having resolved the error(s).");
                }
            }

            Herald.Dispose();
        }
    }
}
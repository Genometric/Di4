using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI3;



namespace Di3BMain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine(".::.   I'm Di3, Welcome.");
            Console.WriteLine("");
            Console.WriteLine(".::.   Running Directory : {0}", Environment.CurrentDirectory);

            Herald.Initialize(Herald.Destination.Both, "E:\\myTestLog.log");


            Orchestrator orchestrator = new Orchestrator();

            string runResult = "";

            while(runResult != "exit")
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
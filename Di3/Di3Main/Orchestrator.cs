using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEDParser;
using DI3;
using Di3Bioinformatics;
using System.Diagnostics;
using Di3BMain.Serializers;

namespace Di3BMain
{
    internal class Orchestrator
    {
        internal Orchestrator()
        {
            di3B = new Di3B<int, PeakClass, PeakDataClass>(23, DataSerializers.Int32, DataSerializers.Peak);
            samplesHashtable = new Hashtable();
            stopWatch = new Stopwatch();
        }

        Di3B<int, PeakClass, PeakDataClass> di3B { set; get; }

        Hashtable samplesHashtable { set; get; }

        Stopwatch stopWatch { set; get; }


        internal string CommandParse(string command)
        {
            string[] splittedCommand = command.Split(' ');

            if (splittedCommand.Length > 1)
            {
                switch (splittedCommand[0].ToLower())
                {
                    case "exit": 
                        return "exit";

                    case "load":
                        stopWatch.Restart();
                        Load(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "index":
                        stopWatch.Restart();
                        Index(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "cover":
                        stopWatch.Restart();
                        Cover(splittedCommand);
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "summit":
                        stopWatch.Restart();
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    default:
                        return "Unknown command";
                }
            }
            else
            {
                if (splittedCommand[0].ToLower() == "exit") return "exit";
                else return "Unknown command, or missing parameters";
            }
        }

        private void Load(string[] args)
        {
            DATA.inputSamples.Add(args[1]);
            BEDParser<PeakClass, PeakDataClass> bedParser = new BEDParser<PeakClass, PeakDataClass>(args[1], "Human");
            DATA.parsedSamples.Add(bedParser.Parse());
            samplesHashtable.Add(args[1], DATA.parsedSamples.Count - 1);
        }

        private void Index(string[] args)
        {
            di3B.Add(DATA.parsedSamples[(int)samplesHashtable[args[1]]].peaks);
        }

        private void Cover(string[] args)
        {
            di3B.Cover(Convert.ToChar(args[1]), Convert.ToByte(args[2]), Convert.ToByte(args[3]), args[4]);
        }

        private void Summit(string[] args)
        {
            di3B.Summit(Convert.ToChar(args[1]), Convert.ToByte(args[2]), Convert.ToByte(args[3]), args[4]);
        }
    }
}

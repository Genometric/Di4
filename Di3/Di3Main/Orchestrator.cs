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
using System.IO;

namespace Di3BMain
{
    internal class Orchestrator
    {
        internal Orchestrator()
        {
            di3B = new Di3B<int, PeakClass, PeakDataClass>(23);
            samplesHashtable = new Dictionary<string, uint>();
            stopWatch = new Stopwatch();
        }

        Di3B<int, PeakClass, PeakDataClass> di3B { set; get; }

        Dictionary<string, UInt32> samplesHashtable { set; get; }

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

                    case "loadall":
                        stopWatch.Restart();
                        LoadAll(splittedCommand);
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
                        // summit function.
                        stopWatch.Stop();
                        return stopWatch.Elapsed.ToString();

                    case "lif":
                        stopWatch.Restart();
                        LIF(splittedCommand);
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
            var parsedSample = bedParser.Parse();
            DATA.parsedSamples.Add(parsedSample.fileHashKey, parsedSample);
            samplesHashtable.Add(args[1], parsedSample.fileHashKey);
        }

        private void LoadAll(string[] args)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(args[1]);

            int counter = -1;

            foreach(var file in dirInfo.GetFiles("*."+args[2]))
            {
                Herald.Announce(String.Format("... Now Loading: [{0}\\{1}] {2}", (++counter), dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Load(new string[] { "null", file.FullName });
            }
        }

        private void LIF(string[] args)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(args[1]);

            int counter = -1;

            foreach (var file in dirInfo.GetFiles("*." + args[2]))
            {
                Herald.Announce(String.Format("... Now Loading: [{0}\\{1}] {2}", (++counter), dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Load(new string[] { "null", file.FullName });

                Herald.Announce(String.Format("... Now Indexing: [{0}\\{1}] {2}", counter, dirInfo.GetFiles("*." + args[2]).Length, file.Name));
                Index(new string[] { "null", file.FullName });

                DATA.parsedSamples.Remove(samplesHashtable[file.FullName]);
            }
        }

        private void Index(string[] args)
        {
            di3B.Add(DATA.parsedSamples[samplesHashtable[args[1]]].peaks);
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

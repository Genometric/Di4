using Polimi.DEIB.VahidJalili.DI4.DI4B.Logging;
using System;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI4.CLI
{
    internal static class Herald
    {
        internal enum Destination { Console = 0, File = 1, Both = 2 };
        internal enum MessageType { Info, Success, Warrning, Error, None };
        internal enum SpeedUnit { intervalPerSecond, bookmarkPerSecond, blockPerSecond };        
        private static Destination _heraldDestination { set; get; }
        internal static StreamWriter writer { set; get; }
        private static StreamWriter _indexSpeedWriter { set; get; }


        internal static void Initialize(
            Destination HeraldDestination,
            string FileFullName)
        {
            _heraldDestination = HeraldDestination;
            if (HeraldDestination == Destination.File ||
                HeraldDestination == Destination.Both)
            {
                if (!Directory.Exists(Path.GetDirectoryName(FileFullName))) Directory.CreateDirectory(Path.GetDirectoryName(FileFullName));
                writer = new StreamWriter(FileFullName, true);

                string speedFile = Path.GetDirectoryName(FileFullName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(FileFullName) + "_indexingSpeed" + Path.GetExtension(FileFullName);
                if (!File.Exists(speedFile))
                {
                    _indexSpeedWriter = new StreamWriter(speedFile, true);
                    _indexSpeedWriter.WriteLine(
                        "Unit\t" + "Count\t" + "ET\t" + "Speed\t" +
                        "InvertedIndexET(sec)\t" + "InvertedIndexSpeed\t" +
                        "IncrementalIndexET(sec)\t" + "IncrementalIndexSpeed");
                }
                else
                {
                    _indexSpeedWriter = new StreamWriter(speedFile, true);
                }
            }
        }

        internal static void Announce(
            MessageType messageType,
            string message,
            Destination heraldDestination = Destination.Both)
        {
            _heraldDestination = heraldDestination;

            switch(messageType)
            {
                case MessageType.Info: break;

                case MessageType.Success:
                    message = "-: Done  ...  " + message;
                    break;

                case MessageType.Warrning: break;

                case MessageType.Error:
                    message = "-: ERROR ...  " + message;
                    break;

                default: break;
            }

            switch (_heraldDestination)
            {
                case Destination.Console:
                    Console.WriteLine(message);
                    break;

                case Destination.File:
                    writer.WriteLine("@" + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
                    writer.WriteLine(message);
                    writer.Flush();
                    break;

                case Destination.Both:
                    Console.WriteLine(message);
                    writer.WriteLine(message);
                    writer.Flush();
                    break;
            }
        }
        internal static void AnnounceExeReport(
            string command,
            ExecutionReport report,
            SpeedUnit speedUnit = SpeedUnit.intervalPerSecond,
            IndexingET indexingET = new IndexingET())
        {
            string sUnit = "";
            switch (speedUnit)
            {
                case SpeedUnit.intervalPerSecond: sUnit = "#i"; break;
                case SpeedUnit.bookmarkPerSecond: sUnit = "#b"; break;
                case SpeedUnit.blockPerSecond: sUnit = "#B"; break;
            }

            if (command.Length > 7) command = command.Substring(0, 4) + "...";

            Announce(MessageType.None,
                /*-*/ string.Format("{0,7} {1}: {2,9}     ET: {3,6}     Speed: {4,14}",
                /*0*/ command,
                /*1*/ sUnit,
                /*2*/ string.Format("{0:N0}", report.count),
                /*3*/ report.ET,
                /*4*/ string.Format("{0:N0} {1}\\sec", Math.Round(report.count / report.ET.TotalSeconds, 2), sUnit)));

            if(command == "Indexed")
            {
                _indexSpeedWriter.WriteLine(
                    sUnit + "\t" +
                    report.count + "\t" +
                    report.ET + "\t" +
                    Math.Round(report.count / report.ET.TotalSeconds, 2) + "\t" +
                    indexingET.InvertedIndex +
                    (indexingET.InvertedIndex == 0 ? "0" : (Math.Round(report.count / indexingET.InvertedIndex)).ToString()) + "\t" +
                    indexingET.IncrementalIndex + "\t" +
                    (indexingET.IncrementalIndex == 0 ? "0" : (Math.Round(report.count / indexingET.IncrementalIndex, 2)).ToString()) + "\t");
                _indexSpeedWriter.Flush();
            }
        }


        internal static void Dispose()
        {
            writer.Close();
            GC.Collect();
        }
    }
}

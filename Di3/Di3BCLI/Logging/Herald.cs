using Polimi.DEIB.VahidJalili.DI3.DI3B.Logging;
using System;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI3.CLI
{
    internal static class Herald
    {
        internal enum Destination { Console = 0, File = 1, Both = 2 };
        internal enum MessageType { Info, Success, Warrning, Error, None };
        internal enum SpeedUnit { intervalPerSecond, bookmarkPerSecond };
        internal static StreamWriter writer { set; get; }
        private static Destination _heraldDestination { set; get; }

        internal static void Initialize(Destination HeraldDestination, string FileFullName)
        {
            _heraldDestination = HeraldDestination;
            if (HeraldDestination == Destination.File ||
                HeraldDestination == Destination.Both)
            {
                if (!Directory.Exists(Path.GetDirectoryName(FileFullName))) Directory.CreateDirectory(Path.GetDirectoryName(FileFullName));
                if (!File.Exists(Path.GetFileName(FileFullName))) File.Create(Path.GetFileName(FileFullName));
                writer = new StreamWriter(FileFullName, true);
            }
        }

        internal static void Announce(MessageType messageType, string message, Destination heraldDestination = Destination.Both)
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

        internal static void AnnounceExeReport(string command, ExecutionReport report, SpeedUnit speedUnit = SpeedUnit.intervalPerSecond)
        {
            string sUnit = "";
            switch (speedUnit)
            {
                case SpeedUnit.intervalPerSecond: sUnit = "#i"; break;
                case SpeedUnit.bookmarkPerSecond: sUnit = "#b"; break;
            }

            Herald.Announce(Herald.MessageType.None,
                /*-*/ String.Format("{0,7} {1}: {2,9}     ET: {3,6}     Speed: {4,14}",
                /*0*/ command,
                /*1*/ sUnit,
                /*2*/ String.Format("{0:N0}", report.count),
                /*3*/ report.ET,
                /*4*/ String.Format("{0:N0} {1}\\sec", Math.Round(report.count / report.ET.TotalSeconds, 2), sUnit)));
        }

        internal static void Dispose()
        {
            writer.Close();
            GC.Collect();
        }
    }
}

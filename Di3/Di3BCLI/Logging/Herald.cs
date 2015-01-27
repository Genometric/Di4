using System;
using System.IO;

namespace Di3BCLI
{
    internal static class Herald
    {
        internal enum Destination { Console = 0, File = 1, Both = 2 };
        internal enum MessageType { Info, Success, Warrning, Error, None };
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
                writer = new StreamWriter(FileFullName);
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

        internal static void Dispose()
        {
            writer.Close();
            GC.Collect();
        }
    }
}

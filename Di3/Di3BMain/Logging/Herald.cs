using System;
using System.IO;

namespace Di3BMain
{
    internal static class Herald
    {
        internal enum Destination { Console = 0, File = 1, Both = 2 };

        private static Destination heraldDestination { set; get; }

        internal static string logFile { set; get; }

        internal static StreamWriter writer { set; get; }


        internal static void Initialize(Destination HeraldDestination, string FileFullName)
        {
            heraldDestination = HeraldDestination;
            if (HeraldDestination == Destination.File ||
                HeraldDestination == Destination.Both)
            {
                if (!Directory.Exists(Path.GetDirectoryName(FileFullName))) Directory.CreateDirectory(Path.GetDirectoryName(FileFullName));
                if (!File.Exists(Path.GetFileName(FileFullName))) File.Create(Path.GetFileName(FileFullName));
                writer = new StreamWriter(FileFullName);
            }
        }

        internal static void Announce(string message)
        {
            switch (heraldDestination)
            {
                case Destination.Console:
                    Console.WriteLine(message);
                    break;

                case Destination.File:
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

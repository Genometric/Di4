using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Genometric.Di4.CLI
{
    class Program
    {
        public static object Paralle { get; private set; }

        static void Main(string[] args)
        {
            /*Test2RI();
            Console.WriteLine("");
            Console.WriteLine("Done ... press any key to exit.");
            Console.ReadKey();
            return;*/

            Console.Title = "Di4B: 1D intervals incremental inverted index for Bio-informatics";
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

        static void Test2RI()
        {
            int repeats = 10;
            UserConfigSetup.SetGet();
            Directory.EnumerateFiles(UserConfig.workingDirectory, "*.idx2R").ToList().ForEach(x => File.Delete(x));
            Herald.Initialize(Herald.Destination.Both, UserConfig.logFile);
            Orchestrator orchestrator = new Orchestrator();
            orchestrator.CommandParse("setdp 4 8");
            Console.Clear();

            int bin = 0;
            Console.WriteLine("Enter bin size :");
            int.TryParse(Console.ReadLine(), out bin);


            var timer = new Stopwatch();
            timer.Start();
            orchestrator.Index2ndResolution(new string[] { "2RI", "uq", bin.ToString() });
            timer.Stop();
            using (var writter = new StreamWriter(UserConfig.workingDirectory + "2riTimeToCreateTest.txt", true))
            {
                writter.WriteLine(bin.ToString() + "\t" + timer.Elapsed.TotalSeconds.ToString());
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "1", "2", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t1\t2\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "5", "10", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t5\t10\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "10", "20", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t10\t20\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "50", "60", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t50\t60\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "80", "90", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t80\t90\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "100", "200", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t100\t200\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "200", "220", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t200\t220\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "300", "320", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t300\t320\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "400", "500", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t400\t500\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "550", "1000", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t550\t1000\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "1", "1", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t1\t1\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "10", "10", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t10\t10\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "25", "25", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t25\t25\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "50", "50", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t50\t50\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "60", "60", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t60\t60\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "80", "80", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t80\t80\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "100", "100", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t100\t100\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }

            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "150", "150", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t150\t150\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "200", "200", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t200\t200\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }


            for (int r = 0; r < repeats; r++)
            {
                timer.Restart();
                orchestrator.Cover(new string[] { "cover", "covres.bed", "*", "500", "500", "count" });
                timer.Stop();
                using (var writter = new StreamWriter(UserConfig.workingDirectory + "CoverResult.txt", true))
                {
                    writter.WriteLine(bin.ToString() + "\t500\t500\t" + timer.Elapsed.TotalSeconds.ToString());
                }
            }

            Herald.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Di3Main
{

    class Program
    {

        static void Main(string[] args)
        {
            List<Di3Main> di3 = new List<Di3Main>();
            for (int i = 0; i < 25; i++)
                di3.Add(new Di3Main());

            Stopwatch parse_Time = new Stopwatch();
            Stopwatch index_Time = new Stopwatch();
            Stopwatch process_Time = new Stopwatch();
            Stopwatch write_Time = new Stopwatch();

            int total_regions = 0;

            for (int i = 0; i < args.Length; i++)
            {
                parse_Time.Start();
                BED_Parser parser = new BED_Parser(args[i], "Human");

                var result = parser.Parse();
                parse_Time.Stop();

                for (int v = 0; v < result.Count; v++)
                    total_regions += result[v].Count;

                index_Time.Start();

                for (int chr = 0; chr < result.Count; chr++)
                {
                    foreach (Peak peak in result[chr])
                    {
                        di3[chr].Insert(peak.start, peak.stop, peak, i, 0, 0);
                    }
                }

                index_Time.Stop();
            }

            List<int> target_List = new List<int>();
            for (int i = 1; i < args.Length; i++)
                target_List.Add(i);

            List<int> source_list = new List<int>();
            source_list.Add(0);


            List<List<Di3Main.Map_Result>> map_Results = new List<List<Di3Main.Map_Result>>();

            process_Time.Start();

            for (int chr = 0; chr < di3.Count; chr++)
            {
                map_Results.Add(di3[chr].GMQL_MAP(source_list, target_List));
            }

            process_Time.Stop();

            write_Time.Start();

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("result.BED"))
            {
                for (int chr = 0; chr < map_Results.Count; chr++)
                {
                    foreach (Di3Main.Map_Result mpR in map_Results[chr])
                    {
                        sw.WriteLine(
                            "chr" + chr.ToString() + "\t" +
                            mpR.peak.start.ToString() + "\t" +
                            mpR.peak.stop.ToString() + "\t" +
                            mpR.peak.name + "\t" +
                            mpR.peak.p_value.ToString() + "\t" +
                            mpR.region_count.ToString());
                    }
                }
            }

            write_Time.Stop();


            Console.WriteLine("Done ...");
            Console.WriteLine("Total determined regions count : {0}", total_regions.ToString());
            Console.WriteLine(".::.     Parser    Time (msec) : {0}", parse_Time.Elapsed.ToString());
            Console.WriteLine(".::.     Index     Time (msec) : {0}", index_Time.Elapsed.ToString());
            Console.WriteLine(".::.  Map Process  Time (msec) : {0}", process_Time.Elapsed.ToString());
            Console.WriteLine(".::.     Write     Time (msec) : {0}", write_Time.Elapsed.ToString());

            int wait = 0;
        }
    }



    

    public sealed class Peak
    {
        public int start { set; get; }
        public int stop { set; get; }
        public string name { set; get; }
        public double p_value { set; get; }

        /// <summary>
        /// This method return hash funtion based on One-at-a-Time method,
        /// which was generated based on Dr. Dobb's methods.
        /// </summary>
        /// <param name="source">The source index of the peak</param>
        /// <returns>The HashKey representing the region</returns>
        public UInt64 Get_HashKey(int source)
        {
            string key = source.ToString() + "|" + start.ToString() + "|" + stop.ToString() + "|";
            int len = key.Length;

            UInt64 hashKey = 0;
            for (int i = 0; i < len; i++)
            {
                hashKey += key[i];
                hashKey += (hashKey << 10);
                hashKey ^= (hashKey >> 6);
            }

            hashKey += (hashKey << 3);
            hashKey ^= (hashKey >> 11);
            hashKey += (hashKey << 15);

            return hashKey;
        }
    }   

    public sealed class General_Features_Data
    {
        public string file_Name { set; get; }
        public string full_Path { set; get; }

        /// <summary>
        /// Chromosome count of the sample which is set in refseq genes parser 
        /// based on the selected species. 
        /// </summary>
        public byte chr_Count { set; get; }
        public int feature_Count { set; get; }
        public List<string> messages { set; get; }

        /// <summary>
        /// All genes of the tab-delimited refseq genes file grouped chromosome-wide.
        /// </summary>
        public List<List<Feature>> features { set; get; }

        /// <summary>
        /// Sets and Gets the species type features belong to.
        /// </summary>
        public string species { set; get; }

        /// <summary>
        /// Contains the determined features.
        /// <para>Each string[] denotes a unique features; the feature index
        /// in the list is the position "feature" in "Feature" class points to.</para>
        /// <para>The second value of the string[] is total number of feature of 
        /// the type present in source file.</para>
        /// </summary>
        public List<string[]> determined_Features { set; get; }

        public class Feature
        {
            /// <summary>
            /// Sets and Gets Start position of the feature, with sequence numbering starting at 1
            /// </summary>
            public int start { set; get; }

            /// <summary>
            /// Sets and Gets Stop position of the feature, with sequence numbering starting at 1.
            /// </summary>
            public int stop { set; get; }

            /// <summary>
            /// Sets and Gets Feature type name (e.g. Gene, Variation, Similarity).
            /// <para>The value is a byte being the hash key of the feature name 
            /// at feature_title_conversion hashtable</para>The value 
            /// </summary>
            public byte feature { set; get; }

            /// <summary>
            /// Sets and Gets a semicolon-separated list of tag-value pairs, providing additional information about each feature.
            /// </summary>
            public string attribute { set; get; }
        }

        public General_Features_Data()
        {
            messages = new List<string>();

            features = new List<List<Feature>>();

            determined_Features = new List<string[]>();
        }
    }

    public sealed class Gene
    {
        public int start { set; get; }
        public int stop { set; get; }
        public char strand { set; get; }
        public string refseq_ID { set; get; }
        public string offi_gene_symbol { set; get; }
    }


  

    



}

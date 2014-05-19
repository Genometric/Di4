using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Di3
{
    class Di3
    {
        public Di3()
        {
            region_count = new int[] { 0, 0, 0 };

            sim_HT = new Hashtable();
            rcc_HT = new Hashtable();
            giK = new Hashtable();
            piK = new Hashtable();
        }

        /// <summary>
        /// The number of various identifiers available. 
        /// The number will be used in estimation of annotation stats.
        /// </summary>
        public int indentifier_count = 4;

        public class Block
        {
            public int index { set; get; }

            /// <summary>
            /// The number of regions (excluding genes) ending at the index.
            /// </summary>
            public byte introduced_stops_count { set; get; }
            public List<Block_Peak_Data> peaks { set; get; }//= new List<Block_Peak_Data>();
            public List<Block_Gene_Data> genes { set; get; }//= new List<Block_Gene_Data>();
            public List<Block_Feature_Data> features { set; get; }

            public Block()
            {
                peaks = new List<Block_Peak_Data>();
                genes = new List<Block_Gene_Data>();
                features = new List<Block_Feature_Data>();
            }

            public class Block_Peak_Data
            {
                /// <summary>
                /// possible values are 1, 2, and 3; indicating :
                /// <para>::    1 : the region starts at this index     ::</para>
                /// <para>::    2 : the region passes    this index     ::</para>
                /// <para>::    3 : the region end    at this index     ::</para>
                /// </summary>
                public byte type { set; get; }

                /// <summary>
                /// Reference to the peak source
                /// </summary>
                public Peak peak_Data { set; get; }

                /// <summary>
                /// The sample index that has the peak.
                /// </summary>
                public int peak_Source { set; get; }

                /// <summary>
                /// values : 
                /// <para>::    4 : Stringent Confirmed     ::</para>
                /// <para>::    5 : Stringent Discarded     ::</para>
                /// <para>::    6 : Weak      Confirmed     ::</para>
                /// <para>::    7 : Weak      Discarded     ::</para>
                /// </summary>
                public byte identifier { set; get; }
            }

            public class Block_Gene_Data
            {
                /// <summary>
                /// possible values are 1, 2, and 3; indicating :
                /// <para>::    1 : the gene starts at this index     ::</para>
                /// <para>::    2 : the gene passes    this index     ::</para>
                /// <para>::    3 : the gene end    at this index     ::</para>
                /// </summary>
                public byte type { set; get; }

                /// <summary>
                /// Reference to the gene source
                /// </summary>
                public Gene gene_Data { set; get; }
            }

            public class Block_Feature_Data
            {
                /// <summary>
                /// possible values are 1, 2, and 3; indicating :
                /// <para>::    1 : the feature starts at this index     ::</para>
                /// <para>::    2 : the feature passes    this index     ::</para>
                /// <para>::    3 : the feature end    at this index     ::</para>
                /// </summary>
                public byte type { set; get; }

                /// <summary>
                /// Gets and Sets the features data.
                /// </summary>
                public General_Features_Data.Feature feature_Data { set; get; }
            }
        }


        public class Ann_Stats_Record
        {
            /// <summary>
            /// The number of regions that their distance from the start of closest 
            /// up-stream gene is within the given range.
            /// </summary>
            public double cuS { set; get; }

            /// <summary>
            /// The number of regions that their distance from the stop of closest
            /// up-stream gene is within the given range.
            /// </summary>
            public double cuE { set; get; }

            /// <summary>
            /// The number of regions that their distance from the start of closest
            /// down-stream gene is within the given range.
            /// </summary>
            public double coS { set; get; }

            /// <summary>
            /// The number of regions that their distance from the stop of closest
            /// down-stream gene is within the given range.
            /// </summary>
            public double coE { set; get; }
        }


        /// <summary>
        /// Annotation Statistics Run Key
        /// </summary>
        public class AnnoStatRunKey
        {
            public bool log10X { set; get; }
            public bool log10Y { set; get; }
            public byte primary_resolution { set; get; }
            public double secondary_resolution { set; get; }
            public List<int> sample_indexes { set; get; }
            public string reference_annotation { set; get; }
            public List<byte> feature_identifiers { set; get; }
            public List<byte> peak_identifiers { set; get; }
        }


        public class Similarity
        {
            /// <summary>
            /// Gets total number of regions/base-pairs of all samples
            /// </summary>
            public int total { internal set; get; }

            /// <summary>
            /// Gets the total number of the regions/base-pairs intersect with each other
            /// </summary>
            public int intersecting_units { internal set; get; }

            /// <summary>
            /// Gets sample wide region/base-pair level similarity.
            /// </summary>
            public List<Sample_wide_similarity> sample_wide_similarity { internal set; get; }

            public Similarity(byte sample_count)
            {
                sample_wide_similarity = new List<Sample_wide_similarity>();

                for (int j = 0; j < sample_count; j++)
                    sample_wide_similarity.Add(new Sample_wide_similarity());
            }

            public class Sample_wide_similarity
            {
                /// <summary>
                /// Gets the index of regions source in cached data.
                /// </summary>
                public int source_sample { internal set; get; }

                /// <summary>
                /// Gets number of regions/base-pairs of the sample that intersect with at least one region/base-pair per sample.
                /// </summary>
                public int intersection_count { internal set; get; }

                /// <summary>
                /// Gets total number of regions/base-pairs of the sample
                /// </summary>
                public int total { internal set; get; }
            }
        }

        /// <summary>
        /// Gets the total number of the indexed peaks 
        /// </summary>
        public int total_peaks_count { internal set; get; }


        private int s { set; get; }
        private int e { set; get; }
        private int Block_count { set; get; }
        private int mid { set; get; }
        private int insert_index { set; get; }

        /// <summary>
        /// Gets the total number of distinct sources of indexed intervals. 
        /// </summary>
        public byte index_source_count { private set; get; }

        private object _interval { set; get; }
        private byte _identifier { set; get; }
        private int _sample_index { set; get; }
        private byte _interval_type { set; get; }
        //private int _start;
        //private int _stop;
        private int _position { set; get; }

        /// <summary>
        /// Gets total number of the indexed regions.
        /// <para>index 0 : total number of indexed regions/peaks.</para>
        /// <para>index 1 : total number of indexed genes.</para>
        /// </summary>
        public int[] region_count { internal set; get; }

        private byte _annotation_identifier { set; get; }

        /// <summary>
        /// General feature Identifiers Key. 
        /// <para>This hashtable contains the idenfiers of Features that
        /// will be considered for annotation stats generation</para>
        /// </summary>
        private Hashtable giK { set; get; }


        /// <summary>
        /// Peak Identifiers Key.
        /// <para>This hashtable contains the identifiers of peaks that
        /// will be considered for annotation stats generation</para>
        /// </summary>
        private Hashtable piK { set; get; }


        private Similarity[] similarity { set; get; }



        /// <summary>
        /// region count control hashtable.
        /// This table helps keepting track of counted regions for intersecting
        /// regions, hence avoid counting a region more than once.
        /// </summary>
        private Hashtable rcc_HT { set; get; }

        /// <summary>
        /// Sample Index Mapping Hashtable.
        /// This table has two main functionalities:
        /// <para>1. It helps counting the number of the distinct sources of intervals.</para>        
        /// <para>2. It helps to map from sample index to array indexes that keep sample-wide information.
        /// <para>For example, if the first region being inserted has a sample index 5, then this sample will always
        /// has it's data on 0-th index of arrays. If the second region being insered has sample index 2, 
        /// then the sample will always has it's data on 1-th index of arrays.</para></para>
        /// </summary>
        public Hashtable sim_HT { set; get; }


        public List<Block> index = new List<Block>();

        public int source_index { set; get; }


        /// <summary>
        /// Inserts the given interval into Di2-space.
        /// </summary>
        /// <param name="start">Start position of the interval.</param>
        /// <param name="stop">Stop position of the interval.</param>
        /// <param name="interval">The interval to insert into Di2-space.</param>
        /// <param name="sample_index">The interval's source index in cached data.</param>
        /// <param name="interval_type">Intervals could be of different type. Following types are supported:
        /// <para>Regions/Peak      ->   interval_type = 0</para>
        /// <para>Refseq Gene       ->   interval_type = 1</para>
        /// <para>General Features  ->   interval_type = 2</para></param>
        /// <param name="identifier">Intervals of the same type could represent different semantics (e.g., 
        /// Stringent-Confirmed peaks, Weak-Confirmed peaks). Hence assigning different identifier's to 
        /// intervals of different type to facilitate the discrimination between them.</param>
        public void Insert(int start, int stop, object interval, int sample_index, byte interval_type, byte identifier)
        {
            _interval = interval;
            _identifier = identifier;
            _sample_index = sample_index;
            _interval_type = interval_type;

            if (sim_HT.ContainsKey(_sample_index) == false)
            { index_source_count++; sim_HT.Add(_sample_index, sim_HT.Count); }


            region_count[_interval_type]++;


            _position = start;
            Find_Start_Insert_Index();
            Insert_New_Block(1);

            insert_index++;

            _position = stop;
            Find_Stop__Insert_Index();
            Insert_New_Block(3);
        }

        private void Insert_New_Block(byte type)
        {
            // Shall new Block be added to the end of list ?
            if (insert_index < Block_count) // No
            {
                // Does the same index already available ?
                if (_position != index[insert_index].index) // No, add new index then
                {
                    index.Insert(insert_index, Get_new_Block(type));
                    Block_count++;
                }
                else // Yes, then update the available index with new region
                {
                    Update_Block(type);
                }
            }
            else // Yes, then add new index
            {
                index.Insert(insert_index, Get_new_Block(type));
                Block_count++;
            }
        }
        private void Find_Start_Insert_Index()
        {
            s = 0;
            Block_count = index.Count;
            e = Block_count;
            mid = 0;
            insert_index = 0;

            while (true)
            {
                mid = (int)Math.Floor((s + e) / 2.0);

                if (mid == e)
                {
                    insert_index = mid;
                    break;
                }
                else if (_position < index[mid].index)
                {
                    if (mid == 0)
                    {
                        insert_index = 0;
                        break;
                    }
                    else
                    {
                        e = mid;
                    }
                }
                else if (_position == index[mid].index)
                {
                    insert_index = mid;
                    break;
                }
                else
                {
                    s = mid;

                    if (s == e - 1)
                    {
                        if (_position < index[s].index)
                        {
                            insert_index = s;
                            break;
                        }
                        else
                        {
                            if (e < Block_count)
                            {
                                if (_position < index[e].index)
                                {
                                    insert_index = e;
                                    break;
                                }
                                else
                                {
                                    insert_index = e + 1;
                                    break;
                                }
                            }
                            else
                            {
                                insert_index = e;
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void Find_Stop__Insert_Index()
        {
            for (int i = insert_index; i < Block_count; i++)
            {
                if (_position < index[i].index)
                {
                    insert_index = i;
                    break;
                }
                else if (_position > index[i].index)
                {
                    Update_Block(2);

                    insert_index++;
                }
                else
                {
                    break;
                }
            }
        }

        private Block Get_new_Block(byte type)
        {
            Block b = new Block() { index = _position, introduced_stops_count = 0 };
            if (type == 3) b.introduced_stops_count = 1;

            if (_interval_type == 0)
            {
                b.peaks.Add(new Block.Block_Peak_Data()
                {
                    type = type,
                    peak_Source = _sample_index,
                    peak_Data = (Peak)_interval,
                    identifier = _identifier
                });
            }
            else if (_interval_type == 1)
            {
                b.genes.Add(new Block.Block_Gene_Data()
                {
                    type = type,
                    gene_Data = (Gene)_interval
                });
            }
            else if (_interval_type == 2)
            {
                b.features.Add(new Block.Block_Feature_Data()
                {
                    type = type,
                    feature_Data = (General_Features_Data.Feature)_interval
                });
            }

            // Will new Block be added to the end of list ?
            if (insert_index < Block_count) // No, then copy data from subsequent Block
            {
                // update peaks
                foreach (var d in index[insert_index].peaks)
                {
                    if (d.type != 1)
                    {
                        b.peaks.Add(new Block.Block_Peak_Data()
                        {
                            type = 2,
                            peak_Source = d.peak_Source,
                            peak_Data = d.peak_Data,
                            identifier = d.identifier
                        });
                    }
                }

                // update genes
                foreach (var d in index[insert_index].genes)
                {
                    if (d.type != 1)
                    {
                        b.genes.Add(new Block.Block_Gene_Data()
                        {
                            type = 2,
                            gene_Data = d.gene_Data,
                        });
                    }
                }

                // update general features
                foreach (var d in index[insert_index].features)
                {
                    if (d.type != 1)
                    {
                        b.features.Add(new Block.Block_Feature_Data()
                        {
                            type = 2,
                            feature_Data = d.feature_Data
                        });
                    }
                }
            }

            return b;
        }
        private void Update_Block(byte type)
        {
            if (_interval_type == 0)
            {
                index[insert_index].peaks.Add(new Block.Block_Peak_Data()
                {
                    type = type,
                    peak_Source = _sample_index,
                    peak_Data = (Peak)_interval,
                    identifier = _identifier
                });
            }
            else if (_interval_type == 1)
            {
                index[insert_index].genes.Add(new Block.Block_Gene_Data()
                {
                    type = type,
                    gene_Data = (Gene)_interval,
                });
            }

            if (type == 3)
            {
                index[insert_index].introduced_stops_count++;
            }
        }




        /// <summary>
        /// Estimates the similarity between the various indexed sets of regions.
        /// </summary>
        /// <returns></returns>
        public Similarity[] Get_Similarity()
        {
            similarity = new Similarity[] { new Similarity(index_source_count), new Similarity(index_source_count) };

            return Sim_Cover(index_source_count, index_source_count);
        }

        /// <summary>
        /// Determines the regions satisfying min_Acc and max_Acc condtions for region accumulation 
        /// </summary>
        /// <param name="min_Acc">minimum required accumulation</param>
        /// <param name="max_Acc">maximum required accumulation</param>
        /// <returns>An array of two of similarity reports among samples based on Cover funtion.
        /// <para>First array represents Region level similarity</para>
        /// <para>Second array represents Base-pair level similarity</para></returns>
        private Similarity[] Sim_Cover(int min_Acc, int max_Acc)
        {
            rcc_HT.Clear();

            Similarity[] rtv = new Similarity[] { new Similarity(index_source_count), new Similarity(index_source_count) };

            int[] Int_Peak = new int[] { -1, -1 }; // 0: index , 1: value

            byte index_cout = 0;

            for (int k = 0; k < index.Count; k++)
            {
                index_cout = (byte)(index[k].peaks.Count - index[k].introduced_stops_count);

                if (Int_Peak[1] == -1 &&
                    index_cout >= min_Acc &&
                    index_cout <= max_Acc)
                {
                    Int_Peak[0] = k;
                    Int_Peak[1] = index_cout;

                    /// This line is new and guarantees to count a region only once
                    Update_intervals_wise_similarity(k);
                }
                else if (Int_Peak[1] != -1 && (
                    index_cout < min_Acc ||
                    index_cout > max_Acc))
                {
                    /// This line is new and guarantees to count a region only once
                    Update_intervals_wise_similarity(k);

                    similarity[1].intersecting_units += (index[k].index - index[Int_Peak[0]].index);

                    Int_Peak[0] = -1;
                    Int_Peak[1] = -1;
                }
            }

            similarity[0].total = region_count[0];
            similarity[1].total = Get_Union_bp();

            for (int i = 0; i < sim_HT.Count; i++)
            {
                int tkey = 0;
                foreach (DictionaryEntry de in sim_HT)
                {
                    if ((int)de.Value == i)
                    {
                        tkey = (int)de.Key;
                        break;
                    }
                }

                similarity[0].sample_wide_similarity[i].source_sample = tkey;
                similarity[1].sample_wide_similarity[i].source_sample = tkey;
                similarity[1].sample_wide_similarity[i].intersection_count = similarity[1].intersecting_units;
                similarity[1].sample_wide_similarity[i].total = Get_Sample_bp(tkey);
            }

            return similarity;
        }

        private void Update_intervals_wise_similarity(int k)
        {
            for (int i = 0; i < (byte)(index[k].peaks.Count); i++)
            {
                UInt64 temp_HashKey = index[k].peaks[i].peak_Data.Get_HashKey(index[k].peaks[i].peak_Source);

                if (index[k].peaks[i].type != 3 &&
                    rcc_HT.Contains(temp_HashKey) == false)
                {
                    similarity[0].intersecting_units++;
                    similarity[0].sample_wide_similarity[(int)sim_HT[index[k].peaks[i].peak_Source]].intersection_count++;

                    rcc_HT.Add(temp_HashKey, 'H');
                }
                else if (index[k].peaks[i].type == 3 &&
                    rcc_HT.Contains(temp_HashKey) == true)
                {
                    rcc_HT.Remove(temp_HashKey);
                }
            }
        }



        /// <summary>
        /// Return the number of the base-pairs of the union of all indexed samples
        /// </summary>
        /// <returns>the number of the base-pairs of the union of all indexd samples</returns>
        private int Get_Union_bp()
        {
            int rtv = 0;

            int temp_start = int.MaxValue, temp_stop = int.MaxValue;

            int[] Int_Peak = new int[] { -1, -1 }; // 0: index , 1: value

            byte index_cout = 0;

            for (int k = 0; k < index.Count; k++)
            {
                index_cout = (byte)(index[k].peaks.Count - index[k].introduced_stops_count);

                if (Int_Peak[1] == -1 &&
                    index_cout >= 1)
                {
                    Int_Peak[0] = k;
                    Int_Peak[1] = index_cout;
                }
                else if (Int_Peak[1] != -1 && index_cout < 1)
                {
                    temp_start = int.MaxValue;
                    for (int i = 0; i < index[Int_Peak[0]].peaks.Count; i++)
                        temp_start = Math.Min(temp_start, index[Int_Peak[0]].peaks[i].peak_Data.start);


                    temp_stop = int.MaxValue;
                    for (int i = 0; i < index[k].peaks.Count; i++)
                        temp_stop = Math.Min(temp_stop, index[k].peaks[i].peak_Data.stop);

                    rtv += temp_stop - temp_start;

                    Int_Peak[0] = -1;
                    Int_Peak[1] = -1;
                }
            }


            return rtv;
        }

        /// <summary>
        /// Returns the number of the base-pairs covered by the sample
        /// </summary>
        /// <returns></returns>
        private int Get_Sample_bp(int peak_Source)
        {
            int rtv = 0;

            for (int k = 0; k < index.Count; k++)
            {
                for (int i = 0; i < index[k].peaks.Count; i++)
                {
                    if (index[k].peaks[i].type == 1)
                    {
                        if (index[k].peaks[i].peak_Source == peak_Source)
                        {
                            rtv += (index[k].peaks[i].peak_Data.stop - index[k].peaks[i].peak_Data.start);
                        }
                    }
                }
            }

            return rtv;
        }



        public int Find_Position(int position)
        {
            _position = position;
            Find_Start_Insert_Index();
            return insert_index--;
        }

        /// <summary>
        /// Estimates an overview of annotation stats for region in given chromosome range
        /// </summary>
        /// <param name="start_chr">Start chromosome for annotation stats estimation</param>
        /// <param name="stop_chr">Stop chromosome for annotation stats estimation
        /// (stop chromosome is included)</param>
        /// <param name="resolution">The resolution in which the statistics need to be estimated</param>
        /// <returns></returns>
        public SortedList Get_Annotation_Stats(AnnoStatRunKey runKey)
        {
            SortedList rtv = new SortedList();
            Hashtable stats_HT = new Hashtable();
            Hashtable peak_Source_HT = new Hashtable();

            giK.Clear();
            for (int i = 0; i < runKey.feature_identifiers.Count; i++)
            {
                giK.Add(runKey.feature_identifiers[i], "Hamed");
            }

            piK.Clear();
            for (int i = 0; i < runKey.peak_identifiers.Count; i++)
            {
                piK.Add(runKey.peak_identifiers[i], "Hamed");
            }

            for (int i = 0; i < runKey.sample_indexes.Count; i++)
            {
                peak_Source_HT.Add(runKey.sample_indexes[i], "Hamed");
            }


            for (int i = 0; i < index.Count; i++)
            {
                for (int j = 0; j < index[i].peaks.Count; j++)
                {
                    if (index[i].peaks[j].type == 1 && peak_Source_HT.ContainsKey(index[i].peaks[j].peak_Source) && piK.ContainsKey(index[i].peaks[j].identifier))
                    {
                        Ann_Stats_Record result = new Ann_Stats_Record();
                        if (string.Equals(runKey.reference_annotation, "refseq genes")) result = Get_interval_Annotation_relative_to_genes(i, j);
                        else if (string.Equals(runKey.reference_annotation, "features")) result = Get_interval_Annotation_relative_to_general_features(i, j);

                        if (result.cuS != int.MinValue)
                        {
                            result.cuS = (int)(Math.Round(result.cuS / runKey.secondary_resolution, runKey.primary_resolution) * runKey.secondary_resolution);
                            result.cuE = (int)(Math.Round(result.cuE / runKey.secondary_resolution, runKey.primary_resolution) * runKey.secondary_resolution);

                            #region .::.        process :   Start   Up-Stream       .::.

                            if (stats_HT.ContainsKey(result.cuS))
                            {
                                var current_value = ((Ann_Stats_Record)stats_HT[result.cuS]);
                                current_value.cuS++;

                                stats_HT[result.cuS] = current_value;
                            }
                            else
                            {
                                stats_HT.Add(result.cuS, new Ann_Stats_Record() { cuS = 1, cuE = 0, coS = 0, coE = 0 });
                            }

                            #endregion
                            #region .::.        process :   Stop    Up-Stream       .::.

                            if (stats_HT.ContainsKey(result.cuE))
                            {
                                var current_value = ((Ann_Stats_Record)stats_HT[result.cuE]);
                                current_value.cuE++;

                                stats_HT[result.cuE] = current_value;
                            }
                            else
                            {
                                stats_HT.Add(result.cuE, new Ann_Stats_Record() { cuS = 0, cuE = 1, coS = 0, coE = 0 });
                            }

                            #endregion
                        }

                        // If the region intersects with a gene, it is not required to add the stats to closest down-stream gene.
                        if ((result.cuS > 0 || result.cuS == int.MinValue) && result.coS != int.MinValue)
                        {
                            result.coS = (int)(Math.Round(result.coS / runKey.secondary_resolution, runKey.primary_resolution) * runKey.secondary_resolution);
                            result.coE = (int)(Math.Round(result.coE / runKey.secondary_resolution, runKey.primary_resolution) * runKey.secondary_resolution);

                            #region .::.        process :   Start   Down-Stream     .::.

                            if (stats_HT.ContainsKey(result.coS))
                            {
                                var current_value = ((Ann_Stats_Record)stats_HT[result.coS]);
                                current_value.coS++;

                                stats_HT[result.coS] = current_value;
                            }
                            else
                            {
                                stats_HT.Add(result.coS, new Ann_Stats_Record() { cuS = 0, cuE = 0, coS = 1, coE = 0 });
                            }

                            #endregion
                            #region .::.        process :   Stop    Down-Stream     .::.

                            if (stats_HT.ContainsKey(result.coE))
                            {
                                var current_value = ((Ann_Stats_Record)stats_HT[result.coE]);
                                current_value.coE++;

                                stats_HT[result.coE] = current_value;
                            }
                            else
                            {
                                stats_HT.Add(result.coE, new Ann_Stats_Record() { cuS = 0, cuE = 0, coS = 0, coE = 1 });
                            }

                            #endregion
                        }
                    }
                }
            }


            double dicEnt_Key = 0.0;
            Ann_Stats_Record dicEnt_Val = new Ann_Stats_Record();

            foreach (DictionaryEntry dicEnt in stats_HT)
            {
                dicEnt_Key = Convert.ToInt32(dicEnt.Key);
                if (runKey.log10X)
                {
                    if (dicEnt_Key > 0) dicEnt_Key = Math.Log10(dicEnt_Key);
                    else dicEnt_Key = (-1) * Math.Log10(Math.Abs(dicEnt_Key));
                    if (dicEnt_Key == double.PositiveInfinity) dicEnt_Key = 0;
                }


                dicEnt_Val = (Ann_Stats_Record)dicEnt.Value;
                if (runKey.log10Y)
                {
                    if (dicEnt_Val.cuS != 0) dicEnt_Val.cuS = Math.Log10(dicEnt_Val.cuS);
                    if (dicEnt_Val.cuE != 0) dicEnt_Val.cuE = Math.Log10(dicEnt_Val.cuE);
                    if (dicEnt_Val.coS != 0) dicEnt_Val.coS = Math.Log10(dicEnt_Val.coS);
                    if (dicEnt_Val.coE != 0) dicEnt_Val.coE = Math.Log10(dicEnt_Val.coE);
                }

                rtv.Add(dicEnt_Key, dicEnt_Val);
            }

            return rtv;
        }


        private Ann_Stats_Record Get_interval_Annotation_relative_to_genes(int Block_index, int region_index)
        {
            Ann_Stats_Record rtv = new Ann_Stats_Record();

            var source = index[Block_index];

            rtv.cuS = int.MinValue;
            rtv.cuE = int.MinValue;
            rtv.coS = int.MinValue;
            rtv.coE = int.MinValue;

            if (source.genes.Count != 0)
            {
                #region .::.        The Region Intersects with a Gene        .::.

                int max_gene_start = 0;
                int max_gene_index = 0;

                for (int i = 0; i < source.genes.Count; i++)
                {
                    if (source.genes[i].type == 2)
                    {
                        if (source.genes[i].gene_Data.start > max_gene_start)
                        {
                            max_gene_start = source.genes[i].gene_Data.start;
                            max_gene_index = i;
                        }
                    }
                    else if (source.genes[i].type == 1)
                    {
                        rtv.cuS = 0;
                        rtv.cuE = source.genes[i].gene_Data.start - source.genes[i].gene_Data.stop;

                        return rtv;
                    }
                }

                rtv.cuS = source.genes[max_gene_index].gene_Data.start - source.peaks[region_index].peak_Data.start;
                rtv.cuE = source.peaks[region_index].peak_Data.start - source.genes[max_gene_index].gene_Data.stop;

                return rtv;

                #endregion
            }
            else
            {
                #region .::.        Search Closest Gene Up-Stream            .::.

                bool gene_found = false;

                for (int i = Block_index - 1; (i >= 0 && !gene_found); i--)
                {
                    for (int j = 0; j < index[i].genes.Count; j++)
                    {
                        if (index[i].genes[j].type == 1)
                        {
                            rtv.cuS = source.peaks[region_index].peak_Data.start - index[i].genes[j].gene_Data.start;
                            rtv.cuE = source.peaks[region_index].peak_Data.start - index[i].genes[j].gene_Data.stop;

                            gene_found = true;
                            break;
                        }
                    }
                }

                #endregion

                #region .::.        Search Closest Gene Down-Stream          .::.

                gene_found = false;

                for (int i = Block_index + 1; (i < index.Count && !gene_found); i++)
                {
                    for (int j = 0; j < index[i].genes.Count; j++)
                    {
                        if (index[i].genes[j].type == 1)
                        {
                            rtv.coS = index[i].genes[j].gene_Data.start - source.peaks[region_index].peak_Data.start;
                            rtv.coE = index[i].genes[j].gene_Data.stop - source.peaks[region_index].peak_Data.start;

                            gene_found = true;
                            break;
                        }
                    }
                }

                #endregion
            }

            return rtv;
        }
        private Ann_Stats_Record Get_interval_Annotation_relative_to_general_features(int Block_index, int region_index)
        {
            Ann_Stats_Record rtv = new Ann_Stats_Record();

            var source = index[Block_index];

            rtv.cuS = int.MinValue;
            rtv.cuE = int.MinValue;
            rtv.coS = int.MinValue;
            rtv.coE = int.MinValue;

            if (source.features.Count != 0)
            {
                #region .::.        The Region Intersects with a Feature        .::.

                int max_gene_start = 0;
                int max_gene_index = 0;

                for (int i = 0; i < source.features.Count; i++)
                {
                    if (giK.ContainsKey(source.features[i].feature_Data.feature))
                    {
                        if (source.features[i].type == 2)
                        {
                            if (source.features[i].feature_Data.start > max_gene_start)
                            {
                                max_gene_start = source.features[i].feature_Data.start;
                                max_gene_index = i;
                            }
                        }
                        else if (source.features[i].type == 1)
                        {
                            rtv.cuS = 0;
                            rtv.cuE = source.features[i].feature_Data.start - source.features[i].feature_Data.stop;

                            return rtv;
                        }
                    }
                }

                rtv.cuS = source.features[max_gene_index].feature_Data.start - source.features[region_index].feature_Data.start;
                rtv.cuE = source.features[region_index].feature_Data.start - source.features[max_gene_index].feature_Data.stop;

                return rtv;

                #endregion
            }
            else
            {
                #region .::.        Search Closest Feature Up-Stream            .::.

                bool feature_found = false;

                for (int i = Block_index - 1; (i >= 0 && !feature_found); i--)
                {
                    for (int j = 0; j < index[i].features.Count; j++)
                    {
                        if (giK.ContainsKey(index[i].features[j].feature_Data.feature) && index[i].features[j].type == 1)
                        {
                            rtv.cuS = source.peaks[region_index].peak_Data.start - index[i].features[j].feature_Data.start;
                            rtv.cuE = source.peaks[region_index].peak_Data.start - index[i].features[j].feature_Data.stop;

                            feature_found = true;
                            break;
                        }
                    }
                }

                #endregion

                #region .::.        Search Closest Feature Down-Stream          .::.

                feature_found = false;

                for (int i = Block_index + 1; (i < index.Count && !feature_found); i++)
                {
                    for (int j = 0; j < index[i].features.Count; j++)
                    {
                        if (giK.ContainsKey(index[i].features[j].feature_Data.feature) && index[i].features[j].type == 1)
                        {
                            rtv.coS = index[i].features[j].feature_Data.start - source.peaks[region_index].peak_Data.start;
                            rtv.coE = index[i].features[j].feature_Data.stop - source.peaks[region_index].peak_Data.start;

                            feature_found = true;
                            break;
                        }
                    }
                }

                #endregion
            }

            return rtv;
        }

    }

}

using System;
using System.Collections.Generic;
using System.IO;

namespace Di3BMain
{
    /// <summary>
    /// Parse standard Browser Extensible Data (BED) format, and generates chromosome-wide statistics regarding regions length and p-values.
    /// </summary>
    internal class BED_Parser
    {
        /// <summary>
        /// Parse standard Browser Extensible Data (BED) format.
        /// </summary>
        /// <param name="source">Full path of source file name</param>
        /// <param name="species">This parameter will be used for initializing the chromosome count and sex chromosomes mappings.
        /// Values could be "Homo sapiens" or "Human" , or "Mus musculus" or "Mouse".</param>
        protected internal BED_Parser(
            string source,
            string species)
        {
            _source = source;
            _species = species;

            Initialize();
        }


        /// <summary>
        /// Parse standard Browser Extensible Data (BED) format.
        /// </summary>
        /// <param name="source">Full path of source file name</param>
        /// <param name="species">This parameter will be used for initializing the chromosome count and sex chromosomes mappings.
        /// Values could be "Homo sapiens" or "Human" , or "Mus musculus" or "Mouse".</param>
        /// <param name="start_offset">If the source file comes with header, the number of headers lines needs to be specified so that
        /// parser can ignore them. If not specified and header is present, header might be dropped because
        /// of improper format it might have. </param>
        /// <param name="chrom_Column">The coloumn number of chromosome name</param>
        /// <param name="peak_Start_Column">The column number of peak start position</param>
        /// <param name="peak_Stop_Column">The column number of peak stop position</param>
        /// <param name="peak_Name_Column">The column number of peak name</param>
        /// <param name="p_Value_Column">The column number of p-value</param>
        protected internal BED_Parser(
            string source,
            string species,
            byte start_offset,
            byte chrom_Column,
            byte peak_Start_Column,
            byte peak_Stop_Column,
            byte peak_Name_Column,
            byte p_Value_Column)
        {
            _source = source;
            _species = species;
            _start_offset = start_offset;
            _chrom_Column = chrom_Column;
            _peak_Start_Column = peak_Start_Column;
            _peak_Stop_Column = peak_Stop_Column;
            _peak_Name_Column = peak_Name_Column;
            _p_Value_Column = p_Value_Column;

            Initialize();
        }


        /// <summary>
        /// Parse standard Browser Extensible Data (BED) format.
        /// </summary>
        /// <param name="source">Full path of source file name</param>
        /// <param name="species">This parameter will be used for initializing the chromosome count and sex chromosomes mappings.
        /// Values could be "Homo sapiens" or "Human" , or "Mus musculus" or "Mouse".</param>
        /// <param name="start_offset">If the source file comes with header, the number of headers lines needs to be specified so that
        /// parser can ignore them. If not specified and header is present, header might be dropped because
        /// of improper format it might have. </param>
        /// <param name="chrom_Column">The coloumn number of chromosome name</param>
        /// <param name="peak_Start_Column">The column number of peak start position</param>
        /// <param name="peak_Stop_Column">The column number of peak stop position</param>
        /// <param name="peak_Name_Column">The column number of peak name</param>
        /// <param name="p_Value_Column">The column number of p-value</param>
        /// <param name="default_p_value">Will be used as region'left p-value if the 
        /// region in source file has an invalid p-value and 'drop_Peak_if_no_p_Value_is_given = false'</param>
        /// <param name="p_value_convertion_option">The p-value conversion will be based on this parameter.
        /// Set to '1' if the p-values are in (-10)Log10(p-value) format or
        /// set to '2' if the p-values are in (-1)Log10(p-value) format</param>
        /// <param name="drop_Peak_if_no_p_Value_is_given">If set to true, any peak that has invalid p-value will be droped. </param>
        protected internal BED_Parser(
            string source,
            string species,
            byte start_offset,
            byte chrom_Column,
            byte peak_Start_Column,
            byte peak_Stop_Column,
            byte peak_Name_Column,
            byte p_Value_Column,
            double default_p_value,
            char p_value_convertion_option,
            bool drop_Peak_if_no_p_Value_is_given)
        {
            _source = source;
            _species = species;
            _start_offset = start_offset;
            _chrom_Column = chrom_Column;
            _peak_Start_Column = peak_Start_Column;
            _peak_Stop_Column = peak_Stop_Column;
            _peak_Name_Column = peak_Name_Column;
            _p_Value_Column = p_Value_Column;
            _default_p_value = default_p_value;
            _p_value_convertion_option = p_value_convertion_option;
            _drop_Peak_if_no_p_Value_is_given = drop_Peak_if_no_p_Value_is_given;

            Initialize();
        }



        #region -_-         protected internal Variables declaration    -_-



        /// <summary>
        /// The chromosomes that observed in source BED file, while should not be present
        /// considering the chosen species. Note, the corresponding reads are not cached.
        /// </summary>
        protected internal List<string> excess__chromosomes = new List<string>();

        /// <summary>
        /// Sets the number of lines to be read from input file.
        /// The default value is 4,294,967,295 (0xFFFFFFFF) which will be used if not set. 
        /// </summary>
        protected internal UInt32 lines_to_be_read_count = UInt32.MaxValue;

        /// <summary>
        /// decimal places/precision of numbers when rounding them. It is used for 
        /// chromosome-wide stats estimation.
        /// </summary>
        protected internal byte decimal_places = 3;

        #endregion

        #region -_-         private Variables declaration               -_-

        /// <summary>
        /// Full path of source file name
        /// </summary>
        private string _source;

        /// <summary>
        /// This parameter will be used for initializing the chromosome count and sex chromosomes mappings.
        /// Values could be "Homo sapiens" or "Human" , or "Mus musculus" or "Mouse".
        /// </summary>
        private string _species = "Homo sapiens";

        /// <summary>
        /// If the source file comes with header, the number of headers lines needs to be specified so that
        /// parser can ignore them. If not specified and header is present, header might be dropped because
        /// of improper format it might have. 
        /// </summary>
        private byte _start_offset = 0;

        /// <summary>
        /// The coloumn number of chromosome name
        /// </summary>
        private byte _chrom_Column = 0;

        /// <summary>
        /// The column number of peak start position
        /// </summary>
        private byte _peak_Start_Column = 1;

        /// <summary>
        /// The column number of peak stop position
        /// </summary>
        private byte _peak_Stop_Column = 2;

        /// <summary>
        /// The column number of peak name
        /// </summary>
        private byte _peak_Name_Column = 3;

        /// <summary>
        /// The column number of p-value
        /// </summary>
        private byte _p_Value_Column = 4;

        /// <summary>
        /// Will be used as region'left p-value if the 
        /// region in source file has an invalid p-value and 'drop_Peak_if_no_p_Value_is_given = false'
        /// </summary>
        private double _default_p_value = 1E-8;

        /// <summary>
        /// The p-value conversion will be based on this parameter.
        /// Set to '1' if the p-values are in (-10)Log10(p-value) format or
        /// set to '2' if the p-values are in (-1)Log10(p-value) format
        /// </summary>
        private char _p_value_convertion_option = '1';

        /// <summary>
        /// If set to true, any peak that has invalid p-value will be droped. 
        /// </summary>
        private bool _drop_Peak_if_no_p_Value_is_given = true;

        /// <summary>
        /// It needs to be set to true if an unsuccessful attemp to read a region is occured, 
        /// such as invalid chromosome number. If the value is true, the reading peak will 
        /// not be stored. 
        /// </summary>
        private bool drop_Line = false;

        /// <summary>
        /// The chromosome count of the species, that is initialized based on the chosen species. 
        /// </summary>
        private byte chr_count;

        /// <summary>
        /// Holds catched information of each chromosome'left base pairs count. 
        /// This information will be updated based on the selected species.
        /// </summary>
        private int[] base_pairs_count;

        /// <summary>
        /// Mapping from chromosome Title (right.g., chrX, chrY, chrM) to chromosome Number (right.g., 23, 24, 25). 
        /// This parameter is initialized based on the selected species.
        /// Index 0 specifies chrX, index 1 specifies chrY, and index 2 specifies chrM mappings.
        /// </summary>
        private byte[] chromosome_T_2_N = new byte[3];

        /// <summary>
        /// Contains all read information from the input BED file, and 
        /// will be retured as read process result.
        /// </summary>
        private List<List<Peak>> data = new List<List<Peak>>();

        #endregion

        private void Initialize()
        {
            switch (_species.ToLower().Trim())
            {
                case "human":
                case "homo sapiens":
                    Initialize_Human_Data();
                    break;

                case "mouse":
                case "mus musculus":
                    Initialize_Mouse_Data();
                    break;
            }
        }

        private void Initialize_Human_Data()
        {
            // Assembly : GRCh37.p13
            // Gencode version : GENCODE 19
            // hm19
            base_pairs_count = new int[]
            {249250621, // chr1
            243199373,  // chr2
            198022430,  // chr3
            191154276,  // chr4
            180915260,  // chr5
            171115067,  // chr6
            159138663,  // chr7
            146364022,  // chr8
            141213431,  // chr9
            135534747,  // chr10
            135006516,  // chr11
            133851895,  // chr12
            115169878,  // chr13
            107349540,  // chr14
            102531392,  // chr15
            90354753,   // chr16
            81195210,   // chr17
            78077248,   // chr18
            59128983,   // chr19
            63025520,   // chr20
            48129895,   // chr21
            51304566,   // chr22
            155270560,  // chrX
            59373566,   // chrY
            16569};     // chrMT

            chr_count = 25;

            Initialize_Data_Structures();

            chromosome_T_2_N[0] = 22; // chrX
            chromosome_T_2_N[1] = 23; // chrY
            chromosome_T_2_N[2] = 24; // chrM
        }

        private void Initialize_Mouse_Data()
        {
            // Assembly : GRCm38.p2
            // Gencode version : GENCODE M2
            // mm10
            base_pairs_count = new int[]
            {195471971, // chr1
            182113224 , // chr2
            160039680 , // chr3
            156508116 , // chr4
            151834684 , // chr5
            149736546 , // chr6
            145441459 , // chr7
            129401213 , // chr8
            124595110 , // chr9
            130694993 , // chr10
            122082543 , // chr11
            120129022 , // chr12
            120421639 , // chr13
            124902244 , // chr14
            104043685 , // chr15
            98207768 ,  // chr16
            94987271 ,  // chr17
            90702639 ,  // chr18
            90702639 ,  // chr19
            61431566 ,  // chr20
            171031299 , // chrX
            91744698 ,  // chrY
            16299};     // chrM            

            chr_count = 22;

            Initialize_Data_Structures();

            chromosome_T_2_N[0] = 19; // chrX
            chromosome_T_2_N[1] = 20; // chrY
            chromosome_T_2_N[2] = 21; // chrM
        }

        /// <summary>
        /// Based on the chromosome count, initializes "Chromosomes" and statistics data structures
        /// </summary>
        private void Initialize_Data_Structures()
        {
            for (byte i = 0; i < chr_count; i++)
            {
                data.Add(new List<Peak>());
            }
        }

        /// <summary>
        /// Reads the regions presented in source file and generates chromosome-wide statistics regarding regions length and p-values. 
        /// </summary>
        /// <returns>Returns an object of Input_BED_Data class</returns>
        protected internal List<List<Peak>> Parse()
        {
            drop_Line = false;

            byte chr_No = 0;

            int
                start = 0,
                stop = 0;

            UInt32 line_counter = 0;

            double
                p_value = 0.0;

            List<string> line_drop_msg = new List<string>();

            string line;

            if (File.Exists(_source))
            {
                FileInfo file_info = new FileInfo(_source);
                long file_size = file_info.Length;
                int line_size = 0;

                using (StreamReader file_Reader = new StreamReader(_source))
                {
                    for (int i = 0; i < _start_offset; i++)
                    {
                        line = file_Reader.ReadLine();
                        line_counter++;
                    }

                    while ((line = file_Reader.ReadLine()) != null)
                    {
                        line_counter++;

                        line_size += file_Reader.CurrentEncoding.GetByteCount(line);

                        if (line.Trim().Length > 0 && line_counter <= lines_to_be_read_count)
                        {
                            Peak reading_Peak = new Peak();

                            drop_Line = false;

                            string[] splitted_Line = line.Split('\t');

                            #region -_-     Process Start/Stop      -_-

                            if (_peak_Start_Column < splitted_Line.Length && _peak_Stop_Column < splitted_Line.Length)
                            {
                                if (int.TryParse(splitted_Line[_peak_Start_Column], out start) &&
                                    int.TryParse(splitted_Line[_peak_Stop_Column], out stop))
                                {
                                    reading_Peak.start = start;
                                    reading_Peak.stop = stop;
                                }
                                else
                                {
                                    drop_Line = true;
                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid Start\\Stop column number");
                                }
                            }
                            else
                            {
                                drop_Line = true;
                                line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid Start\\Stop column number");
                            }

                            #endregion

                            #region -_-     Process p-value         -_-

                            if (_p_Value_Column < splitted_Line.Length)
                            {
                                if (double.TryParse(splitted_Line[_p_Value_Column], out p_value))
                                {
                                    reading_Peak.p_value = p_value_convertor(p_value);
                                }
                                else if (_drop_Peak_if_no_p_Value_is_given == true)
                                {
                                    drop_Line = true;
                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid p-value ( " + splitted_Line[_p_Value_Column] + " )");
                                }
                                else
                                {
                                    reading_Peak.p_value = _default_p_value;
                                }
                            }
                            else
                            {
                                if (_drop_Peak_if_no_p_Value_is_given == true)
                                {
                                    drop_Line = true;
                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid p-value column number");
                                }
                                else
                                {
                                    reading_Peak.p_value = _default_p_value;
                                }
                            }

                            #endregion

                            #region -_-     Process Peak Name       -_-

                            if (_peak_Name_Column < splitted_Line.Length)
                            {
                                reading_Peak.name = splitted_Line[_peak_Name_Column];
                            }
                            else
                            {
                                reading_Peak.name = "null";
                            }

                            #endregion

                            #region -_-     Get chromosome index    -_-

                            if (_chrom_Column < splitted_Line.Length)
                            {
                                string[] splitted_chromosome_number = splitted_Line[_chrom_Column].Split('r');

                                if (splitted_chromosome_number.Length == 2 && splitted_chromosome_number[0] == "ch")
                                {
                                    switch (splitted_chromosome_number[1])
                                    {
                                        case "X":
                                        case "x":
                                            chr_No = chromosome_T_2_N[0];
                                            break;

                                        case "Y":
                                        case "y":
                                            chr_No = chromosome_T_2_N[1];
                                            break;

                                        case "M":
                                        case "m":
                                            chr_No = chromosome_T_2_N[2];
                                            break;

                                        default:
                                            if (byte.TryParse(splitted_chromosome_number[1], out chr_No))
                                            {
                                                chr_No--;

                                                // This condition is applied to avoid chromosome numbers more than chromosome count.
                                                // The number is subtracted by 3, to avoid any chromosome numbers that overlap with
                                                // chrX, chrY or chrM. For example, if species is chosen as human and chr22 is observed
                                                // it should be mapped to chrX
                                                if (chr_No >= chr_count - 3)
                                                {
                                                    drop_Line = true;
                                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid chromosome number ( " + splitted_Line[_chrom_Column].ToString() + " )");

                                                    if (excess__chromosomes.Find(x => x.Equals("chr" + splitted_chromosome_number[1])) == null)
                                                    {
                                                        excess__chromosomes.Add("chr" + splitted_chromosome_number[1]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                drop_Line = true;
                                                line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid chromosome number ( " + splitted_Line[_chrom_Column].ToString() + " )");
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    drop_Line = true;
                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid chromosome number ( " + splitted_Line[_chrom_Column].ToString() + " )");
                                }
                            }
                            else
                            {
                                drop_Line = true;
                                line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid chromosome number ( " + splitted_Line[_chrom_Column].ToString() + " )");
                            }
                            #endregion

                            if (!drop_Line)
                            {
                                data[chr_No].Add(reading_Peak);
                            }
                        }
                    }
                }
            }
            else
            {
                // throw an exception to inform that the file is not present
            }

            return data;
        }

        /// <summary>
        /// Converts the p-value presented in (-1)Log10 or (-10)Log10 to original format.
        /// The conversion is done based on conversion option which specifies whether the input is in (-1)Log10(p-value) or (-10)Log10(p-value) format.
        /// </summary>
        /// <param name="p_value">The p-value in (-1)Log10 or (-10)Log10 format</param>
        /// <returns>The coverted p-value if the converstion type is valid, otherwise it returns 0</returns>
        private double p_value_convertor(double p_value)
        {
            switch (_p_value_convertion_option)
            {
                case '1': return Math.Pow(10.0, p_value / (-10.0));

                case '2': return Math.Pow(10.0, p_value / (-1.0));

                default: return 0;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IInterval;
using ICPMD;


namespace BEDParser
{
    public class BEDParser<Peak, Metadata>
        where Peak : IInterval<int, Metadata>, new()
        where Metadata : ICPMetadata<int>//, IMetaData<int>
    {
        /// <summary>
        /// Parse standard Browser Extensible Data (BED) format.
        /// </summary>
        /// <param name="source">Full path of source file name/</param>
        /// <param name="species">This parameter will be used for initializing the chromosome count and sex chromosomes mappings.
        /// Values could be "Homo sapiens" or "Human" , or "Mus musculus" or "Mouse".</param>
        public BEDParser(
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
        /// <param name="source">Full path of source file name.</param>
        /// <param name="species">This parameter will be used for initializing the chromosome count and sex chromosomes mappings.
        /// Values could be "Homo sapiens" or "Human" , or "Mus musculus" or "Mouse".</param>
        /// <param name="startOffset">If the source file comes with header, the number of headers lines needs to be specified so that
        /// parser can ignore them. If not specified and header is present, header might be dropped because
        /// of improper format it might have. </param>
        /// <param name="chrColumn">The coloumn number of chromosome name.</param>
        /// <param name="leftColumn">The column number of peak left-end position.</param>
        /// <param name="rightColumn">The column number of peak right-end position.</param>
        /// <param name="nameColumn">The column number of peak name.</param>
        /// <param name="valueColumn">The column number of peak value.</param>
        /// /// <param name="strandColumn">The column number of peak strand. If input is not stranded this value should be set to -1.</param>
        protected internal BEDParser(
            string source,
            string species,
            byte startOffset,
            byte chrColumn,
            byte leftColumn,
            byte rightColumn,
            byte nameColumn,
            byte valueColumn,
            sbyte strandColumn)
        {
            _source = source;
            _species = species;
            _startOffset = startOffset;
            _chrColumn = chrColumn;
            _leftColumn = leftColumn;
            _rightColumn = rightColumn;
            _nameColumn = nameColumn;
            _valueColumn = valueColumn;
            _strandColumn = strandColumn;

            Initialize();
        }


        /// <summary>
        /// Parse standard Browser Extensible Data (BED) format.
        /// </summary>
        /// <param name="source">Full path of source file name.</param>
        /// <param name="species">This parameter will be used for initializing the chromosome count and sex chromosomes mappings.
        /// Values could be "Homo sapiens" or "Human" , or "Mus musculus" or "Mouse".</param>
        /// <param name="startOffset">If the source file comes with header, the number of headers lines needs to be specified so that
        /// parser can ignore them. If not specified and header is present, header might be dropped because
        /// of improper format it might have. </param>
        /// <param name="chrColumn">The coloumn number of chromosome name.</param>
        /// <param name="leftColumn">The column number of peak left-end position.</param>
        /// <param name="rightColumn">The column number of peak right-end position.</param>
        /// <param name="nameColumn">The column number of peak name.</param>
        /// <param name="valueColumn">The column number of peak value.</param>
        /// <param name="strandColumn">The column number of peak strand. If input is not stranded this value should be set to -1.</param>
        /// <param name="defaultValue">Default value of a peak. It will be used in case 
        /// invalid value is read from source.</param>
        /// <param name="valueConvertionOption">It specifies the value convertion option:
        /// <para>0 : no convertion.</para>
        /// <para>1 : value = (-10)Log10(value)</para>
        /// <para>2 : value =  (-1)Log10(value)</para>
        /// <param name="dropPeakIfInvalidValue">If set to true, a peak with invalid value will be droped. 
        /// If set to false, a peak with invalid value with take up the default value.</param>
        protected internal BEDParser(
            string source,
            string species,
            byte startOffset,
            byte chrColumn,
            byte leftColumn,
            byte rightColumn,
            byte nameColumn,
            byte valueColumn,
            sbyte strandColumn,
            double defaultValue,
            char valueConvertionOption,
            bool dropPeakIfInvalidValue)
        {
            _source = source;
            _species = species;
            _startOffset = startOffset;
            _chrColumn = chrColumn;
            _leftColumn = leftColumn;
            _rightColumn = rightColumn;
            _nameColumn = nameColumn;
            _valueColumn = valueColumn;
            _strandColumn = strandColumn;
            _defaultValue = defaultValue;
            _valueConvertionOption = valueConvertionOption;
            _dropPeakIfInvalidValue = dropPeakIfInvalidValue;

            Initialize();
        }


        #region .::.         Status variable and it's event controlers   .::.

        private string _status;
        public string status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnStatusValueChaned(value);
                }
            }
        }
        public event EventHandler<ValueEventArgs> Status_Changed;
        private void OnStatusValueChaned(string value)
        {
            if (Status_Changed != null)
                Status_Changed(this, new ValueEventArgs(value));
        }

        #endregion

        #region .::.         Public Variables declaration                .::.

        /// <summary>
        /// The chromosomes that no valid regions determined on them.
        /// </summary>
        public List<string> missingChrs = new List<string>();

        /// <summary>
        /// The chromosomes that observed in source BED file, while should not be present
        /// considering the chosen species. Note, the corresponding reads are not cached.
        /// </summary>
        public List<string> excessChrs = new List<string>();

        /// <summary>
        /// Sets the number of lines to be read from input file.
        /// The default value is 4,294,967,295 (0xFFFFFFFF) which will be used if not set. 
        /// </summary>
        public UInt32 maxLinesToBeRead = UInt32.MaxValue;

        /// <summary>
        /// decimal places/precision of numbers when rounding them. It is used for 
        /// chromosome-wide stats estimation.
        /// </summary>
        public byte decimalPlaces = 3;

        #endregion

        #region .::.         private Variables declaration               .::.

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
        private byte _startOffset = 0;

        /// <summary>
        /// The coloumn number of chromosome name
        /// </summary>
        private byte _chrColumn = 0;

        /// <summary>
        /// The column number of peak start position
        /// </summary>
        private byte _leftColumn = 1;

        /// <summary>
        /// The column number of peak stop position
        /// </summary>
        private byte _rightColumn = 2;

        /// <summary>
        /// The column number of peak name
        /// </summary>
        private byte _nameColumn = 3;

        /// <summary>
        /// The column number of p-value
        /// </summary>
        private byte _valueColumn = 4;

        /// <summary>
        /// The column number of strand.
        /// </summary>
        private sbyte _strandColumn = -1;

        /// <summary>
        /// Will be used as region's p-value if the 
        /// region in source file has an invalid p-value and 'drop_Peak_if_no_p_Value_is_given = false'
        /// </summary>
        private double _defaultValue = 1E-8;

        /// <summary>
        /// The p-value conversion will be based on this parameter.
        /// Set to '1' if the p-values are in (-10)Log10(p-value) format or
        /// set to '2' if the p-values are in (-1)Log10(p-value) format
        /// </summary>
        private char _valueConvertionOption = '1';

        /// <summary>
        /// If set to true, any peak that has invalid p-value will be droped. 
        /// </summary>
        private bool _dropPeakIfInvalidValue = true;

        /// <summary>
        /// It needs to be set to true if an unsuccessful attemp to read a region is occured, 
        /// such as invalid chromosome number. If the value is true, the reading peak will 
        /// not be stored. 
        /// </summary>
        private bool _dropLine = false;

        /// <summary>
        /// The chromosome count of the species, that is initialized based on the chosen species. 
        /// </summary>
        private byte _chrCount;

        /// <summary>
        /// Holds catched information of each chromosome's base pairs count. 
        /// This information will be updated based on the selected species.
        /// </summary>
        private int[] _basePairsCount;

        /// <summary>
        /// Mapping from chromosome Title (e.g., chrX, chrY, chrM) to chromosome Number (e.g., 23, 24, 25). 
        /// This parameter is initialized based on the selected species.
        /// Index 0 specifies chrX, index 1 specifies chrY, and index 2 specifies chrM mappings.
        /// </summary>
        private byte[] _chrT2N = new byte[3];

        /// <summary>
        /// When read process is finished, this variable contains the number
        /// of dropped regions.
        /// </summary>
        private UInt16 _dropedLinesCount = 0;

        /// <summary>
        /// When read process is finished, this variable contains the number
        /// of regions that contained invalid p-value and replaced by default p-value. 
        /// </summary>
        private UInt16 _defaultValueUtilizationCount = 0;

        /// <summary>
        /// Contains all read information from the input BED file, and 
        /// will be retured as read process result.
        /// </summary>
        private ParsedBED<Peak, Metadata> _data = new ParsedBED<Peak, Metadata>();

        /// <summary>
        /// Contains chromosome wide information and statistics of the sample
        /// </summary>
        private List<ChrStatistics> _Chrs = new List<ChrStatistics>();

        /// <summary>
        /// Holds the width sum of all read peaks chromosome wide.
        /// </summary>
        private List<uint> _pw__Sum = new List<uint>();

        /// <summary>
        /// Holds the width mean of all read peaks chromosome wide.
        /// </summary>
        private List<double> _pw_Mean = new List<double>();

        /// <summary>
        /// Holds the width standard deviation of all read peaks chromosome wide.
        /// </summary>
        private List<double> _pw_STDV = new List<double>();

        /// <summary>
        /// Holds the p-value sum of all read peaks chromosome wide.
        /// </summary>
        private List<double> _pV__Sum = new List<double>();

        /// <summary>
        /// Holds the p-value mean of all read peaks chromosome wide.
        /// </summary>
        private List<double> _pV_Mean = new List<double>();

        /// <summary>
        /// Hold the p-value standard deviation of all read peaks chromosome wide
        /// </summary>
        private List<double> _pV_STDV = new List<double>();

        #endregion



        private void Initialize()
        {
            _data.filePath = System.IO.Path.GetFullPath(_source);
            _data.fileName = System.IO.Path.GetFileName(_source);

            switch (_species.ToLower().Trim())
            {
                case "human":
                case "homo sapiens":
                    InitializeHumanData();
                    break;

                case "mouse":
                case "mus musculus":
                    InitializeMouseData();
                    break;
            }
        }

        private void InitializeHumanData()
        {
            // Assembly : GRCh37.p13
            // Gencode version : GENCODE 19
            // hm19
            _basePairsCount = new int[]
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

            _chrCount = 25;

            InitializeDataStructures();

            _chrT2N[0] = 22; // chrX
            _chrT2N[1] = 23; // chrY
            _chrT2N[2] = 24; // chrM

            _Chrs[22].chrTitle = "chrX";
            _Chrs[23].chrTitle = "chrY";
            _Chrs[24].chrTitle = "chrM";

            _data.species = "Homo sapiens";
        }

        private void InitializeMouseData()
        {
            // Assembly : GRCm38.p2
            // Gencode version : GENCODE M2
            // mm10
            _basePairsCount = new int[]
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

            _chrCount = 22;

            InitializeDataStructures();

            _chrT2N[0] = 19; // chrX
            _chrT2N[1] = 20; // chrY
            _chrT2N[2] = 21; // chrM

            _Chrs[19].chrTitle = "chrX";
            _Chrs[20].chrTitle = "chrY";
            _Chrs[21].chrTitle = "chrM";

            _data.species = "Mus musculus";
        }

        /// <summary>
        /// Based on the chromosome count, initializes "_Chrs" and statistics _data structures
        /// </summary>
        private void InitializeDataStructures()
        {
            for (byte i = 0; i < _chrCount; i++)
            {
                _Chrs.Add(new ChrStatistics()
                {
                    chrTitle = "chr" + (i + 1).ToString(),
                    count = 0,
                    percentage = "0 %",

                    pValueMax = 0,
                    pValueMin = 1,
                    pValueMean = 0,
                    pValueSTDV = 0,

                    peakWidthMax = 0,
                    peakWidthMin = uint.MaxValue,
                    peakWidthMean = 0,
                    peakWidth_STDV = 0,

                    coverage = 0,
                });

                _data.peaks.Add(new List<Peak>());

                _pw__Sum.Add(0);
                _pw_Mean.Add(0.0);
                _pw_STDV.Add(0.0);

                _pV__Sum.Add(0.0);
                _pV_Mean.Add(0.0);
                _pV_STDV.Add(0.0);
            }
        }

        /// <summary>
        /// Reads the regions presented in source file and generates chromosome-wide statistics regarding regions length and values. 
        /// </summary>
        /// <returns>Returns an object of Input_BED_Data class</returns>
        public ParsedBED<Peak, Metadata> Parse()
        {
            _dropLine = false;

            byte chr_No = 0;

            int
                start = 0,
                stop = 0;

            UInt32 line_counter = 0;

            double p_value = 0.0;

            char strand = '*';

            List<string> line_drop_msg = new List<string>();

            string line;

            if (File.Exists(_source))
            {
                FileInfo file_info = new FileInfo(_source);
                long file_size = file_info.Length;
                int line_size = 0;

                using (StreamReader file_Reader = new StreamReader(_source))
                {
                    for (int i = 0; i < _startOffset; i++)
                    {
                        line = file_Reader.ReadLine();
                        line_counter++;
                    }

                    while ((line = file_Reader.ReadLine()) != null)
                    {
                        line_counter++;

                        line_size += file_Reader.CurrentEncoding.GetByteCount(line);
                        status = (Math.Round((line_size * 100.0) / file_size, 0)).ToString();

                        if (line.Trim().Length > 0 && line_counter <= maxLinesToBeRead)
                        {
                            Peak reading_Peak = new Peak();

                            _dropLine = false;

                            string[] splitted_Line = line.Split('\t');

                            #region -_-     Process Start/Stop      -_-

                            if (_leftColumn < splitted_Line.Length && _rightColumn < splitted_Line.Length)
                            {
                                if (int.TryParse(splitted_Line[_leftColumn], out start) &&
                                    int.TryParse(splitted_Line[_rightColumn], out stop))
                                {
                                    reading_Peak.left = start;
                                    reading_Peak.right = stop;
                                    reading_Peak.metadata.left = start;
                                    reading_Peak.metadata.right = stop;
                                }
                                else
                                {
                                    _dropLine = true;
                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid Start\\Stop column number");
                                }
                            }
                            else
                            {
                                _dropLine = true;
                                line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid Start\\Stop column number");
                            }

                            #endregion

                            #region -_-     Process p-value         -_-

                            if (_valueColumn < splitted_Line.Length)
                            {
                                if (double.TryParse(splitted_Line[_valueColumn], out p_value))
                                {
                                    reading_Peak.metadata.value = pValueConvertor(p_value);
                                }
                                else if (_dropPeakIfInvalidValue == true)
                                {
                                    _dropLine = true;
                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid p-value ( " + splitted_Line[_valueColumn] + " )");
                                }
                                else
                                {
                                    reading_Peak.metadata.value = _defaultValue;
                                    _defaultValueUtilizationCount++;
                                }
                            }
                            else
                            {
                                if (_dropPeakIfInvalidValue == true)
                                {
                                    _dropLine = true;
                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid p-value column number");
                                }
                                else
                                {
                                    reading_Peak.metadata.value = _defaultValue;
                                    _defaultValueUtilizationCount++;
                                }
                            }

                            #endregion

                            #region -_-     Process Peak Name       -_-

                            if (_nameColumn < splitted_Line.Length)
                            {
                                reading_Peak.metadata.name = splitted_Line[_nameColumn];
                            }
                            else
                            {
                                reading_Peak.metadata.name = "null";
                            }

                            #endregion

                            #region -_-     Process Peak Strand     -_-

                            if (_strandColumn != -1 && _strandColumn < splitted_Line.Length)
                            {
                                if (char.TryParse(splitted_Line[_strandColumn], out strand))
                                {
                                    reading_Peak.metadata.strand = strand;
                                }
                                else
                                {
                                    reading_Peak.metadata.strand = '*';
                                }
                            }
                            else
                            {
                                reading_Peak.metadata.strand = '*';
                            }

                            #endregion

                            #region -_-     Get chromosome index    -_-

                            if (_chrColumn < splitted_Line.Length)
                            {
                                string[] splitted_chromosome_number = splitted_Line[_chrColumn].Split('r');

                                if (splitted_chromosome_number.Length == 2 && splitted_chromosome_number[0] == "ch")
                                {
                                    switch (splitted_chromosome_number[1])
                                    {
                                        case "X":
                                        case "x":
                                            chr_No = _chrT2N[0];
                                            break;

                                        case "Y":
                                        case "y":
                                            chr_No = _chrT2N[1];
                                            break;

                                        case "M":
                                        case "m":
                                            chr_No = _chrT2N[2];
                                            break;

                                        default:
                                            if (byte.TryParse(splitted_chromosome_number[1], out chr_No))
                                            {
                                                chr_No--;

                                                // This condition is applied to avoid chromosome numbers more than chromosome count.
                                                // The number is subtracted by 3, to avoid any chromosome numbers that overlap with
                                                // chrX, chrY or chrM. For example, if species is chosen as human and chr22 is observed
                                                // it should be mapped to chrX
                                                if (chr_No >= _chrCount - 3)
                                                {
                                                    _dropLine = true;
                                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid chromosome number ( " + splitted_Line[_chrColumn].ToString() + " )");

                                                    if (excessChrs.Find(x => x.Equals("chr" + splitted_chromosome_number[1])) == null)
                                                    {
                                                        excessChrs.Add("chr" + splitted_chromosome_number[1]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                _dropLine = true;
                                                line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid chromosome number ( " + splitted_Line[_chrColumn].ToString() + " )");
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    _dropLine = true;
                                    line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid chromosome number ( " + splitted_Line[_chrColumn].ToString() + " )");
                                }
                            }
                            else
                            {
                                _dropLine = true;
                                line_drop_msg.Add("\tLine " + line_counter.ToString() + "\t:\tInvalid chromosome number ( " + splitted_Line[_chrColumn].ToString() + " )");
                            }
                            #endregion

                            if (!_dropLine)
                            {
                                _data.peaks[chr_No].Add(reading_Peak);

                                _Chrs[chr_No].count++;

                                #region -_-     Check for Widest and Narrowest Peaks    -_-

                                if (reading_Peak.right - reading_Peak.left > _Chrs[chr_No].peakWidthMax)
                                {
                                    _Chrs[chr_No].peakWidthMax = (uint)(reading_Peak.right - reading_Peak.left);
                                }

                                if (reading_Peak.right - reading_Peak.left < _Chrs[chr_No].peakWidthMin)
                                {
                                    _Chrs[chr_No].peakWidthMin = (uint)(reading_Peak.right - reading_Peak.left);
                                }

                                #endregion

                                #region -_-     Check for Max and Min of p-value        -_-

                                if (reading_Peak.metadata.value > _Chrs[chr_No].pValueMax)
                                {
                                    _Chrs[chr_No].pValueMax = reading_Peak.metadata.value;
                                }

                                if (reading_Peak.metadata.value < _Chrs[chr_No].pValueMin)
                                {
                                    _Chrs[chr_No].pValueMin = reading_Peak.metadata.value;
                                }

                                #endregion

                                #region -_-     Maximum and Minimum p-value             -_-

                                if (reading_Peak.metadata.value > _data.pValueMax.metadata.value)
                                {
                                    _data.pValueMax = reading_Peak;
                                }

                                if (reading_Peak.metadata.value < _data.pValueMin.metadata.value)
                                {
                                    _data.pValueMin = reading_Peak;
                                }

                                #endregion

                                _pw__Sum[chr_No] = (uint)(_pw__Sum[chr_No] + (reading_Peak.right - reading_Peak.left));

                                _pV__Sum[chr_No] += reading_Peak.metadata.value;
                            }
                            else
                            {
                                _dropedLinesCount++;
                            }
                        }
                    }
                }

                #region -_-                Dropped Lines                -_-

                if (_defaultValueUtilizationCount > 0)
                    line_drop_msg.Insert(0, "\tDefault p-value used for " + _defaultValueUtilizationCount.ToString() + " times");

                if (_dropedLinesCount > 0)
                    line_drop_msg.Insert(0, "\t" + _dropedLinesCount.ToString() + " Lines droped");

                _data.messages = line_drop_msg;
                #endregion

                _data.chrCount = _chrCount;

                EstimateStatistics();
            }
            else
            {
                // throw an exception to inform that the file is not present
            }

            status = "100";

            return _data;
        }

        private void EstimateStatistics()
        {
            uint total_Peak_count = 0;

            #region -_-     chromosome Statistics preparation       -_-

            for (int i = 0; i < _Chrs.Count; i++)
            {
                total_Peak_count += (uint)_Chrs[i].count;

                if ((uint)_Chrs[i].count != 0)
                {
                    _pw_Mean[i] = Math.Round(_pw__Sum[i] / (double)_Chrs[i].count, decimalPlaces);
                    _pV_Mean[i] = _pV__Sum[i] / (double)_Chrs[i].count;
                }
                else
                {
                    _pw_Mean[i] = 0;
                    _pV_Mean[i] = 0;
                }
            }

            for (int i = 0; i < _Chrs.Count; i++)
            {
                foreach (Peak P in _data.peaks[i])
                {
                    _pw_STDV[i] += Math.Pow((P.right - P.left) - _pw_Mean[i], 2.0);
                    _pV_STDV[i] += Math.Pow(P.metadata.value - _pV_Mean[i], 2.0);
                }
            }

            for (int i = 0; i < _Chrs.Count; i++)
            {
                if ((double)_Chrs[i].count != 0)
                {
                    _pw_STDV[i] = Math.Sqrt(_pw_STDV[i] / (double)_Chrs[i].count);
                    _pV_STDV[i] = Math.Sqrt(_pV_STDV[i] / (double)_Chrs[i].count);
                }
                else
                {
                    _pw_STDV[i] = 0;
                    _pV_STDV[i] = 0;
                }
            }

            #endregion

            if (total_Peak_count > 0)
            {
                double p_Values_Sum = 0;

                #region -_-        Store chromosome wide statistics         -_-

                for (byte i = 0; i < _Chrs.Count; i++)
                {
                    if (_Chrs[i].peakWidthMin == uint.MaxValue) _Chrs[i].peakWidthMin = 0;

                    if (_Chrs[i].pValueMin == 1) _Chrs[i].pValueMin = 0;

                    _data.chrStatistics.Add(new ChrStatistics()
                    {
                        chrTitle = _Chrs[i].chrTitle,
                        count = _Chrs[i].count,
                        percentage = (Math.Round((double)((_Chrs[i].count * 100) / total_Peak_count), decimalPlaces)).ToString() + " %",

                        peakWidthMax = _Chrs[i].peakWidthMax,
                        peakWidthMin = _Chrs[i].peakWidthMin,
                        peakWidthMean = Math.Round((float)_pw_Mean[i], decimalPlaces),
                        peakWidth_STDV = Math.Round((float)_pw_STDV[i], decimalPlaces),


                        pValueMax = _Chrs[i].pValueMax,
                        pValueMin = _Chrs[i].pValueMin,
                        pValueMean = _pV_Mean[i],
                        pValueSTDV = _pV_STDV[i],

                        coverage = (float)Math.Round(((int)_pw__Sum[i] * 100.0) / _basePairsCount[i], decimalPlaces)
                    });

                    if (!double.IsNaN(_pV_Mean[i]))
                    {
                        p_Values_Sum += (double)_pV_Mean[i];
                    }

                    if (i != _chrCount - 1 && i != _chrCount - 2 && _Chrs[i].count == 0)
                    {
                        missingChrs.Add(_Chrs[i].chrTitle);
                    }
                }

                #endregion

                _data.pValueMean = p_Values_Sum / total_Peak_count;
                _data.peaksCount = Convert.ToInt32(total_Peak_count);
            }
        }

        /// <summary>
        /// Converts the p-value presented in (-1)Log10 or (-10)Log10 to original format.
        /// The conversion is done based on conversion option which specifies whether the input is in (-1)Log10(p-value) or (-10)Log10(p-value) format.
        /// </summary>
        /// <param name="value">The p-value in (-1)Log10 or (-10)Log10 format</param>
        /// <returns>The coverted p-value if the converstion type is valid, otherwise it returns 0</returns>
        private double pValueConvertor(double value)
        {
            switch (_valueConvertionOption)
            {
                case '0': return value;
                case '1': return Math.Pow(10.0, value / (-10.0));
                case '2': return Math.Pow(10.0, value / (-1.0));
                default: return 0;
            }
        }
    }
}

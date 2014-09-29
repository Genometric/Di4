using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IInterval;
using ICPMD;
using System.Text.RegularExpressions;


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
        private Dictionary<string, int> _basePairsCount;

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
        private Dictionary<string, ChrStatistics> _Chrs = new Dictionary<string, ChrStatistics>();

        /// <summary>
        /// Holds the width sum of all read peaks chromosome wide.
        /// </summary>
        private Dictionary<string, uint> _pw__Sum = new Dictionary<string, uint>();

        /// <summary>
        /// Holds the width mean of all read peaks chromosome wide.
        /// </summary>
        private Dictionary<string, double> _pw_Mean = new Dictionary<string, double>();

        /// <summary>
        /// Holds the width standard deviation of all read peaks chromosome wide.
        /// </summary>
        private Dictionary<string, double> _pw_STDV = new Dictionary<string, double>();

        /// <summary>
        /// Holds the p-value sum of all read peaks chromosome wide.
        /// </summary>
        private Dictionary<string, double> _pV__Sum = new Dictionary<string, double>();

        /// <summary>
        /// Holds the p-value mean of all read peaks chromosome wide.
        /// </summary>
        private Dictionary<string, double> _pV_Mean = new Dictionary<string, double>();

        /// <summary>
        /// Hold the p-value standard deviation of all read peaks chromosome wide
        /// </summary>
        private Dictionary<string, double> _pV_STDV = new Dictionary<string, double>();

        #endregion



        private void Initialize()
        {
            _data.filePath = System.IO.Path.GetFullPath(_source);
            _data.fileName = System.IO.Path.GetFileName(_source);
            _data.fileHashKey = GetFileHashKey(_data.filePath);
            _basePairsCount = new Dictionary<string, int>();

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
            _basePairsCount.Add("chr1",249250621);
            _basePairsCount.Add("chr2", 243199373);
            _basePairsCount.Add("chr3", 198022430);
            _basePairsCount.Add("chr4", 191154276);
            _basePairsCount.Add("chr5", 180915260);
            _basePairsCount.Add("chr6", 171115067);
            _basePairsCount.Add("chr7", 159138663);
            _basePairsCount.Add("chr8", 146364022);
            _basePairsCount.Add("chr9", 141213431);
            _basePairsCount.Add("chr10", 135534747);
            _basePairsCount.Add("chr11", 135006516);
            _basePairsCount.Add("chr12", 133851895);
            _basePairsCount.Add("chr13", 115169878);
            _basePairsCount.Add("chr14", 107349540);
            _basePairsCount.Add("chr15", 102531392);
            _basePairsCount.Add("chr16", 90354753);
            _basePairsCount.Add("chr17", 81195210);
            _basePairsCount.Add("chr18", 78077248);
            _basePairsCount.Add("chr19", 59128983);
            _basePairsCount.Add("chr20", 63025520);
            _basePairsCount.Add("chr21", 48129895);
            _basePairsCount.Add("chr22", 51304566);
            _basePairsCount.Add("chrX", 155270560);
            _basePairsCount.Add("chrY", 59373566);
            _basePairsCount.Add("chrM", 16569);

            _data.species = "Homo sapiens";
        }

        private void InitializeMouseData()
        {
            // Assembly : GRCm38.p2
            // Gencode version : GENCODE M2
            // mm10            
            _basePairsCount.Add("chr1",195471971);
            _basePairsCount.Add("chr2",182113224);
            _basePairsCount.Add("chr3",160039680);
            _basePairsCount.Add("chr4",156508116);
            _basePairsCount.Add("chr5",151834684);
            _basePairsCount.Add("chr6",149736546);
            _basePairsCount.Add("chr7",145441459);
            _basePairsCount.Add("chr8",129401213);
            _basePairsCount.Add("chr9",124595110);
            _basePairsCount.Add("chr10",130694993);
            _basePairsCount.Add("chr11",122082543);
            _basePairsCount.Add("chr12",120129022);
            _basePairsCount.Add("chr13",120421639);
            _basePairsCount.Add("chr14",124902244);
            _basePairsCount.Add("chr15",104043685);
            _basePairsCount.Add("chr16",98207768);
            _basePairsCount.Add("chr17",94987271);
            _basePairsCount.Add("chr18",90702639);
            _basePairsCount.Add("chr19",90702639);
            _basePairsCount.Add("chr20",61431566);
            _basePairsCount.Add("chrX",171031299);
            _basePairsCount.Add("chrY", 91744698);
            _basePairsCount.Add("chrM", 16299);

            _data.species = "Mus musculus";
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

            UInt32 lineCounter = 0;

            double p_value = 0.0;

            char strand = '*';

            List<string> lineDropMsg = new List<string>();

            string line;

            if (File.Exists(_source))
            {
                FileInfo file_info = new FileInfo(_source);
                long file_size = file_info.Length;
                int line_size = 0;
                string chrTitle = "";

                using (StreamReader fileReader = new StreamReader(_source))
                {
                    for (int i = 0; i < _startOffset; i++)
                    {
                        line = fileReader.ReadLine();
                        lineCounter++;
                    }

                    while ((line = fileReader.ReadLine()) != null)
                    {
                        lineCounter++;

                        line_size += fileReader.CurrentEncoding.GetByteCount(line);
                        status = (Math.Round((line_size * 100.0) / file_size, 0)).ToString();

                        if (line.Trim().Length > 0 && lineCounter <= maxLinesToBeRead)
                        {
                            Peak readingPeak = new Peak();

                            _dropLine = false;

                            string[] splittedLine = line.Split('\t');

                            #region -_-     Process Start/Stop      -_-

                            if (_leftColumn < splittedLine.Length && _rightColumn < splittedLine.Length)
                            {
                                if (int.TryParse(splittedLine[_leftColumn], out start) &&
                                    int.TryParse(splittedLine[_rightColumn], out stop))
                                {
                                    readingPeak.left = start;
                                    readingPeak.right = stop;
                                    readingPeak.metadata.left = start;
                                    readingPeak.metadata.right = stop;
                                }
                                else
                                {
                                    _dropLine = true;
                                    lineDropMsg.Add("\tLine " + lineCounter.ToString() + "\t:\tInvalid Start\\Stop column number");
                                }
                            }
                            else
                            {
                                _dropLine = true;
                                lineDropMsg.Add("\tLine " + lineCounter.ToString() + "\t:\tInvalid Start\\Stop column number");
                            }

                            #endregion

                            #region -_-     Process p-value         -_-

                            if (_valueColumn < splittedLine.Length)
                            {
                                if (double.TryParse(splittedLine[_valueColumn], out p_value))
                                {
                                    readingPeak.metadata.value = pValueConvertor(p_value);
                                }
                                else if (_dropPeakIfInvalidValue == true)
                                {
                                    _dropLine = true;
                                    lineDropMsg.Add("\tLine " + lineCounter.ToString() + "\t:\tInvalid p-value ( " + splittedLine[_valueColumn] + " )");
                                }
                                else
                                {
                                    readingPeak.metadata.value = _defaultValue;
                                    _defaultValueUtilizationCount++;
                                }
                            }
                            else
                            {
                                if (_dropPeakIfInvalidValue == true)
                                {
                                    _dropLine = true;
                                    lineDropMsg.Add("\tLine " + lineCounter.ToString() + "\t:\tInvalid p-value column number");
                                }
                                else
                                {
                                    readingPeak.metadata.value = _defaultValue;
                                    _defaultValueUtilizationCount++;
                                }
                            }

                            #endregion

                            #region -_-     Process Peak Name       -_-

                            if (_nameColumn < splittedLine.Length)
                            {
                                readingPeak.metadata.name = splittedLine[_nameColumn];
                            }
                            else
                            {
                                readingPeak.metadata.name = "null";
                            }

                            #endregion

                            #region -_-     Process Peak Strand     -_-

                            if (_strandColumn != -1 && _strandColumn < splittedLine.Length)
                            {
                                if (char.TryParse(splittedLine[_strandColumn], out strand))
                                {
                                    readingPeak.metadata.strand = strand;
                                }
                                else
                                {
                                    readingPeak.metadata.strand = '*';
                                }
                            }
                            else
                            {
                                readingPeak.metadata.strand = '*';
                            }

                            #endregion

                            #region -_-     Get chromosome index    -_-

                            if (_chrColumn < splittedLine.Length &&
                                Regex.IsMatch(splittedLine[_chrColumn].ToLower(), "chr"))
                            {
                                chrTitle = splittedLine[_chrColumn].ToLower();
                            }
                            else
                            {
                                _dropLine = true;
                                lineDropMsg.Add("\tLine " + lineCounter.ToString() + "\t:\tInvalid chromosome number ( " + splittedLine[_chrColumn].ToString() + " )");
                            }
                            #endregion

                            if (!_dropLine)
                            {
                                if (!_data.peaks.ContainsKey(chrTitle))
                                    AddNewChromosome(chrTitle);

                                _data.peaks[chrTitle].Add(readingPeak);

                                _Chrs[chrTitle].count++;

                                #region -_-     Check for Widest and Narrowest Peaks    -_-

                                if (readingPeak.right - readingPeak.left > _Chrs[chrTitle].peakWidthMax)
                                {
                                    _Chrs[chrTitle].peakWidthMax = (uint)(readingPeak.right - readingPeak.left);
                                }

                                if (readingPeak.right - readingPeak.left < _Chrs[chrTitle].peakWidthMin)
                                {
                                    _Chrs[chrTitle].peakWidthMin = (uint)(readingPeak.right - readingPeak.left);
                                }

                                #endregion

                                #region -_-     Check for Max and Min of p-value        -_-

                                if (readingPeak.metadata.value > _Chrs[chrTitle].pValueMax)
                                {
                                    _Chrs[chrTitle].pValueMax = readingPeak.metadata.value;
                                }

                                if (readingPeak.metadata.value < _Chrs[chrTitle].pValueMin)
                                {
                                    _Chrs[chrTitle].pValueMin = readingPeak.metadata.value;
                                }

                                #endregion

                                #region -_-     Maximum and Minimum p-value             -_-

                                if (readingPeak.metadata.value > _data.pValueMax.metadata.value)
                                {
                                    _data.pValueMax = readingPeak;
                                }

                                if (readingPeak.metadata.value < _data.pValueMin.metadata.value)
                                {
                                    _data.pValueMin = readingPeak;
                                }

                                #endregion

                                _pw__Sum[chrTitle] = (uint)(_pw__Sum[chrTitle] + (readingPeak.right - readingPeak.left));

                                _pV__Sum[chrTitle] += readingPeak.metadata.value;
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
                    lineDropMsg.Insert(0, "\tDefault p-value used for " + _defaultValueUtilizationCount.ToString() + " times");

                if (_dropedLinesCount > 0)
                    lineDropMsg.Insert(0, "\t" + _dropedLinesCount.ToString() + " Lines droped");

                _data.messages = lineDropMsg;
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

            foreach (KeyValuePair<string, ChrStatistics> entry in _Chrs)
            {
                total_Peak_count += (uint)entry.Value.count;

                if ((uint)entry.Value.count != 0)
                {
                    _pw_Mean[entry.Key] = Math.Round(_pw__Sum[entry.Key] / (double)_Chrs[entry.Key].count, decimalPlaces);
                    _pV_Mean[entry.Key] = _pV__Sum[entry.Key] / (double)_Chrs[entry.Key].count;
                }
                else
                {
                    _pw_Mean[entry.Key] = 0;
                    _pV_Mean[entry.Key] = 0;
                }
            }

            foreach (KeyValuePair<string, ChrStatistics> entry in _Chrs)
            {
                foreach (Peak P in _data.peaks[entry.Key])
                {
                    _pw_STDV[entry.Key] += Math.Pow((P.right - P.left) - _pw_Mean[entry.Key], 2.0);
                    _pV_STDV[entry.Key] += Math.Pow(P.metadata.value - _pV_Mean[entry.Key], 2.0);
                }
            }

            foreach (KeyValuePair<string, ChrStatistics> entry in _Chrs)
            {
                if ((double)entry.Value.count != 0)
                {
                    _pw_STDV[entry.Key] = Math.Sqrt(_pw_STDV[entry.Key] / (double)entry.Value.count);
                    _pV_STDV[entry.Key] = Math.Sqrt(_pV_STDV[entry.Key] / (double)entry.Value.count);
                }
                else
                {
                    _pw_STDV[entry.Key] = 0;
                    _pV_STDV[entry.Key] = 0;
                }
            }

            #endregion

            if (total_Peak_count > 0)
            {
                double pValuesSum = 0;

                #region -_-        Store chromosome wide statistics         -_-

                foreach (KeyValuePair<string, ChrStatistics> entry in _Chrs)
                {
                    if (entry.Value.peakWidthMin == uint.MaxValue) entry.Value.peakWidthMin = 0;

                    if (entry.Value.pValueMin == 1) entry.Value.pValueMin = 0;

                    _data.chrStatistics.Add(entry.Key, new ChrStatistics()
                    {
                        chrTitle = FixChrCasing(entry.Key),
                        count = entry.Value.count,
                        percentage = (Math.Round((double)((entry.Value.count * 100) / total_Peak_count), decimalPlaces)).ToString() + " %",

                        peakWidthMax = entry.Value.peakWidthMax,
                        peakWidthMin = entry.Value.peakWidthMin,
                        peakWidthMean = Math.Round((float)_pw_Mean[entry.Key], decimalPlaces),
                        peakWidth_STDV = Math.Round((float)_pw_STDV[entry.Key], decimalPlaces),

                        pValueMax = entry.Value.pValueMax,
                        pValueMin = entry.Value.pValueMin,
                        pValueMean = _pV_Mean[entry.Key],
                        pValueSTDV = _pV_STDV[entry.Key],

                        coverage = (float)Math.Round(((int)_pw__Sum[entry.Key] * 100.0) / _basePairsCount[FixChrCasing(entry.Key)], decimalPlaces)
                    });

                    if (!double.IsNaN(_pV_Mean[entry.Key]))
                    {
                        pValuesSum += (double)_pV_Mean[entry.Key];
                    }

                    /*if (i != _chrCount - 1 && i != _chrCount - 2 && _Chrs[i].count == 0)
                    {
                        missingChrs.Add(_Chrs[i].chrTitle);
                    }*/
                }

                #endregion

                _data.pValueMean = pValuesSum / total_Peak_count;
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

        private void AddNewChromosome(string newChr)
        {
            _data.peaks.Add(newChr, new List<Peak>());
            _Chrs.Add(newChr, new ChrStatistics()
            {
                chrTitle = newChr,
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

            _pw__Sum.Add(newChr, 0);
            _pw_Mean.Add(newChr, 0.0);
            _pw_STDV.Add(newChr, 0.0);
            _pV__Sum.Add(newChr, 0.0);
            _pV_Mean.Add(newChr, 0.0);
            _pV_STDV.Add(newChr, 0.0);
        }

        /// <summary>
        /// Returns case-fixed chr title. 
        /// <para>During parsing process all chr title are changed to
        /// lower-case. However, it is prefered to have chrX, chrY and 
        /// chrM in camel casing. This function will do conversion.</para>
        /// </summary>
        /// <param name="allLowerCaseChr">camel-cased chr titles.</param>
        /// <returns></returns>
        private string FixChrCasing(string allLowerCaseChr)
        {
            switch (allLowerCaseChr)
            {
                case "chrx": return "chrX";
                case "chry": return "chrY";
                case "chrm": return "chrM";
                default: return allLowerCaseChr;
            }
        }

        private UInt32 GetFileHashKey(string file)
        {
            int len = file.Length;

            UInt32 hashKey = 0;
            for (int i = 0; i < len; i++)
            {
                hashKey += file[i];
                hashKey += (hashKey << 10);
                hashKey ^= (hashKey >> 6);
            }

            hashKey += (hashKey << 3);
            hashKey ^= (hashKey >> 11);
            hashKey += (hashKey << 15);

            return hashKey;
        }
    }
}
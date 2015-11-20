using System;
using System.Configuration;
using System.IO;

namespace Polimi.DEIB.VahidJalili.DI4.CLI
{
    static class UserConfigSetup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns TRUE if everything was OK. 
        /// Returns FALSE if something went wrong and application must exit.</returns>
        public static bool SetGet()
        {
            _updateConfiguration = false;
            _confingFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _settings = _confingFile.AppSettings.Settings;

            if (!GetMemoryType()) return false;
            if (!SetGetWorkingDirectory()) return false;
            if (!SetGetLogFile()) return false;
            if (!SetGetInvertedIndex()) return false;
            if (!SetGetIncrementalIndex()) return false;
            if (_settings[_memoryKey].Value.ToLower() == "hdd")
            {   
                if (!SetGetMinCache()) return false;
                if (!SetGetMaxCache()) return false;
            }
            if (!GetParserParameters()) return false;

            if (_updateConfiguration)
                _confingFile.Save();

            UserConfig.workingDirectory = _settings[_workingDirectoryKey].Value;
            UserConfig.logFile = _settings[_logFileKey].Value;
            UserConfig.memory = (_settings[_memoryKey].Value.ToLower() == "hdd" ? DI4B.Memory.HDD : DI4B.Memory.RAM);
            UserConfig.minCacheSize = UserConfig.memory == DI4B.Memory.HDD ? Convert.ToInt32(_settings[_minCacheKey].Value) : 0;
            UserConfig.maxCacheSize = UserConfig.memory == DI4B.Memory.HDD ? Convert.ToInt32(_settings[_maxCacheKey].Value) : 0;
            UserConfig.ParserParameters.chrColumn = Convert.ToByte(_settings[_chrColKey].Value);
            UserConfig.ParserParameters.leftEndColumn = Convert.ToByte(_settings[_leftEndColKey].Value);
            UserConfig.ParserParameters.rightEndColumn = Convert.ToByte(_settings[_rightEndColKey].Value);
            UserConfig.ParserParameters.nameColumn = Convert.ToByte(_settings[_nameColKey].Value);
            UserConfig.ParserParameters.valueColumn = Convert.ToByte(_settings[_valueColKey].Value);

            if (_settings[_invertedIndexKey].Value.ToLower() == "y" && _settings[_incrementalIndexKey].Value.ToLower() == "y")
                UserConfig.indexType = IndexType.Both;
            else if (_settings[_invertedIndexKey].Value.ToLower() == "y")
                UserConfig.indexType = IndexType.OnlyInverted;
            else
                UserConfig.indexType = IndexType.OnlyIncremental;

            return true;
        }

        private static bool _updateConfiguration { set; get; }
        private static Configuration _confingFile { set; get; }
        private static KeyValueConfigurationCollection _settings { set; get; }

        #region .::.   Keys   .::.
        private static string _memoryKey { get { return "Memory"; } }
        private static string _workingDirectoryKey { get { return "WorkingDirectory"; } }
        private static string _logFileKey { get { return "LogFile"; } }
        private static string _invertedIndexKey { get { return "EnableInvertedIndex"; } }
        private static string _incrementalIndexKey { get { return "EnableIncrementalIndex"; } }        
        private static string _minCacheKey { get { return "MinBInCache"; } }
        private static string _maxCacheKey { get { return "MaxBInCache"; } }
        private static string _chrColKey { get { return "Parser__ChrColumn"; } }
        private static string _leftEndColKey { get { return "Parser__LeftEndColumn"; } }
        private static string _rightEndColKey { get { return "Parser__RightEndColumn"; } }
        private static string _nameColKey { get { return "Parser__NameColumn"; } }
        private static string _valueColKey { get { return "Parser__ValueColumn"; } }
        #endregion

        private static bool GetMemoryType()
        {
            if (_settings[_memoryKey] == null)
            {
                _settings.Add(_memoryKey, "HDD");
                _updateConfiguration = true;
            }
            else
            {
                if (_settings[_memoryKey].Value.ToLower() != "ram" &&
                    _settings[_memoryKey].Value.ToLower() != "hdd")
                {
                    ExitMessage("Invalid memory type.");
                    return false;
                }
            }

            Console.WriteLine(string.Format("Di4 runs {0}", (_settings[_memoryKey].Value.ToLower() == "ram" ? "in RAM" : "on HDD")));
            Console.WriteLine("");

            return true;
        }
        private static bool SetGetWorkingDirectory()
        {
            Uri wdURI = null;
            string wd = "";
            if (_settings[_workingDirectoryKey] == null)
            {
                Console.WriteLine("Current directory: {0}", Environment.CurrentDirectory);
                Console.Write("Please specify working directory : ");

                string line = Console.ReadLine();
                while (Uri.TryCreate(line, UriKind.RelativeOrAbsolute, out wdURI) == false ||
                    line.Trim() == "" || line.Trim() == "\\")
                {
                    if (line.Trim() == "\\")
                        line = Environment.CurrentDirectory;
                    else
                    {
                        Console.WriteLine("Error: Incorrect Format!");
                        Console.Write("Please specify working directory : ");
                        line = Console.ReadLine();
                    }
                }

                _updateConfiguration = true;

            }
            else
            {
                if (Uri.TryCreate(_settings[_workingDirectoryKey].Value, UriKind.RelativeOrAbsolute, out wdURI))                
                    Console.WriteLine("Configuration defines a valid working directory.");                
                else
                {
                    ExitMessage("Invalid working directory.");
                    return false;
                }
            }

            if (Path.GetExtension(wdURI.AbsolutePath).Trim() == "")
                wd = Path.GetDirectoryName(wdURI.AbsolutePath) + Path.DirectorySeparatorChar + Path.GetFileName(wdURI.AbsolutePath) + Path.DirectorySeparatorChar;
            else
                wd = Path.GetDirectoryName(wdURI.AbsoluteUri) + Path.DirectorySeparatorChar;

            wd = Path.GetFullPath(wd);
            if (!Directory.Exists(wd))
            {
                Directory.CreateDirectory(wd);
                Console.WriteLine("The directory is created.");
            }

            if (_updateConfiguration)
                _settings.Add(_workingDirectoryKey, wd);

            Console.WriteLine("Working directory is successfully set to : {0}", wd);
            Console.WriteLine("");

            return true;
        }
        private static bool SetGetLogFile()
        {
            Uri logFileURI = null;
            string logFilePath = "", logFile = "", readFileName = "";
            if (_settings[_logFileKey] == null)
            {
                bool validFileProvided = false;
                while (!validFileProvided)
                {
                    Console.Write("Please specify the Log file : ");
                    readFileName = Console.ReadLine();
                    if (readFileName.Trim() != "")
                    {
                        if (Uri.TryCreate(readFileName, UriKind.RelativeOrAbsolute, out logFileURI))
                        {
                            try
                            {
                                readFileName = Path.GetFullPath(readFileName);
                                if (Path.GetFileName(readFileName) != null &&
                                    Path.GetFileName(readFileName).Trim() != "")
                                    break;
                            }
                            catch (Exception)
                            { }
                        }
                    }

                    Console.WriteLine("Error: Incorrect Format!");
                }

                _updateConfiguration = true;
            }
            else
            {
                if (Uri.TryCreate(_settings[_logFileKey].Value, UriKind.RelativeOrAbsolute, out logFileURI))
                {
                    try
                    {
                        readFileName = Path.GetFullPath(_settings[_logFileKey].Value);
                        if (Path.GetFileName(readFileName) != null &&
                            Path.GetFileName(readFileName).Trim() != "")
                            Console.WriteLine("Configuration defines a valid Log file.");
                    }
                    catch (Exception)
                    {
                        ExitMessage("Invalid Log file.");
                        return false;
                    }
                }                    
                else
                {
                    ExitMessage("Invalid Log file.");
                    return false;
                }
            }

            //readFileName = Path.GetFullPath(logFileURI.OriginalString);
            logFilePath = Path.GetDirectoryName(readFileName);
            logFile = Path.GetFileName(readFileName);

            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
                Console.WriteLine("The directory is created.");
            }

            if (_updateConfiguration)
                _settings.Add(_logFileKey, logFilePath + Path.DirectorySeparatorChar + logFile);

            Console.WriteLine("Log file is successfully set to : {0}", logFilePath + Path.DirectorySeparatorChar + logFile);
            Console.WriteLine("");

            return true;
        }
        private static bool SetGetInvertedIndex()
        {
            if (_settings[_invertedIndexKey] == null)
            {
                Console.Write("Do you want to enable inverted index ? [y/n] ");
                char key = Console.ReadKey().KeyChar;
                while (key != 'y' && key != 'n')
                {
                    Console.WriteLine("\nError: Incorrect argument!");
                    Console.Write("Please press \"y\" to Enable the inverted index, or \"n\" to Disable [y/n]: ");
                    key = Console.ReadKey().KeyChar;
                }

                Console.WriteLine("");
                Console.WriteLine("Inverted index is {0} !", (key == 'y' ? "enabled" : "disabled"));
                _settings.Add(_invertedIndexKey, key.ToString());
                _updateConfiguration = true;
            }
            else
            {
                switch (_settings[_invertedIndexKey].Value)
                {
                    case "y": Console.WriteLine("Inverted index is Enabled !"); break;
                    case "n": Console.WriteLine("Inverted index is Disabled !"); break;
                    default:
                        ExitMessage("Invalid inverted index parameter.");
                        return false;
                }
            }

            Console.WriteLine("");
            return true;
        }
        private static bool SetGetIncrementalIndex()
        {
            if (_settings[_incrementalIndexKey] == null)
            {
                Console.Write("Do you want to enable incremental inverted index ? [y/n] ");
                char key = Console.ReadKey().KeyChar;
                while (key != 'y' && key != 'n')
                {
                    Console.WriteLine("\nError: Incorrect argument!");
                    Console.Write("Please press \"y\" to Enable the incremental inverted index, or \"n\" to Disable [y/n]: ");
                    key = Console.ReadKey().KeyChar;
                }

                Console.WriteLine("");
                Console.WriteLine("Incremental inverted index is {0} !", (key == 'y' ? "enabled" : "disabled"));
                _settings.Add(_incrementalIndexKey, key.ToString());
                _updateConfiguration = true;
            }
            else
            {
                switch (_settings[_incrementalIndexKey].Value)
                {
                    case "y": Console.WriteLine("Incremental inverted index is Enabled !"); break;
                    case "n": Console.WriteLine("Incremental inverted index is Disabled !"); break;
                    default:
                        ExitMessage("Invalid incremental inverted index parameter.");
                        return false;
                }
            }

            Console.WriteLine("");
            return true;
        }
        private static bool SetGetMinCache()
        {
            int minHistorySize;
            if (_settings[_minCacheKey] == null)
            {
                _updateConfiguration = true;
                Console.Write("Please specify the minimum history size (#bookmarks) per each index instance : ");                

                while (!int.TryParse(Console.ReadLine(), out minHistorySize))
                {
                    Console.WriteLine("\nError: Incorrect number!");
                    Console.Write("Please specify the minimum history size (#bookmarks) per each index instance : ");
                }

                _settings.Add(_minCacheKey, minHistorySize.ToString());
                _updateConfiguration = true;
            }
            else
            {
                if (int.TryParse(_settings[_minCacheKey].Value, out minHistorySize))
                    Console.WriteLine("Configuration defines a valid minimum history size.");
                else
                {
                    ExitMessage("Invalid minimum history size.");
                    return false;
                }
            }


            Console.WriteLine(string.Format("Minimum history size is set to [{0:N0}] per each index instance.", minHistorySize));
            Console.WriteLine("");

            return true;
        }
        private static bool SetGetMaxCache()
        {
            int maxHistorySize;
            if (_settings[_maxCacheKey] == null)
            {
                _updateConfiguration = true;
                Console.Write("Please specify the maximum history size (#bookmarks) per each index instance : ");

                while (!int.TryParse(Console.ReadLine(), out maxHistorySize) || maxHistorySize < Convert.ToInt32(_settings[_minCacheKey].Value))
                {
                    if (maxHistorySize < Convert.ToInt32(_settings[_minCacheKey].Value))
                        Console.WriteLine("\nMaximum history size should be greater than minimum history size ({0:N0}).", Convert.ToInt32(_settings[_minCacheKey].Value));
                    else
                        Console.WriteLine("\nError: Incorrect number!");
                    Console.Write("Please specify the maximum history size (#bookmarks) per each index instance : ");
                }

                _settings.Add(_maxCacheKey, maxHistorySize.ToString());
                _updateConfiguration = true;
            }
            else
            {
                if (int.TryParse(_settings[_maxCacheKey].Value, out maxHistorySize))
                    Console.WriteLine("Configuration defines a valid maximum history size.");
                else
                {
                    ExitMessage("Invalid maximum history size.");
                    return false;
                }
            }

            Console.WriteLine(string.Format("Maximum history size is set to <{0:N0}> per each index instance.", maxHistorySize));
            Console.WriteLine("");

            return true;
        }        
        private static bool GetParserParameters()
        {
            if (_settings[_chrColKey] == null)
            {
                _settings.Add(_chrColKey, "0");
                _updateConfiguration = true;
            }

            if (_settings[_leftEndColKey] == null)
            {
                _settings.Add(_leftEndColKey, "3");
                _updateConfiguration = true;
            }

            if (_settings[_rightEndColKey] == null)
            {
                _settings.Add(_rightEndColKey, "4");
                _updateConfiguration = true;
            }

            if (_settings[_nameColKey] == null)
            {
                _settings.Add(_nameColKey, "8");
                _updateConfiguration = true;
            }

            if (_settings[_valueColKey] == null)
            {
                _settings.Add(_valueColKey, "5");
                _updateConfiguration = true;
            }

            return true;
        }
        private static void ExitMessage(string message)
        {
            Console.WriteLine("Error in configuration file. {0} Please manually correct configuration file.", message);
            Console.WriteLine("Application will exit now.");
            Console.ReadKey();
        }
    }
}

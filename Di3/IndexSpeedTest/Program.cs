using CSharpTest.Net.Serialization;
using DI3;
using System.IO;

namespace IndexSpeedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\VahidTest";//Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;

            Tests_v2 SpeedTest_V2 = new Tests_v2();
            SpeedTest_V2.path = path;
            //SpeedTest_V2.Test_01();
            //SpeedTest_V2.Test_02();
            //SpeedTest_V2.Test_03();
            //SpeedTest_V2.Test_04();
            //SpeedTest_V2.Test_05();
            //SpeedTest_V2.Test_06();
            //SpeedTest_V2.Test_07();
            //SpeedTest_V2.Test_08();
            //SpeedTest_V2.Test_09();
            //SpeedTest_V2.Test_10();
            //SpeedTest_V2.Test_11();
            //SpeedTest_V2.Test_12();
            //SpeedTest_V2.Test_13();
            //SpeedTest_V2.Test_14();
            //SpeedTest_V2.Test_15();
            //SpeedTest_V2.Test_16();
            //SpeedTest_V2.Test_17();
            //SpeedTest_V2.Test_18();
            //SpeedTest_V2.Test_19();
            //SpeedTest_V2.Test_20();
            //SpeedTest_V2.Test_21();
            //SpeedTest_V2.Test_22();
            //SpeedTest_V2.Test_23();
            //SpeedTest_V2.Test_24();
            //SpeedTest_V2.Test_25();
            //SpeedTest_V2.Test_26();
            //SpeedTest_V2.Test_27();
            SpeedTest_V2.Test_28();


            //ControledIntervals ControlledIntervals = new ControledIntervals();
            //ControlledIntervals.path = path;
            //ControlledIntervals.Sequence();
            //ControlledIntervals.Controlled_10_05_10();
            



            //IndexSpeedTest_v1 SpeedTest = new IndexSpeedTest_v1();
            //string path = @"D:\VahidTest";//Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
            //SpeedTest.Run(2000, 200000, path, "TEST_11", 5, 500, 500, 1000);
            //SpeedTest.Run(2000, 200000, true, path, "Test_1", 5, 500, 500, 1000);
            //SpeedTest.Run(path, "smallTest");


            #region .::.          Test 1          .::.

            /// Condensed Regions
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_1", // Test Name
                5,        // min gap
                50,       // max gap
                50,       // min lenght
                100);     // max lenght
            */

            #endregion

            #region .::.          Test 2          .::.

            /// Spars Regions
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_2", // Test Name
                50,       // min gap
                500,      // max gap
                500,      // min lenght
                1000);    // max lenght
            */

            #endregion

            #region .::.          Test 3          .::.

            /// Spars Regions & Big node sizes (both key and currentValue)
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_3", // Test Name
                50,       // min gap
                500,      // max gap
                500,      // min lenght
                1000,     // max lenght
                100,      // min Child Nodes
                200,      // max Child Nodes
                100,      // min newValue Nodes
                200);     // max newValue Nodes
            */

            #endregion

            #region .::.          Test 4          .::.

            /// Spars Regions & Small node sizes (both key and currentValue)
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_4", // Test Name
                50,       // min gap
                500,      // max gap
                500,      // min lenght
                1000,     // max lenght
                4,        // min Child Nodes
                8,        // max Child Nodes
                4,        // min newValue Nodes
                8);       // max newValue Nodes
            */

            #endregion

            #region .::.          Test 5          .::.

            /// Most condensed regions
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_5", // Test Name
                5,        // min gap
                10,       // max gap
                20,       // min lenght
                60);      // max lenght
            */

            #endregion

            #region .::.          Test 6          .::.

            /// Sparse regions; tweak key-currentValue sizes.
            /*
            SpeedTest.Run(
                2000,        // sample count
                200000,      // region count
                true,        // Keep or Dispose Di3?
                path,        // output path
                "Test_6",    // Test Name
                5,           // min gap
                10,          // max gap
                20,          // min lenght
                60,          // max lenght
                sizeof(int), // Size of Key
                24);         // Size of newValue
            */

            #endregion

            #region .::.          Test 7          .::.

            /// Sparse regions; tweak key-currentValue sizes.
            /*
            SpeedTest.Run(
                2000,        // sample count
                200000,      // region count
                true,        // Keep or Dispose Di3?
                path,        // output path
                "Test_7",    // Test Name
                5,           // min gap
                10,          // max gap
                20,          // min lenght
                60,          // max lenght
                sizeof(uint),// Size of Key
                1);          // Size of newValue
            */

            #endregion

            #region .::.          Test 8          .::.

            /// Sparse regions; tweak key-currentValue sizes.
            /*
            SpeedTest.Run(
                2000,        // sample count
                200000,      // region count
                true,        // Keep or Dispose Di3?
                path,        // output path
                "Test_9",    // Test Name
                5,           // min gap
                10,          // max gap
                20,          // min lenght
                60,          // max lenght
                sizeof(uint),// Size of Key
                14000);      // Size of newValue
            */
            #endregion

            #region .::.          Test 9          .::.

            /// Sparse regions; tweak key-currentValue sizes.            
            /*
            SpeedTest.Run(
                2000,            // sample count
                200000,          // region count
                true,            // Keep or Dispose Di3?
                path,            // output path
                "Test_9",        // Test Name
                5,               // min gap
                10,              // max gap
                20,              // min lenght
                60,              // max lenght
                2 * sizeof(uint),// Size of Key
                14000);          // Size of newValue
            */
            #endregion

            #region .::.          Test 10         .::.

            /// Sparse regions; tweak key-currentValue sizes.
            /*
            SpeedTest.Run(
                2000,            // sample count
                200000,          // region count
                true,            // Keep or Dispose Di3?
                path,            // output path
                "Test_10",       // Test Name
                5,               // min gap
                10,              // max gap
                20,              // min lenght
                60,              // max lenght
                2 * sizeof(uint),// Size of Key
                4000);          // Size of newValue
            */
            #endregion

            #region .::.          Test 11         .::.

            /// Condensed Regions
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_11", // Test Name
                5,        // min gap
                50,       // max gap
                50,       // min lenght
                100,      // max lenght
                4,
                196);
            
            */
            #endregion

            #region .::.          Test 12         .::.

            /// Condensed Regions
            /*
            SpeedTest.Run(
                1000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_12", // Test Name
                50,        // min gap
                500,       // max gap
                500,       // min lenght
                1000,      // max lenght
                4,
                64);

            */
            #endregion

            #region .::.          Test 17         .::.

            /// Condensed Regions
            /*
            SpeedTest.Run(
                1000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_17", // Test Name
                50,        // min gap
                500,       // max gap
                500,       // min lenght
                1000//,      // max lenght
                //2,         // min child nodes
                //256,       // max child nodes
                //2,         // min currentValue nodes
                //256);      // max currentValue nodes
                , 4, 64);
            */
            #endregion

            #region .::.          Test 18         .::.

            /// Condensed Regions
            /*
            SpeedTest.Run(
                1000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_18", // Test Name
                50,        // min gap
                500,       // max gap
                500,       // min lenght
                1000,      // max lenght
                128,         // min child nodes
                256,       // max child nodes
                128,         // min currentValue nodes
                256);      // max currentValue nodes
            */
            #endregion

            #region .::.          Test 20: Multi-Threading          .::.

            /// Condensed Regions + Multi-Threading
            /*
            SpeedTest.Run(
                150,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Di3?
                path,     // output path
                "Test_20_MultiThread", // Test Name
                50,        // min gap
                500,       // max gap
                500,       // min lenght
                1000//,      // max lenght
                //2,         // min child nodes
                //256,       // max child nodes
                //2,         // min currentValue nodes
                //256);      // max currentValue nodes
                , 4, 64, true);
            */
            #endregion
        }
    }
}

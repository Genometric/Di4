
namespace IndexSpeedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"F:\directSim";//Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;

            //SpeedTest_v1(path);
            SpeedTest_v2(path);
        }

        private static void SpeedTest_v1(string path)
        {
            IndexSpeedTest_v1 indexSpeedTest_v1 = new IndexSpeedTest_v1();

            #region .::.          Test 1          .::.

            /// Condensed Regions
            /*
            indexSpeedTest_v1.RunOLD(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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
            indexSpeedTest_v1.RunOLD(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
                path,     // output path
                "Test_2", // Test Name
                50,       // min gap
                500,      // max gap
                500,      // min lenght
                1000);    // max lenght
            */

            #endregion

            #region .::.          Test 3          .::.

            /// Spars Regions & Big node sizes (both newKey and currentValue)
            /*
            indexSpeedTest_v1.RunOLD(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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

            /// Spars Regions & Small node sizes (both newKey and currentValue)
            /*
            indexSpeedTest_v1.RunOLD(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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
            indexSpeedTest_v1.RunOLD(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
                path,     // output path
                "Test_5", // Test Name
                5,        // min gap
                10,       // max gap
                20,       // min lenght
                60);      // max lenght
            */

            #endregion

            #region .::.          Test 6          .::.

            /// Sparse regions; tweak newKey-currentValue sizes.
            /*
            indexSpeedTest_v1.RunOLD(
                2000,        // sample count
                200000,      // region count
                true,        // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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

            /// Sparse regions; tweak newKey-currentValue sizes.
            /*
            indexSpeedTest_v1.RunOLD(
                2000,        // sample count
                200000,      // region count
                true,        // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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

            /// Sparse regions; tweak newKey-currentValue sizes.
            /*
            indexSpeedTest_v1.RunOLD(
                2000,        // sample count
                200000,      // region count
                true,        // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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

            /// Sparse regions; tweak newKey-currentValue sizes.            
            /*
            indexSpeedTest_v1.RunOLD(
                2000,            // sample count
                200000,          // region count
                true,            // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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

            /// Sparse regions; tweak newKey-currentValue sizes.
            /*
            indexSpeedTest_v1.RunOLD(
                2000,            // sample count
                200000,          // region count
                true,            // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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
            indexSpeedTest_v1.RunOLD(
                2000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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
            indexSpeedTest_v1.RunOLD(
                1000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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
            indexSpeedTest_v1.RunOLD(
                1000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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
            indexSpeedTest_v1.RunOLD(
                1000,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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
            indexSpeedTest_v1.RunOLD(
                150,     // sample count
                200000,   // region count
                true,     // Keep or Dispose Polimi.DEIB.VahidJalili.DI3?
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
        private static void SpeedTest_v2(string path)
        {
            IndexSpeedTest_v2 indexSpeedTest_v2 = new IndexSpeedTest_v2();
            indexSpeedTest_v2.path = path;
            //indexSpeedTest_v2.Test_01();
            //indexSpeedTest_v2.Test_02();
            //indexSpeedTest_v2.Test_03();
            //indexSpeedTest_v2.Test_04();
            //indexSpeedTest_v2.Test_05();
            //indexSpeedTest_v2.Test_06();
            //indexSpeedTest_v2.Test_07();
            //indexSpeedTest_v2.Test_08();
            //indexSpeedTest_v2.Test_09();
            //indexSpeedTest_v2.Test_10();
            //indexSpeedTest_v2.Test_11();
            //indexSpeedTest_v2.Test_12();
            //indexSpeedTest_v2.Test_13();
            //indexSpeedTest_v2.Test_14();
            //indexSpeedTest_v2.Test_15();
            //indexSpeedTest_v2.Test_16();
            //indexSpeedTest_v2.Test_17();
            //indexSpeedTest_v2.Test_18();
            //indexSpeedTest_v2.Test_19();
            //indexSpeedTest_v2.Test_20();
            //indexSpeedTest_v2.Test_21();
            //indexSpeedTest_v2.Test_22();
            //indexSpeedTest_v2.Test_23();
            //indexSpeedTest_v2.Test_24();
            //indexSpeedTest_v2.Test_25();
            //indexSpeedTest_v2.Test_26();
            //indexSpeedTest_v2.Test_27();
            indexSpeedTest_v2.Test_28();
        }
    }
}

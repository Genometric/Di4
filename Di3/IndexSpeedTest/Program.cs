using System.IO;

namespace IndexSpeedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IndexSpeedTest SpeedTest = new IndexSpeedTest();

            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;

            /// Test 1:
            /// Condensed Regions
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                path,     // output path
                "Test_1", // Test Name
                5,        // min gap
                50,       // max gap
                50,       // min lenght
                100);     // max lenght
            */



            /// Test 2:
            /// Spars Regions
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                path,     // output path
                "Test_2", // Test Name
                50,       // min gap
                500,      // max gap
                500,      // min lenght
                1000);    // max lenght
            */



            /// Test 3:
            /// Spars Regions & Big node sizes (both key and value)
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                path,     // output path
                "Test_3", // Test Name
                50,       // min gap
                500,      // max gap
                500,      // min lenght
                1000,     // max lenght
                100,      // min Child Nodes
                200,      // max Child Nodes
                100,      // min Value Nodes
                200);     // max Value Nodes
            */



            /// Test 4:
            /// Spars Regions & Small node sizes (both key and value)
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                path,     // output path
                "Test_4", // Test Name
                50,       // min gap
                500,      // max gap
                500,      // min lenght
                1000,     // max lenght
                4,        // min Child Nodes
                8,        // max Child Nodes
                4,        // min Value Nodes
                8);       // max Value Nodes
            */




            /// Test 5:
            /// Most condensed regions
            /*
            SpeedTest.Run(
                2000,     // sample count
                200000,   // region count
                path,     // output path
                "Test_5", // Test Name
                5,        // min gap
                10,       // max gap
                20,       // min lenght
                60);      // max lenght
            */



            /// Test 6:
            /// Sparse regions; tweak key-value sizes.
            SpeedTest.Run(
                2000,        // sample count
                200000,      // region count
                path,        // output path
                "Test_6",    // Test Name
                5,           // min gap
                10,          // max gap
                20,          // min lenght
                60,          // max lenght
                sizeof(int), // Size of Key
                24);         // Size of Value
        }
    }
}

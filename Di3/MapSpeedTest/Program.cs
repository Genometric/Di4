
namespace MapSpeedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MapSpeedTest mapSpeedTest = new MapSpeedTest();

            mapSpeedTest.Run(@"F:\directSim\Di3_Test_28.idx1R", 100000, 50, 5000);
        }
    }
}

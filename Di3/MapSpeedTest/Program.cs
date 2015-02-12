
namespace MapSpeedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MapSpeedTest mapSpeedTest = new MapSpeedTest();

            mapSpeedTest.Run(@"I:\\test\\Di3chr1U.indx.idx1R", 100000, 5, 50);
        }
    }
}

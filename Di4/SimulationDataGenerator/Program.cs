namespace Polimi.DEIB.VahidJalili.DI4.SimulationDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            RegionGenerator gR = new RegionGenerator();
            gR.GenerateSimulationRegions();
        }
    }
}

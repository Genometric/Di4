namespace Polimi.DEIB.VahidJalili.DI4.SimulationDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            RegionGenerator gR = new RegionGenerator();
            //gR.GenerateSimulationRegions();
            //gR.GenerateSimulationRegions(10, 20, 1, 1, 50);
            //gR.GenerateSimulationRegions(40, 9, 12, 20, 100);
            //gR.GenerateSimulationRegions(new RegionGenerator.ErlangDistribution(30, 40), new RegionGenerator.ErlangDistribution(10, 40), 100);
            gR.GenerateSimulationRegions(new RegionGenerator.ErlangDistribution(10, 20), new RegionGenerator.ErlangDistribution(1, 1), 75);
        }
    }
}


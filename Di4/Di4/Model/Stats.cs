namespace Genometric.Di4
{
    public struct Stats
    {
        public Stats(int intervalCount, int bookmarkCount, int blockCount) : this()
        {
            this.intervalCount = intervalCount;
            this.bookmarkCount = bookmarkCount;
            this.blockCount = blockCount;
        }
        public int intervalCount { private set; get; }
        public int bookmarkCount { private set; get; }
        public int blockCount { private set; get; }
    }
}

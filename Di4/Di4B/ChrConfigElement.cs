using System.Configuration;

namespace Polimi.DEIB.VahidJalili.DI4.DI4B
{
    public class ChrConfigElement : ConfigurationElement
    {
        public ChrConfigElement() { }
        
        public ChrConfigElement(string Chr, char Strand, string Index)
        {
            this.chr = Chr;
            this.strand = Strand;
            this.index = Index;
        }

        [ConfigurationProperty("Chr", DefaultValue = "Initial", IsRequired = true)]
        public string chr
        {
            get { return (string)this["Chr"]; }
            set { this["Chr"] = value; }
        }

        [ConfigurationProperty("Strand", DefaultValue = "*", IsRequired = true)]
        public char strand
        {
            get { return (char)this["Strand"]; }
            set { this["Strand"] = value; }
        }

        [ConfigurationProperty("Index", DefaultValue = "Initial", IsRequired = true)]
        public string index
        {
            get { return (string)this["Index"]; }
            set { this["Index"] = value; }
        }
    }
    public class Genome : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ChrConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ChrConfigElement)element).chr;
        }

        public int IndexOf(ChrConfigElement chrElement)
        {
            return BaseIndexOf(chrElement);
        }

        public void Add(ChrConfigElement chrElement)
        {
            BaseAdd(chrElement);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
    }
    public class ChrSection : ConfigurationSection
    {
        public ChrSection()
        { }

        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "Chromosomes";
        private const string genome = "GenomeEndpoints";

        [ConfigurationProperty(genome)]
        [ConfigurationCollection(typeof(Genome), AddItemName = "add")]
        public Genome genomeChrs { get { return (Genome)base[genome]; } }
    }
}

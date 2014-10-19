using System.Configuration;

namespace Di3B
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
        public Genome()
        {
            ChrConfigElement chrElement = (ChrConfigElement)CreateNewElement();
            Add(chrElement);
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ChrConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ChrConfigElement)element).chr;
        }        

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /*protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ChrConfigElement)element).chr;
        }*/

        public ChrConfigElement this[int index]
        {
            get
            {
                return (ChrConfigElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public ChrConfigElement this[string Name]
        {
            get
            {
                return (ChrConfigElement)BaseGet(Name);
            }
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

        public void Remove(ChrConfigElement chrElement)
        {
            if (BaseIndexOf(chrElement) >= 0)
                BaseRemove(chrElement.chr);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
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

        /*
        [ConfigurationProperty("Chromosome")]
        public ChrConfigElement ChrElement
        {
            get { return ((ChrConfigElement)this["Chromosome"]); }
            set { this["Chromosome"] = currentValue; }
        }*/
    }








    public class ConnectionManagerDataSection : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "ConnectionManagerDataSection";

        private const string EndpointCollectionName = "ConnectionManagerEndpoints";

        [ConfigurationProperty(EndpointCollectionName)]
        [ConfigurationCollection(typeof(ConnectionManagerEndpointsCollection), AddItemName = "add")]
        public ConnectionManagerEndpointsCollection ConnectionManagerEndpoints { get { return (ConnectionManagerEndpointsCollection)base[EndpointCollectionName]; } }
    }

    public class ConnectionManagerEndpointsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionManagerEndpointElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConnectionManagerEndpointElement)element).Name;
        }
    }

    public class ConnectionManagerEndpointElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("address", IsRequired = true)]
        public string Address
        {
            get { return (string)this["address"]; }
            set { this["address"] = value; }
        }

        [ConfigurationProperty("useSSL", IsRequired = false, DefaultValue = false)]
        public bool UseSSL
        {
            get { return (bool)this["useSSL"]; }
            set { this["useSSL"] = value; }
        }

        [ConfigurationProperty("securityGroupsAllowedToSaveChanges", IsRequired = false)]
        public string SecurityGroupsAllowedToSaveChanges
        {
            get { return (string)this["securityGroupsAllowedToSaveChanges"]; }
            set { this["securityGroupsAllowedToSaveChanges"] = value; }
        }
    }


}

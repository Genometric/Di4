using System.Configuration;

namespace Di3B
{
    /// <summary>
    /// Class holds the <Message> element
    /// </summary>
    public class MessageElement : ConfigurationElement
    {
        // Holds the Name attribute of the Message
        private static readonly ConfigurationProperty messageName =
            new ConfigurationProperty("Name", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        // Holds the newValue attribute currentValue of Message.
        private static readonly ConfigurationProperty messageValue =
            new ConfigurationProperty("Value", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        public MessageElement()
        {
            base.Properties.Add(messageName);
            base.Properties.Add(messageValue);
        }

        /// <summary>
        /// Name
        /// </summary>
        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (string)this[messageName]; }
        }

        /// <summary>
        /// newValue
        /// </summary>
        [ConfigurationProperty("Value", IsRequired = true)]
        public string Value
        {
            get { return (string)this[messageValue]; }
        }
    }


    [ConfigurationCollection(typeof(MessageElement), AddItemName = "Message",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class MessageCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MessageElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MessageElement)element).Name;
        }

        new public MessageElement this[string name]
        {
            get { return (MessageElement)BaseGet(name); }
        }
    }











    public class MyConfiguration : ConfigurationSection
    {
        private static readonly ConfigurationProperty toAttribute =
             new ConfigurationProperty("To", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);
        
        private static readonly ConfigurationProperty fromAttribute =
             new ConfigurationProperty("From", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty messagesElement =
             new ConfigurationProperty("Messages", typeof(MessageCollection), null, ConfigurationPropertyOptions.IsRequired);
        public MyConfiguration()
        {
            base.Properties.Add(toAttribute);
            base.Properties.Add(fromAttribute);
            base.Properties.Add(messagesElement);
        }
        /// <summary>
        /// To
        /// </summary>
        [ConfigurationProperty("To", IsRequired = true)]
        public string To
        {
            get { return (string)this[toAttribute]; }
        }
        
        /// <summary>
        /// From
        /// </summary>
        [ConfigurationProperty("From", IsRequired = true)]
        public string From
        {
            get { return (string)this[fromAttribute]; }
        }
        /// <summary>
        /// Messages Collection
        /// </summary>
        [ConfigurationProperty("Messages", IsRequired = true)]
        public MessageCollection Messages
        {
            get { return (MessageCollection)this[messagesElement]; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ICPMD
{
    /// <summary>
    /// Interface representing ChIP-seq Peak Metadata.
    /// </summary>
    public interface ICPMetadata<C> : IMetaData/*<C>*/
    {
        /// <summary>
        /// Sets and gets zero-based chromosome number.
        /// </summary>
        byte chrNo { set; get; }

        /// <summary>
        /// Sets and gets peak name.
        /// </summary>
        string name { set; get; }

        /// <summary>
        /// Sets and gets peak value.
        /// </summary>
        double value { set; get; }

        /// <summary>
        /// Sets and gets the strand of peak.
        /// </summary>
        char strand { set; get; }
    }
}

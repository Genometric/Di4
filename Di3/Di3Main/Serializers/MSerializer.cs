using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI3.Interfaces;
using Google.ProtocolBuffers.PeakSearilizer;

namespace Di3BMain.Serializers
{
    public interface MSerializer<T> : IMSerializer<PeakDataClass>
    {
        public PeakClass ReadFrom(System.IO.Stream stream)
        {
            Peak peakData = Peak.ParseDelimitedFrom(stream);

            return new PeakClass()
            {
                left = peakData.Left,
                right = peakData.Right,
                metadata = new PeakDataClass()
                {
                    chrNo = Convert.ToByte(peakData.Metadata.ChrNo), // there should be a problem here ;-( 
                    left = peakData.Metadata.Left,
                    right = peakData.Metadata.Right,
                    strand = Convert.ToChar(peakData.Metadata.Strand), // also, there should be here ;-( 
                    name = peakData.Metadata.Name,
                    value = peakData.Metadata.Value
                }
            };
        }

        public void WriteTo(PeakClass value, System.IO.Stream stream)
        {
            PeakData.Builder peakData = PeakData.CreateBuilder();

            peakData.SetChrNo(Google.ProtocolBuffers.ByteString.CopyFrom(new byte[] { Convert.ToByte(value.metadata.chrNo) }));
            peakData.SetLeft(value.metadata.left);
            peakData.SetRight(value.metadata.right);
            peakData.SetStrand(value.metadata.strand);
            peakData.SetName(value.metadata.name);
            peakData.SetValue(value.metadata.value);
            peakData.SetHashKey(10); // this must be updated. Instead of GetHashKey function in metadata; but a UInt64 hashkey pre-generated. 

            Peak.Builder peak = Peak.CreateBuilder();
            peak.SetLeft(value.left);
            peak.SetRight(value.right);
            peak.SetMetadata(peakData);

            peak.Build().WriteTo(stream);
        }
    }
}

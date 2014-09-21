// Generated by ProtoGen, Version=2.4.1.521, Culture=neutral, PublicKeyToken=55f7125234beb589.  DO NOT EDIT!
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace Google.ProtocolBuffers.PeakSearilizer {
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class PeakUmbrellaClass {
  
    #region Extension registration
    public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
    }
    #endregion
    #region Static variables
    internal static pbd::MessageDescriptor internal__static_PeakPBSerializer_PeakData__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::Google.ProtocolBuffers.PeakSearilizer.PeakData, global::Google.ProtocolBuffers.PeakSearilizer.PeakData.Builder> internal__static_PeakPBSerializer_PeakData__FieldAccessorTable;
    internal static pbd::MessageDescriptor internal__static_PeakPBSerializer_Peak__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::Google.ProtocolBuffers.PeakSearilizer.Peak, global::Google.ProtocolBuffers.PeakSearilizer.Peak.Builder> internal__static_PeakPBSerializer_Peak__FieldAccessorTable;
    #endregion
    #region Descriptor
    public static pbd::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbd::FileDescriptor descriptor;
    
    static PeakUmbrellaClass() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          "ChN0dXRvcmlhbC9QZWFrLnByb3RvEhBQZWFrUEJTZXJpYWxpemVyGiRnb29n" + 
          "bGUvcHJvdG9idWYvY3NoYXJwX29wdGlvbnMucHJvdG8ihgEKCFBlYWtEYXRh" + 
          "EhAKBWNock5vGAEgAigMOgEwEgwKBG5hbWUYAiACKAkSEAoFdmFsdWUYAyAC" + 
          "KAE6ATASEQoGc3RyYW5kGAQgAigFOgEwEg8KBGxlZnQYBSACKAU6ATASEAoF" + 
          "cmlnaHQYBiACKAU6ATASEgoHaGFzaEtleRgHIAIoBDoBMCJXCgRQZWFrEg8K" + 
          "BGxlZnQYASACKAU6ATASEAoFcmlnaHQYAiACKAU6ATASLAoIbWV0YWRhdGEY" + 
          "AyACKAsyGi5QZWFrUEJTZXJpYWxpemVyLlBlYWtEYXRhQj9IAcI+OgolR29v" + 
          "Z2xlLlByb3RvY29sQnVmZmVycy5QZWFrU2VhcmlsaXplchIRUGVha1VtYnJl" + 
          "bGxhQ2xhc3M=");
      pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
        descriptor = root;
        internal__static_PeakPBSerializer_PeakData__Descriptor = Descriptor.MessageTypes[0];
        internal__static_PeakPBSerializer_PeakData__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::Google.ProtocolBuffers.PeakSearilizer.PeakData, global::Google.ProtocolBuffers.PeakSearilizer.PeakData.Builder>(internal__static_PeakPBSerializer_PeakData__Descriptor,
                new string[] { "ChrNo", "Name", "Value", "Strand", "Left", "Right", "HashKey", });
        internal__static_PeakPBSerializer_Peak__Descriptor = Descriptor.MessageTypes[1];
        internal__static_PeakPBSerializer_Peak__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::Google.ProtocolBuffers.PeakSearilizer.Peak, global::Google.ProtocolBuffers.PeakSearilizer.Peak.Builder>(internal__static_PeakPBSerializer_Peak__Descriptor,
                new string[] { "Left", "Right", "Metadata", });
        pb::ExtensionRegistry registry = pb::ExtensionRegistry.CreateInstance();
        RegisterAllExtensions(registry);
        global::Google.ProtocolBuffers.DescriptorProtos.CSharpOptions.RegisterAllExtensions(registry);
        return registry;
      };
      pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
          new pbd::FileDescriptor[] {
          global::Google.ProtocolBuffers.DescriptorProtos.CSharpOptions.Descriptor, 
          }, assigner);
    }
    #endregion
    
  }
  #region Messages
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PeakData : pb::GeneratedMessage<PeakData, PeakData.Builder> {
    private PeakData() { }
    private static readonly PeakData defaultInstance = new PeakData().MakeReadOnly();
    private static readonly string[] _peakDataFieldNames = new string[] { "chrNo", "hashKey", "left", "name", "right", "strand", "value" };
    private static readonly uint[] _peakDataFieldTags = new uint[] { 10, 56, 40, 18, 48, 32, 25 };
    public static PeakData DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override PeakData DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override PeakData ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::Google.ProtocolBuffers.PeakSearilizer.PeakUmbrellaClass.internal__static_PeakPBSerializer_PeakData__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<PeakData, PeakData.Builder> InternalFieldAccessors {
      get { return global::Google.ProtocolBuffers.PeakSearilizer.PeakUmbrellaClass.internal__static_PeakPBSerializer_PeakData__FieldAccessorTable; }
    }
    
    public const int ChrNoFieldNumber = 1;
    private bool hasChrNo;
    private pb::ByteString chrNo_ = (pb::ByteString) global::Google.ProtocolBuffers.PeakSearilizer.PeakData.Descriptor.Fields[0].DefaultValue;
    public bool HasChrNo {
      get { return hasChrNo; }
    }
    public pb::ByteString ChrNo {
      get { return chrNo_; }
    }
    
    public const int NameFieldNumber = 2;
    private bool hasName;
    private string name_ = "";
    public bool HasName {
      get { return hasName; }
    }
    public string Name {
      get { return name_; }
    }
    
    public const int ValueFieldNumber = 3;
    private bool hasValue;
    private double value_;
    public bool HasValue {
      get { return hasValue; }
    }
    public double Value {
      get { return value_; }
    }
    
    public const int StrandFieldNumber = 4;
    private bool hasStrand;
    private int strand_;
    public bool HasStrand {
      get { return hasStrand; }
    }
    public int Strand {
      get { return strand_; }
    }
    
    public const int LeftFieldNumber = 5;
    private bool hasLeft;
    private int left_;
    public bool HasLeft {
      get { return hasLeft; }
    }
    public int Left {
      get { return left_; }
    }
    
    public const int RightFieldNumber = 6;
    private bool hasRight;
    private int right_;
    public bool HasRight {
      get { return hasRight; }
    }
    public int Right {
      get { return right_; }
    }
    
    public const int HashKeyFieldNumber = 7;
    private bool hasHashKey;
    private ulong hashKey_;
    public bool HasHashKey {
      get { return hasHashKey; }
    }
    [global::System.CLSCompliant(false)]
    public ulong HashKey {
      get { return hashKey_; }
    }
    
    public override bool IsInitialized {
      get {
        if (!hasChrNo) return false;
        if (!hasName) return false;
        if (!hasValue) return false;
        if (!hasStrand) return false;
        if (!hasLeft) return false;
        if (!hasRight) return false;
        if (!hasHashKey) return false;
        return true;
      }
    }
    
    public override void WriteTo(pb::ICodedOutputStream output) {
      int size = SerializedSize;
      string[] field_names = _peakDataFieldNames;
      if (hasChrNo) {
        output.WriteBytes(1, field_names[0], ChrNo);
      }
      if (hasName) {
        output.WriteString(2, field_names[3], Name);
      }
      if (hasValue) {
        output.WriteDouble(3, field_names[6], Value);
      }
      if (hasStrand) {
        output.WriteInt32(4, field_names[5], Strand);
      }
      if (hasLeft) {
        output.WriteInt32(5, field_names[2], Left);
      }
      if (hasRight) {
        output.WriteInt32(6, field_names[4], Right);
      }
      if (hasHashKey) {
        output.WriteUInt64(7, field_names[1], HashKey);
      }
      UnknownFields.WriteTo(output);
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (hasChrNo) {
          size += pb::CodedOutputStream.ComputeBytesSize(1, ChrNo);
        }
        if (hasName) {
          size += pb::CodedOutputStream.ComputeStringSize(2, Name);
        }
        if (hasValue) {
          size += pb::CodedOutputStream.ComputeDoubleSize(3, Value);
        }
        if (hasStrand) {
          size += pb::CodedOutputStream.ComputeInt32Size(4, Strand);
        }
        if (hasLeft) {
          size += pb::CodedOutputStream.ComputeInt32Size(5, Left);
        }
        if (hasRight) {
          size += pb::CodedOutputStream.ComputeInt32Size(6, Right);
        }
        if (hasHashKey) {
          size += pb::CodedOutputStream.ComputeUInt64Size(7, HashKey);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
    }
    
    public static PeakData ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static PeakData ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static PeakData ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static PeakData ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static PeakData ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static PeakData ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static PeakData ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static PeakData ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static PeakData ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static PeakData ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private PeakData MakeReadOnly() {
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(PeakData prototype) {
      return new Builder(prototype);
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public sealed partial class Builder : pb::GeneratedBuilder<PeakData, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(PeakData cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private PeakData result;
      
      private PeakData PrepareBuilder() {
        if (resultIsReadOnly) {
          PeakData original = result;
          result = new PeakData();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override PeakData MessageBeingBuilt {
        get { return PrepareBuilder(); }
      }
      
      public override Builder Clear() {
        result = DefaultInstance;
        resultIsReadOnly = true;
        return this;
      }
      
      public override Builder Clone() {
        if (resultIsReadOnly) {
          return new Builder(result);
        } else {
          return new Builder().MergeFrom(result);
        }
      }
      
      public override pbd::MessageDescriptor DescriptorForType {
        get { return global::Google.ProtocolBuffers.PeakSearilizer.PeakData.Descriptor; }
      }
      
      public override PeakData DefaultInstanceForType {
        get { return global::Google.ProtocolBuffers.PeakSearilizer.PeakData.DefaultInstance; }
      }
      
      public override PeakData BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      public override Builder MergeFrom(pb::IMessage other) {
        if (other is PeakData) {
          return MergeFrom((PeakData) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(PeakData other) {
        if (other == global::Google.ProtocolBuffers.PeakSearilizer.PeakData.DefaultInstance) return this;
        PrepareBuilder();
        if (other.HasChrNo) {
          ChrNo = other.ChrNo;
        }
        if (other.HasName) {
          Name = other.Name;
        }
        if (other.HasValue) {
          Value = other.Value;
        }
        if (other.HasStrand) {
          Strand = other.Strand;
        }
        if (other.HasLeft) {
          Left = other.Left;
        }
        if (other.HasRight) {
          Right = other.Right;
        }
        if (other.HasHashKey) {
          HashKey = other.HashKey;
        }
        this.MergeUnknownFields(other.UnknownFields);
        return this;
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        PrepareBuilder();
        pb::UnknownFieldSet.Builder unknownFields = null;
        uint tag;
        string field_name;
        while (input.ReadTag(out tag, out field_name)) {
          if(tag == 0 && field_name != null) {
            int field_ordinal = global::System.Array.BinarySearch(_peakDataFieldNames, field_name, global::System.StringComparer.Ordinal);
            if(field_ordinal >= 0)
              tag = _peakDataFieldTags[field_ordinal];
            else {
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              continue;
            }
          }
          switch (tag) {
            case 0: {
              throw pb::InvalidProtocolBufferException.InvalidTag();
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                if (unknownFields != null) {
                  this.UnknownFields = unknownFields.Build();
                }
                return this;
              }
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              break;
            }
            case 10: {
              result.hasChrNo = input.ReadBytes(ref result.chrNo_);
              break;
            }
            case 18: {
              result.hasName = input.ReadString(ref result.name_);
              break;
            }
            case 25: {
              result.hasValue = input.ReadDouble(ref result.value_);
              break;
            }
            case 32: {
              result.hasStrand = input.ReadInt32(ref result.strand_);
              break;
            }
            case 40: {
              result.hasLeft = input.ReadInt32(ref result.left_);
              break;
            }
            case 48: {
              result.hasRight = input.ReadInt32(ref result.right_);
              break;
            }
            case 56: {
              result.hasHashKey = input.ReadUInt64(ref result.hashKey_);
              break;
            }
          }
        }
        
        if (unknownFields != null) {
          this.UnknownFields = unknownFields.Build();
        }
        return this;
      }
      
      
      public bool HasChrNo {
        get { return result.hasChrNo; }
      }
      public pb::ByteString ChrNo {
        get { return result.ChrNo; }
        set { SetChrNo(value); }
      }
      public Builder SetChrNo(pb::ByteString value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasChrNo = true;
        result.chrNo_ = value;
        return this;
      }
      public Builder ClearChrNo() {
        PrepareBuilder();
        result.hasChrNo = false;
        result.chrNo_ = (pb::ByteString) global::Google.ProtocolBuffers.PeakSearilizer.PeakData.Descriptor.Fields[0].DefaultValue;
        return this;
      }
      
      public bool HasName {
        get { return result.hasName; }
      }
      public string Name {
        get { return result.Name; }
        set { SetName(value); }
      }
      public Builder SetName(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasName = true;
        result.name_ = value;
        return this;
      }
      public Builder ClearName() {
        PrepareBuilder();
        result.hasName = false;
        result.name_ = "";
        return this;
      }
      
      public bool HasValue {
        get { return result.hasValue; }
      }
      public double Value {
        get { return result.Value; }
        set { SetValue(value); }
      }
      public Builder SetValue(double value) {
        PrepareBuilder();
        result.hasValue = true;
        result.value_ = value;
        return this;
      }
      public Builder ClearValue() {
        PrepareBuilder();
        result.hasValue = false;
        result.value_ = 0D;
        return this;
      }
      
      public bool HasStrand {
        get { return result.hasStrand; }
      }
      public int Strand {
        get { return result.Strand; }
        set { SetStrand(value); }
      }
      public Builder SetStrand(int value) {
        PrepareBuilder();
        result.hasStrand = true;
        result.strand_ = value;
        return this;
      }
      public Builder ClearStrand() {
        PrepareBuilder();
        result.hasStrand = false;
        result.strand_ = 0;
        return this;
      }
      
      public bool HasLeft {
        get { return result.hasLeft; }
      }
      public int Left {
        get { return result.Left; }
        set { SetLeft(value); }
      }
      public Builder SetLeft(int value) {
        PrepareBuilder();
        result.hasLeft = true;
        result.left_ = value;
        return this;
      }
      public Builder ClearLeft() {
        PrepareBuilder();
        result.hasLeft = false;
        result.left_ = 0;
        return this;
      }
      
      public bool HasRight {
        get { return result.hasRight; }
      }
      public int Right {
        get { return result.Right; }
        set { SetRight(value); }
      }
      public Builder SetRight(int value) {
        PrepareBuilder();
        result.hasRight = true;
        result.right_ = value;
        return this;
      }
      public Builder ClearRight() {
        PrepareBuilder();
        result.hasRight = false;
        result.right_ = 0;
        return this;
      }
      
      public bool HasHashKey {
        get { return result.hasHashKey; }
      }
      [global::System.CLSCompliant(false)]
      public ulong HashKey {
        get { return result.HashKey; }
        set { SetHashKey(value); }
      }
      [global::System.CLSCompliant(false)]
      public Builder SetHashKey(ulong value) {
        PrepareBuilder();
        result.hasHashKey = true;
        result.hashKey_ = value;
        return this;
      }
      public Builder ClearHashKey() {
        PrepareBuilder();
        result.hasHashKey = false;
        result.hashKey_ = 0UL;
        return this;
      }
    }
    static PeakData() {
      object.ReferenceEquals(global::Google.ProtocolBuffers.PeakSearilizer.PeakUmbrellaClass.Descriptor, null);
    }
  }
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class Peak : pb::GeneratedMessage<Peak, Peak.Builder> {
    private Peak() { }
    private static readonly Peak defaultInstance = new Peak().MakeReadOnly();
    private static readonly string[] _peakFieldNames = new string[] { "left", "metadata", "right" };
    private static readonly uint[] _peakFieldTags = new uint[] { 8, 26, 16 };
    public static Peak DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override Peak DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override Peak ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::Google.ProtocolBuffers.PeakSearilizer.PeakUmbrellaClass.internal__static_PeakPBSerializer_Peak__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<Peak, Peak.Builder> InternalFieldAccessors {
      get { return global::Google.ProtocolBuffers.PeakSearilizer.PeakUmbrellaClass.internal__static_PeakPBSerializer_Peak__FieldAccessorTable; }
    }
    
    public const int LeftFieldNumber = 1;
    private bool hasLeft;
    private int left_;
    public bool HasLeft {
      get { return hasLeft; }
    }
    public int Left {
      get { return left_; }
    }
    
    public const int RightFieldNumber = 2;
    private bool hasRight;
    private int right_;
    public bool HasRight {
      get { return hasRight; }
    }
    public int Right {
      get { return right_; }
    }
    
    public const int MetadataFieldNumber = 3;
    private bool hasMetadata;
    private global::Google.ProtocolBuffers.PeakSearilizer.PeakData metadata_;
    public bool HasMetadata {
      get { return hasMetadata; }
    }
    public global::Google.ProtocolBuffers.PeakSearilizer.PeakData Metadata {
      get { return metadata_ ?? global::Google.ProtocolBuffers.PeakSearilizer.PeakData.DefaultInstance; }
    }
    
    public override bool IsInitialized {
      get {
        if (!hasLeft) return false;
        if (!hasRight) return false;
        if (!hasMetadata) return false;
        if (!Metadata.IsInitialized) return false;
        return true;
      }
    }
    
    public override void WriteTo(pb::ICodedOutputStream output) {
      int size = SerializedSize;
      string[] field_names = _peakFieldNames;
      if (hasLeft) {
        output.WriteInt32(1, field_names[0], Left);
      }
      if (hasRight) {
        output.WriteInt32(2, field_names[2], Right);
      }
      if (hasMetadata) {
        output.WriteMessage(3, field_names[1], Metadata);
      }
      UnknownFields.WriteTo(output);
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (hasLeft) {
          size += pb::CodedOutputStream.ComputeInt32Size(1, Left);
        }
        if (hasRight) {
          size += pb::CodedOutputStream.ComputeInt32Size(2, Right);
        }
        if (hasMetadata) {
          size += pb::CodedOutputStream.ComputeMessageSize(3, Metadata);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
    }
    
    public static Peak ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static Peak ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static Peak ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static Peak ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static Peak ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static Peak ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static Peak ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static Peak ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static Peak ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static Peak ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private Peak MakeReadOnly() {
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(Peak prototype) {
      return new Builder(prototype);
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public sealed partial class Builder : pb::GeneratedBuilder<Peak, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(Peak cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private Peak result;
      
      private Peak PrepareBuilder() {
        if (resultIsReadOnly) {
          Peak original = result;
          result = new Peak();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override Peak MessageBeingBuilt {
        get { return PrepareBuilder(); }
      }
      
      public override Builder Clear() {
        result = DefaultInstance;
        resultIsReadOnly = true;
        return this;
      }
      
      public override Builder Clone() {
        if (resultIsReadOnly) {
          return new Builder(result);
        } else {
          return new Builder().MergeFrom(result);
        }
      }
      
      public override pbd::MessageDescriptor DescriptorForType {
        get { return global::Google.ProtocolBuffers.PeakSearilizer.Peak.Descriptor; }
      }
      
      public override Peak DefaultInstanceForType {
        get { return global::Google.ProtocolBuffers.PeakSearilizer.Peak.DefaultInstance; }
      }
      
      public override Peak BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      public override Builder MergeFrom(pb::IMessage other) {
        if (other is Peak) {
          return MergeFrom((Peak) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(Peak other) {
        if (other == global::Google.ProtocolBuffers.PeakSearilizer.Peak.DefaultInstance) return this;
        PrepareBuilder();
        if (other.HasLeft) {
          Left = other.Left;
        }
        if (other.HasRight) {
          Right = other.Right;
        }
        if (other.HasMetadata) {
          MergeMetadata(other.Metadata);
        }
        this.MergeUnknownFields(other.UnknownFields);
        return this;
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        PrepareBuilder();
        pb::UnknownFieldSet.Builder unknownFields = null;
        uint tag;
        string field_name;
        while (input.ReadTag(out tag, out field_name)) {
          if(tag == 0 && field_name != null) {
            int field_ordinal = global::System.Array.BinarySearch(_peakFieldNames, field_name, global::System.StringComparer.Ordinal);
            if(field_ordinal >= 0)
              tag = _peakFieldTags[field_ordinal];
            else {
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              continue;
            }
          }
          switch (tag) {
            case 0: {
              throw pb::InvalidProtocolBufferException.InvalidTag();
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                if (unknownFields != null) {
                  this.UnknownFields = unknownFields.Build();
                }
                return this;
              }
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              break;
            }
            case 8: {
              result.hasLeft = input.ReadInt32(ref result.left_);
              break;
            }
            case 16: {
              result.hasRight = input.ReadInt32(ref result.right_);
              break;
            }
            case 26: {
              global::Google.ProtocolBuffers.PeakSearilizer.PeakData.Builder subBuilder = global::Google.ProtocolBuffers.PeakSearilizer.PeakData.CreateBuilder();
              if (result.hasMetadata) {
                subBuilder.MergeFrom(Metadata);
              }
              input.ReadMessage(subBuilder, extensionRegistry);
              Metadata = subBuilder.BuildPartial();
              break;
            }
          }
        }
        
        if (unknownFields != null) {
          this.UnknownFields = unknownFields.Build();
        }
        return this;
      }
      
      
      public bool HasLeft {
        get { return result.hasLeft; }
      }
      public int Left {
        get { return result.Left; }
        set { SetLeft(value); }
      }
      public Builder SetLeft(int value) {
        PrepareBuilder();
        result.hasLeft = true;
        result.left_ = value;
        return this;
      }
      public Builder ClearLeft() {
        PrepareBuilder();
        result.hasLeft = false;
        result.left_ = 0;
        return this;
      }
      
      public bool HasRight {
        get { return result.hasRight; }
      }
      public int Right {
        get { return result.Right; }
        set { SetRight(value); }
      }
      public Builder SetRight(int value) {
        PrepareBuilder();
        result.hasRight = true;
        result.right_ = value;
        return this;
      }
      public Builder ClearRight() {
        PrepareBuilder();
        result.hasRight = false;
        result.right_ = 0;
        return this;
      }
      
      public bool HasMetadata {
       get { return result.hasMetadata; }
      }
      public global::Google.ProtocolBuffers.PeakSearilizer.PeakData Metadata {
        get { return result.Metadata; }
        set { SetMetadata(value); }
      }
      public Builder SetMetadata(global::Google.ProtocolBuffers.PeakSearilizer.PeakData value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasMetadata = true;
        result.metadata_ = value;
        return this;
      }
      public Builder SetMetadata(global::Google.ProtocolBuffers.PeakSearilizer.PeakData.Builder builderForValue) {
        pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
        PrepareBuilder();
        result.hasMetadata = true;
        result.metadata_ = builderForValue.Build();
        return this;
      }
      public Builder MergeMetadata(global::Google.ProtocolBuffers.PeakSearilizer.PeakData value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        if (result.hasMetadata &&
            result.metadata_ != global::Google.ProtocolBuffers.PeakSearilizer.PeakData.DefaultInstance) {
            result.metadata_ = global::Google.ProtocolBuffers.PeakSearilizer.PeakData.CreateBuilder(result.metadata_).MergeFrom(value).BuildPartial();
        } else {
          result.metadata_ = value;
        }
        result.hasMetadata = true;
        return this;
      }
      public Builder ClearMetadata() {
        PrepareBuilder();
        result.hasMetadata = false;
        result.metadata_ = null;
        return this;
      }
    }
    static Peak() {
      object.ReferenceEquals(global::Google.ProtocolBuffers.PeakSearilizer.PeakUmbrellaClass.Descriptor, null);
    }
  }
  
  #endregion
  
}

#endregion Designer generated code

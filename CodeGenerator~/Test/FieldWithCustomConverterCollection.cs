using Leap.Forward.JsonVersioning;
using System;

[JsonVersion]
public partial class FieldWithCustomConverterCollection
{
    [JsonTokenConverter(typeof(JsonTokenDateTimeConverter))]
    public DateTime CustomSerializedField { get; set; }
}

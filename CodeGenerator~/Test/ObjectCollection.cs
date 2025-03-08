using Leap.Forward.JsonVersioning;
using System.Collections.Generic;

[JsonVersion]
public partial class ObjectCollection
{
    public List<NewVersion> Items { get; private set; } = new List<NewVersion>();
}

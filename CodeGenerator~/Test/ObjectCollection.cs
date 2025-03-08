using Leap.Forward.JsonVersioning;
using System.Collections.Generic;

[JsonVersion]
public partial class ObjectCollection
{
    public IList<object> Items { get; private set; } = new List<object>();
}

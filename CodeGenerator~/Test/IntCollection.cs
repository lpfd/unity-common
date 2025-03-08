using Leap.Forward.JsonVersioning;
using System.Collections.Generic;

[JsonVersion]
public partial class IntCollection
{
    public List<int> ListOfInts { get; private set; } = new List<int>();
}

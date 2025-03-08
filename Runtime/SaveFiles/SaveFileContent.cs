using Leap.Forward.JsonVersioning;
using UnityEngine;

namespace Leap.Forward.SaveFiles
{
    [JsonVersion]
    public partial class SaveFileContent
    {
        public string SceneName { get; set; }
        public string StateName { get; set; }
    }
}

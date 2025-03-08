using System;

namespace Leap.Forward.JsonVersioning
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonVersionAttribute : Attribute
    {
        public JsonVersionAttribute()
        {
        }

        /// <summary>
        /// Version of the type.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Previous version of the type.
        /// </summary>
        public Type PreviousVersion { get; set; }
    }
}

using System;
using UnityEngine;

namespace Leap.Forward.JsonVersioning
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonTokenConverterAttribute : Attribute
    {
        public JsonTokenConverterAttribute(Type converter)
        {
            Converter = converter;
        }

        /// <summary>
        /// JToken converter class.
        /// </summary>
        public Type Converter { get; private set; }
    }
}

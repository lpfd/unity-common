using System;
using UnityEngine;

namespace Leap.Forward
{
    [Serializable]
    public struct GameStateReference : ISerializationCallbackReceiver
    {
        [SerializeField]
        public string TypeName;

        public Type Type { get; private set; }

        GameStateReference(Type type)
        {
            Type = type;
            TypeName = type.FullName;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            TypeName = Type?.FullName;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(TypeName))
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type = assembly.GetType(TypeName);
                    if (Type != null)
                        break;
                }
            }
        }
    }
}
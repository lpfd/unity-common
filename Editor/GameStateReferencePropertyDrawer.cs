using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

namespace Leap.Forward
{
    [CustomPropertyDrawer(typeof(GameStateReference))]
    public sealed class GameStateReferencePropertyDrawer : PropertyDrawer
    {
        static string[] GetAllTypeNames()
        {
            return new List<string> { "None" }
                .Concat(TypeCache.GetTypesDerivedFrom<IGameState>()
                    .Select(type => type.FullName))
                .ToArray();
        }

        static string GetLabel(Type type) => $"{type.Namespace}/{type.Name}";

        string[] names;

        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            if (names == null)
            {
                names = GetAllTypeNames();
            }

            var typeNameProp = prop.FindPropertyRelative("TypeName");

            using (new EditorGUI.PropertyScope(rect, label, prop))
            {
                var labelRect = new Rect(rect.x, rect.y, rect.width, 18f);
                var popupRect = new Rect(rect.x, rect.y + labelRect.height, rect.width, 18f);

                var index = Array.IndexOf(names, typeNameProp.stringValue);
                if (index < 0) index = 0;

                EditorGUI.LabelField(labelRect, label);
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    index = EditorGUI.Popup(popupRect, index, names);
                    if (check.changed)
                    {
                        typeNameProp.stringValue = names[index];
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18f + 18f + 4f;
        }
    }
}
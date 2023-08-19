using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumDropdownAttribute))]
public class EnumDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Enum)
        {
            property.enumValueIndex = EditorGUI.Popup(position, property.displayName, property.enumValueIndex, property.enumDisplayNames);
        }
        else
        {
            EditorGUI.LabelField(position, "Use EnumDropdown with enum type.");
        }

        EditorGUI.EndProperty();
    }
}

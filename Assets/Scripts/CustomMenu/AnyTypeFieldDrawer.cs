using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnyTypeFieldAttribute))]
public class AnyTypeFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        EditorGUI.PropertyField(position, property, label, true);

        if (EditorGUI.EndChangeCheck())
        {
            // Handle any processing after a value change if needed
        }
    }
}


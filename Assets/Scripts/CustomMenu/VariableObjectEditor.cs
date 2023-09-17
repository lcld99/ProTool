using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Runtime.ExceptionServices;
using System;

[CustomEditor(typeof(VariableObject))]
public class VariableObjectEditor : Editor
{
    private List<string> propertyNames = new List<string>();

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // Draw the default inspector fields
        DrawDefaultInspector();

        // Get the target VariableObject component
        VariableObject variableObject = target as VariableObject;
        //Debug.Log(variableObject.selectedPropertyName);

        // Check if the GameObject field has changed

        // Ensure the targetGameObject is assigned
        if (variableObject.scriptTargetObject != null)
        {
            //UpdatePropertyNames(variableObject);
            propertyNames.Clear();
            propertyNames = variableObject.scriptTargetObject.GetFieldNames();
            // Ensure there are property names to display
            if (propertyNames.Count > 0)
            {
                //Debug.Log(variableObject.selectedIndex);
                //Debug.Log("choice: " + _choiceIndex);
                // Find the index of the selected property
                // Display a dropdown menu for selecting the property
                //selectedIndex = EditorGUILayout.Popup("Select Property", selectedIndex, propertyNames.ToArray());
                variableObject.selectedIndex = EditorGUILayout.Popup("Select Property", variableObject.selectedIndex, propertyNames.ToArray());

                //Debug.Log(_choiceIndex);
                if (variableObject.selectedIndex > 0)
                {
                    // Update the selected property based on the dropdown selection
                    //variableObject.selectedPropertyName = propertyNames[variableObject.selectedIndex];
                    variableObject.SetInitialPropertyName(propertyNames[variableObject.selectedIndex]);
                    variableObject.GetComponent<ToolTipTrigger>().SetInitialContent();
                }
                else
                {
                    variableObject.selectedIndex = 0;
                    //Debug.Log("passou aq");
                    variableObject.SetInitialPropertyName(propertyNames[0]);
                    variableObject.GetComponent<ToolTipTrigger>().SetInitialContent();
                }
                EditorUtility.SetDirty(target);

            }
            else
            {
                EditorGUILayout.LabelField("No properties found in TrainScript.");
            }
        }
        else
        {
            EditorGUILayout.LabelField("Target GameObject not assigned.");
        }
        serializedObject.ApplyModifiedProperties();
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(VariableObject))]
public class VariableObjectEditor : Editor
{
    private List<string> propertyNames = new List<string>();
    private int selectedIndex = 0;
    private void OnEnable()
    {
        // Get the target VariableObject component
        VariableObject variableObject = (VariableObject)target;

        // Ensure the targetGameObject is assigned
        if (variableObject.targetGameObject != null)
        {
            // Get the TrainScript component from the targetGameObject
            TrainScript trainScript = variableObject.targetGameObject;

            // Ensure the TrainScript component exists
            if (trainScript != null)
            {
                // Retrieve the list of property names from TrainScript
                propertyNames.Clear();
                System.Type type = typeof(TrainProperties);
                foreach (var field in type.GetFields())
                {
                    propertyNames.Add(field.Name);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields
        DrawDefaultInspector();

        // Get the target VariableObject component
        VariableObject variableObject = (VariableObject)target;

        // Ensure the targetGameObject is assigned
        if (variableObject.targetGameObject != null)
        {
            // Ensure there are property names to display
            if (propertyNames.Count > 0)
            {
                // Find the index of the selected property
                
                selectedIndex = propertyNames.IndexOf(variableObject.selectedPropertyName);
                // Display a dropdown menu for selecting the property
                selectedIndex = EditorGUILayout.Popup("Select Property", selectedIndex, propertyNames.ToArray());

                if(selectedIndex > 0)
                {

                    // Update the selected property based on the dropdown selection
                    variableObject.selectedPropertyName = propertyNames[selectedIndex];
                    variableObject.SetInitialPropertyName(propertyNames[selectedIndex]);
                    variableObject.GetComponent<ToolTipTrigger>().SetInitialContent();
                }
                else
                {
                    //variableObject.selectedPropertyName = propertyNames[0];                   
                    variableObject.SetInitialPropertyName(propertyNames[0]);
                    variableObject.GetComponent<ToolTipTrigger>().SetInitialContent();

                }

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
    }
}


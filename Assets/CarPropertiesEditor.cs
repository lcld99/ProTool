
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(VariableCarPropertiesObject))]
public class CarPropertiesEditor : Editor
{
    private List<string> propertyNames = new List<string>();

    private void OnEnable()
    {
        // Get the target VariableCarPropertiesObject component
        VariableCarPropertiesObject variableObject = (VariableCarPropertiesObject)target;

        // Ensure the targetGameObject is assigned
        if (variableObject.targetGameObject != null)
        {
            // Get the CarPropertiesScript component from the targetGameObject
            CarPropertiesScript carPropertiesScript = variableObject.targetGameObject.GetComponent<CarPropertiesScript>();

            // Ensure the CarPropertiesScript component exists
            if (carPropertiesScript != null)
            {
                // Retrieve the list of property names from CarPropertiesScript
                propertyNames.Clear();
                System.Type type = typeof(CarProperties);
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

        // Get the target VariableCarPropertiesObject component
        VariableCarPropertiesObject variableObject = (VariableCarPropertiesObject)target;

        // Ensure the targetGameObject is assigned
        if (variableObject.targetGameObject != null)
        {
            // Ensure there are property names to display
            if (propertyNames.Count > 0)
            {

                // Find the index of the selected property
                int selectedIndex = propertyNames.IndexOf(variableObject.selectedPropertyName);
                // Display a dropdown menu for selecting the property
                selectedIndex = EditorGUILayout.Popup("Select Property", selectedIndex, propertyNames.ToArray());
                if(selectedIndex >= 0)
                {

                    // Update the selected property based on the dropdown selection
                    variableObject.selectedPropertyName = propertyNames[selectedIndex];
                }

            }
            else
            {
                EditorGUILayout.LabelField("No properties found in CarPropertiesScript.");
            }
        }
        else
        {
            EditorGUILayout.LabelField("Target GameObject not assigned.");
        }
    }
}

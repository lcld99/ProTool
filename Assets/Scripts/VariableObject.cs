using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VariableObject : MonoBehaviour
{
    // Store the GameObject with multiple properties
    public GameObject targetGameObject;
    private GameObject parent;

    // Store the name of the selected property (exposed in the Inspector)
    public string selectedPropertyName;

    // Function to get the value of the selected property
    public object GetPropertyValue()
    {
        if (targetGameObject != null && !string.IsNullOrEmpty(selectedPropertyName))
        {
           
            if (targetGameObject != null)
            {
                //Debug.Log(selectedPropertyName);
                object property = targetGameObject.GetComponent<TrainScript>().GetPropertyValue(selectedPropertyName);

                if (property != null)
                {
                    // Extract the property value and return it
                    return property;
                }
                else
                {
                    Debug.LogWarning("Property not found on the component!");
                }
            }
            else
            {
                Debug.LogWarning("Target GameObject does not have the required component!");
            }
        }
        else
        {
            Debug.LogWarning("Missing information. Make sure to select a GameObject and a valid property name.");
        }

        return null;
    }
}


using UnityEngine;

public class VariableCarPropertiesObject : MonoBehaviour
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
                object property = targetGameObject.GetComponent<CarPropertiesScript>().GetPropertyValue(selectedPropertyName);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using TMPro;
using Unity.Loading;
using UnityEngine.UI;

public class VariableObject : MonoBehaviour, IGetToolTipInfo<string>
{
    // Store the GameObject with multiple properties
    public TrainScript targetGameObject;
    public TextMeshProUGUI displayInfo;
    public LayoutElement layoutElement;
    
    private GameObject parent;
    private string previousProperty;
    // Store the name of the selected property (exposed in the Inspector)
    [SerializeField]
    [HideInInspector]
    public string selectedPropertyName;


    // Function to get the value of the selected property

    //public string GetToolTipInfo()
    //{
    //    object value = GetPropertyValue();
    //    return selectedPropertyName+ ": " + value;
    //}

    private void Update()
    {
        if (previousProperty != selectedPropertyName) // Check if the text has changed
        {
            displayInfo.text = selectedPropertyName; // Update the text element
            previousProperty= selectedPropertyName;
        }
        layoutElement.enabled = (displayInfo.preferredWidth >= layoutElement.preferredWidth);
    }
    public object GetPropertyValue()
    {
        if (targetGameObject != null && !string.IsNullOrEmpty(selectedPropertyName))
        {

            if (targetGameObject != null)
            {
                //Debug.Log(selectedPropertyName);
                object property = targetGameObject.GetPropertyValue(selectedPropertyName);

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

    public void SetInitialPropertyName(string property)
    {
        this.selectedPropertyName = property;
        //previousProperty = selectedPropertyName;
    }

    public string ValueGetToolTipInfo()
    {
        object value = GetPropertyValue();
        return selectedPropertyName + ": " + value;
    }
}

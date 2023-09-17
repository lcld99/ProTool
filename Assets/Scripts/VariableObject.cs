using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using TMPro;
using Unity.Loading;
using UnityEngine.UI;

[System.Serializable]
[CustomPropertyDrawer(typeof(GameObject))]
public class VariableObject : MonoBehaviour, IGetToolTipInfo<string>
{
    // Store the GameObject with multiple properties
    [SerializeField]
    public GameObject targetGameObject;

    public TextMeshProUGUI displayInfo;
    public LayoutElement layoutElement;


    [SerializeField]
    [HideInInspector]
    public int selectedIndex = -1;

    public IGetObject scriptTargetObject;
    
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

    //private void OnValidate()
    //{
    //    // This code will be executed whenever the inspector values change,
    //    // including when you assign a GameObject in the editor.
    //    // Use 'yourGameObject' here.
    //    scriptTargetObject = targetGameObject.GetComponent<IGetObject>().GetSelf();
    //    Debug.Log(scriptTargetObject);
    //}

    void OnEnable()
    {
        if (targetGameObject != null)
        {
            OnGameObjectAssigned(targetGameObject);
            Debug.Log("Variavel: " + selectedPropertyName + "Com valor: " + GetPropertyValue());
            this.GetComponent<ToolTipTrigger>().SetInitialContent();
        }
    }



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
        if (scriptTargetObject != null && !string.IsNullOrEmpty(selectedPropertyName))
        {

            if (scriptTargetObject != null)
            {
                //Debug.Log(selectedPropertyName);
                object property = scriptTargetObject.GetPropertyValue(selectedPropertyName);

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

    public void OnGameObjectAssigned(GameObject assignedObject)
    {
        // Custom code to execute when GameObject is assigned
        if(scriptTargetObject == null) {
            scriptTargetObject = assignedObject.GetComponent<IGetObject>().GetSelf();
        }
        else
        {
            //Debug.Log("called");
        }
        //Debug.Log(scriptTargetObject);
        //Debug.Log("GameObject assigned!");
    }
    public void SetInitialPropertyName(string property)
    {
        this.selectedPropertyName = property;
        //previousProperty = selectedPropertyName;
    }

    public List<string> GetProperties()
    {
        if (scriptTargetObject != null)
        {
            return scriptTargetObject.GetFieldNames();
        }
        return null;
    }

    public string ValueGetToolTipInfo()
    {
        object value = GetPropertyValue();
        return selectedPropertyName + ": " + value;
    }

}

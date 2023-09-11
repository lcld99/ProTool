using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarProperties
{
    public int pessoas;
    public string modelo;
}
public class CarPropertiesScript : MonoBehaviour
{
    [SerializeField]
    private CarProperties carProperties;

    private void Awake()
    {
        // Initialize the properties if needed
        carProperties ??= new CarProperties
            {
                pessoas = 2,
                modelo = "corsa",
            };
    }

    // A method that allows you to access a property by its name
    public object GetPropertyValue(string propertyName)
    {
        System.Type type = typeof(CarProperties);
        var property = type.GetField(propertyName);
        if (property != null)
        {
            return property.GetValue(carProperties);
        }
        else
        {
            Debug.LogError($"Property {propertyName} not found in CarPropertiesScript.");
            return null;
        }
    }
}

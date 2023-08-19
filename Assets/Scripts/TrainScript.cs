using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainProperties
{
    public int passengers;
    public float speed;
    public string name;
    // ... Other properties you want to serialize
}
public class TrainScript : MonoBehaviour
{
    [SerializeField]
    private TrainProperties trainProperties;

    private void Awake()
    {
        // Initialize the properties if needed
        if (trainProperties == null)
        {
            trainProperties = new TrainProperties
            {
                passengers = 100,
                speed = 5.0f,
                name = "Taimanin",
                // ... Initialize other properties
            };
        }
    }

    // A method that allows you to access a property by its name
    public object GetPropertyValue(string propertyName)
    {
        System.Type type = typeof(TrainProperties);
        var property = type.GetField(propertyName);
        //var test = type.GetFields();
        //foreach ( var field in test )
        //{
        //    Debug.Log(field);

        //}
        if (property != null)
        {
            return property.GetValue(trainProperties);
        }
        else
        {
            Debug.LogError($"Property '{propertyName}' not found in TrainScript.");
            return null;
        }
    }

}

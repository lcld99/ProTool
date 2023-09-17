using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(GameObject))]
public class GameObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldApplyDrawer(property))
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            if (EditorGUI.EndChangeCheck())
            {
                GameObject targetGameObject = (GameObject)property.objectReferenceValue;
                if (targetGameObject != null)
                {
                    VariableObject script = (VariableObject)property.serializedObject.targetObject;
                    //script.targetGameObject = targetGameObject;
                    script.OnGameObjectAssigned(targetGameObject);
                }
            }
            else
            {
                GameObject targetGameObject = (GameObject)property.objectReferenceValue;
                if (targetGameObject != null)
                {
                    VariableObject script = (VariableObject)property.serializedObject.targetObject;
                    //script.targetGameObject = targetGameObject;
                    script.OnGameObjectAssigned(targetGameObject);
                }
            }
        }
        else
        {
            // Use the default property drawer for other classes
            EditorGUI.PropertyField(position, property, label);
        }
    

    }

    // Define your criteria for when the drawer should be applied
    private bool ShouldApplyDrawer(SerializedProperty property)
    {
        // Example: Apply the drawer if the property belongs to the YourSpecificClass
        return property.serializedObject.targetObject is VariableObject;
    }

}

using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ScriptGeneratorWindow : EditorWindow
{
    private string className = "MyClass";
    private List<PropertyInfo> properties = new List<PropertyInfo>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/Script Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ScriptGeneratorWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Script Generator", EditorStyles.boldLabel);

        className = EditorGUILayout.TextField("Class Name", className);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Add Property"))
        {
            properties.Add(new PropertyInfo());
        }
        if (GUILayout.Button("Generate Script"))
        {
            GenerateScript();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal(); // End the horizontal layout group
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height - 110));

        for (int i = 0; i < properties.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Property Name", GUILayout.Width(100));
            properties[i].name = EditorGUILayout.TextField(properties[i].name, GUILayout.MinWidth(100), GUILayout.MaxWidth(150));

            GUILayout.Label("Property Type", GUILayout.Width(100));
            properties[i].type = (PropertyType)EditorGUILayout.EnumPopup(properties[i].type, GUILayout.MinWidth(100), GUILayout.MaxWidth(150));

            GUILayout.Label("Initial Value", GUILayout.Width(100));
            properties[i].initialValue = EditorGUILayout.TextField(properties[i].initialValue, GUILayout.MinWidth(100), GUILayout.MaxWidth(150));

            if (GUILayout.Button("Remove"))
            {
                properties.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }
        // End the scroll view
        EditorGUILayout.EndScrollView();
    }


    private void GenerateScript()
    {
        if (string.IsNullOrEmpty(className))
        {
            Debug.LogError("Class name cannot be empty.");
            return;
        }



        string variableName = char.ToLower(className[0]) + className.Substring(1); // Convert the first letter to lowercase

        // Generate VariableObject script
        string variableObjectScript = $@"
using UnityEngine;

public class Variable{className}Object : MonoBehaviour
{{
    // Store the GameObject with multiple properties
    public GameObject targetGameObject;
    private GameObject parent;

    // Store the name of the selected property (exposed in the Inspector)
    public string selectedPropertyName;

    // Function to get the value of the selected property
    public object GetPropertyValue()
    {{
        if (targetGameObject != null && !string.IsNullOrEmpty(selectedPropertyName))
        {{
            if (targetGameObject != null)
            {{
                object property = targetGameObject.GetComponent<{className}Script>().GetPropertyValue(selectedPropertyName);

                if (property != null)
                {{
                    // Extract the property value and return it
                    return property;
                }}
                else
                {{
                    Debug.LogWarning(""Property not found on the component!"");
                }}
            }}
            else
            {{
                Debug.LogWarning(""Target GameObject does not have the required component!"");
            }}
        }}
        else
        {{
            Debug.LogWarning(""Missing information. Make sure to select a GameObject and a valid property name."");
        }}

        return null;
    }}
}}
";

        string script = "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n";
        script += "[System.Serializable]\n";
        script += $"public class {className}\n";
        script += "{\n";

        foreach (var property in properties)
        {
            // Add double quotes for string initial values
            //string initialValue = property.TypeName == "string" ? $"\"{property.initialValue}\"" : property.initialValue;
            //Debug.Log(initialValue);
            script += $"    public {property.TypeName} {property.name};\n";
        }

        script += "}\n";

        script += $"public class {className}Script : MonoBehaviour\n"; // Use className here
        script += "{\n";
        script += "    [SerializeField]\n";
        script += $"    private {className} {variableName};\n\n"; // Use variableName here
        script += "    private void Awake()\n";
        script += "    {\n";
        script += "        // Initialize the properties if needed\n";
        script += $"        {variableName} ??= new {className}\n"; // Use variableName here
        script += "            {\n";

        foreach (var property in properties)
        {
            string initialValue = property.TypeName == "string" ? $"\"{property.initialValue}\"" : property.initialValue;
            script += $"                {property.name} = {initialValue},\n";
        }

        script += "            };\n";
        script += "    }\n\n";
        script += "    // A method that allows you to access a property by its name\n";
        script += "    public object GetPropertyValue(string propertyName)\n";
        script += "    {\n";
        script += $"        System.Type type = typeof({className});\n"; // Use className here
        script += "        var property = type.GetField(propertyName);\n";
        script += "        if (property != null)\n";
        script += "        {\n";
        script += $"            return property.GetValue({variableName});\n"; // Use variableName here
        script += "        }\n";
        script += "        else\n";
        script += "        {\n";
        script += "            Debug.LogError($\"Property " + "{propertyName}" + " not found in " + className + "Script.\");\n"; // Use className here
        script += "            return null;\n";
        script += "        }\n";
        script += "    }\n";
        script += "}\n";


        // Generate VariableObjectEditor script
        string variableObjectEditorScript = $@"
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Variable{className}Object))]
public class {className}Editor : Editor
{{
    private List<string> propertyNames = new List<string>();

    private void OnEnable()
    {{
        // Get the target Variable{className}Object component
        Variable{className}Object variableObject = (Variable{className}Object)target;

        // Ensure the targetGameObject is assigned
        if (variableObject.targetGameObject != null)
        {{
            // Get the {className}Script component from the targetGameObject
            {className}Script {variableName}Script = variableObject.targetGameObject.GetComponent<{className}Script>();

            // Ensure the {className}Script component exists
            if ({variableName}Script != null)
            {{
                // Retrieve the list of property names from {className}Script
                propertyNames.Clear();
                System.Type type = typeof({className});
                foreach (var field in type.GetFields())
                {{
                    propertyNames.Add(field.Name);
                }}
            }}
        }}
    }}

    public override void OnInspectorGUI()
    {{
        // Draw the default inspector fields
        DrawDefaultInspector();

        // Get the target Variable{className}Object component
        Variable{className}Object variableObject = (Variable{className}Object)target;

        // Ensure the targetGameObject is assigned
        if (variableObject.targetGameObject != null)
        {{
            // Ensure there are property names to display
            if (propertyNames.Count > 0)
            {{

                // Find the index of the selected property
                int selectedIndex = propertyNames.IndexOf(variableObject.selectedPropertyName);
                // Display a dropdown menu for selecting the property
                selectedIndex = EditorGUILayout.Popup(""Select Property"", selectedIndex, propertyNames.ToArray());
                if(selectedIndex >= 0)
                {{

                    // Update the selected property based on the dropdown selection
                    variableObject.selectedPropertyName = propertyNames[selectedIndex];
                }}

            }}
            else
            {{
                EditorGUILayout.LabelField(""No properties found in {className}Script."");
            }}
        }}
        else
        {{
            EditorGUILayout.LabelField(""Target GameObject not assigned."");
        }}
    }}
}}
";


        //string path = EditorUtility.SaveFilePanel("Save Script", "", className, "cs");
        //if (!string.IsNullOrEmpty(path))
        //{
        //    System.IO.File.WriteAllText(path, script);
        //    AssetDatabase.Refresh();
        //}
        // Save the script to the project folder
        string variableObjectScriptPath = $"Assets/Variable{className}Object.cs";
        string classScriptPath = "Assets/" + className + "Script.cs";
        string editorScriptPath = "Assets/" + className + "Editor.cs";
        System.IO.File.WriteAllText(variableObjectScriptPath, variableObjectScript);
        System.IO.File.WriteAllText(classScriptPath, script);
        System.IO.File.WriteAllText(editorScriptPath, variableObjectEditorScript);

        // Refresh the asset database to make the script visible in Unity
        AssetDatabase.Refresh();

        Debug.Log("Scripts generated: " + variableObjectScriptPath + ", " + classScriptPath);
    }
}

public class PropertyInfo
{
    public string name;
    public PropertyType type;
    public string initialValue;

    public string TypeName
    {
        get
        {
            switch (type)
            {
                case PropertyType.Int:
                    return "int";
                case PropertyType.Float:
                    return "float";
                case PropertyType.String:
                    return "string";
                case PropertyType.Bool:
                    return "bool";
                default:
                    return "object";
            }
        }
    }
}

public enum PropertyType
{
    Int,
    Float,
    String,
    Bool,
    Object
}


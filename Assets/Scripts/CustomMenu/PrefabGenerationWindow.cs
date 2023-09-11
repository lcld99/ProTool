using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrefabGenerationWindow : EditorWindow
{
    private GameObject selectedPrefab;
    private Canvas[] canvasObjects;
    private int selectedCanvasIndex = -1;
    private GameObject newPrefab;
    private SerializedObject serializedPrefab;
    private List<SerializedProperty> exposedProperties = new List<SerializedProperty>();

    [MenuItem("Tools/Prefab Generation Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PrefabGenerationWindow));
    }


    private void OnGUI()
    {
        GUILayout.Label("Prefab Generation", EditorStyles.boldLabel);

        selectedPrefab = EditorGUILayout.ObjectField("Select Prefab", selectedPrefab, typeof(GameObject), false) as GameObject;


        if (selectedPrefab != null && GUILayout.Button("Generate Prefab"))
        {
            PromptUserForCanvas();
        }
    }

    private void PromptUserForCanvas()
    {
        // Find all Canvas objects in the scene
        canvasObjects = GameObject.FindObjectsOfType<Canvas>();

        // Create an array of canvas names for the dialog
        string[] canvasNames = new string[canvasObjects.Length + 1];
        canvasNames[0] = "New Canvas"; // Add option for New Canvas
        for (int i = 0; i < canvasObjects.Length; i++)
        {
            canvasNames[i + 1] = canvasObjects[i].name;
        }

        ShowCanvasSelectionDialog(canvasNames);
    }


    private void ShowCanvasSelectionDialog(string[] canvasNames)
    {
        GenericMenu menu = new GenericMenu();

        string canvasName = "0" + " - " + canvasNames[0];
        menu.AddItem(new GUIContent(canvasName), false, OnCanvasSelected, 0);

        for (int i = canvasNames.Length - 1; i > 0; i--)
        {
            canvasName = (canvasNames.Length - i) + " - " + canvasNames[canvasNames.Length - 1];
            menu.AddItem(new GUIContent(canvasName), false, OnCanvasSelected, i);
        }

        menu.ShowAsContext();
    }

    private void OnCanvasSelected(object index)
    {
        selectedCanvasIndex = (int)index;
        GeneratePrefab();
    }

    private void GeneratePrefab()
    {
        if (selectedCanvasIndex == 0)
        {
            // Create a new Canvas and Event System
            GameObject newCanvas = new GameObject("Canvas");
            Canvas canvasComponent = newCanvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.WorldSpace; // Adjust render mode as needed
            newCanvas.AddComponent<CanvasScaler>();
            newCanvas.AddComponent<GraphicRaycaster>();

            GameObject newEventSystem = new GameObject("EventSystem");
            newEventSystem.AddComponent<EventSystem>();
            newEventSystem.AddComponent<StandaloneInputModule>();

            // Instantiate the selected prefab
            newPrefab = Instantiate(selectedPrefab);
            newPrefab.name = selectedPrefab.name;

            // Set the Canvas as parent
            newPrefab.transform.SetParent(newCanvas.transform, false);
        }
        else if (selectedCanvasIndex > 0)
        {
            // Instantiate the selected prefab
            newPrefab = Instantiate(selectedPrefab);
            newPrefab.name = selectedPrefab.name;

            // Set the Canvas as parent
            newPrefab.transform.SetParent(canvasObjects[selectedCanvasIndex - 1].transform, false);

            // Optionally, you can position the prefab within the Canvas
            newPrefab.transform.localPosition = Vector3.zero;
        }

    }


}





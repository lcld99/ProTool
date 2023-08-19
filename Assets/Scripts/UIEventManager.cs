using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventManager : MonoBehaviour
{
    public IfStatement ifStatement;

    // Function to be called when the PlayButton is clicked
    public void OnPlayButtonClick()
    {
        Debug.Log("oi");
        bool result = ifStatement.Execute();
        Debug.Log("IF condition result: " + result);
    }
}

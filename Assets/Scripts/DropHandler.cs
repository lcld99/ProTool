using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{
    //public GameObject temp;
    private VariableObject variableObject;
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if(eventData.pointerDrag != null) {
            //this.temp = eventData.pointerDrag;
            variableObject = eventData.pointerDrag.transform.GetComponent<VariableObject>();
            eventData.pointerDrag.transform.position = this.transform.position;
        }
    }
}

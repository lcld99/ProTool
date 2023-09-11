using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string content;
    public string header;
    private IEnumerator cr;
    public void OnPointerEnter(PointerEventData eventData) {
        cr = Wait(0.5f);
        StartCoroutine(cr);
        //ToolTipSystem.Show(content, header); 
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopCoroutine(cr);
        ToolTipSystem.Hide(); 
    }

    public IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ToolTipSystem.Show(content, header);
    }
}

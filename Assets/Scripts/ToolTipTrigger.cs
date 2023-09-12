using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string content;
    public string header;
    private IEnumerator cr;
    private IGetToolTipInfo<string> getToolTipInfo;

    private void Awake()
    {
        getToolTipInfo = this.gameObject.GetComponent<IGetToolTipInfo<string>>();
    }

    private void Update()
    {
        if(getToolTipInfo!= null)
        {
            content = getToolTipInfo.ValueGetToolTipInfo();
        }
    }
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

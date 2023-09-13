using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IFBlockContainer : MonoBehaviour, IGetToolTipInfo<string>
{
    public TextMeshProUGUI displayInfo;
    private IGetToolTipInfo<string> getToolTipInfo;
    private string previousValue;

    //private void Awake()
    //{
    //    this.gameObject.GetComponent<ToolTipTrigger>().SetInitialContent();

    //}
    //private void Awake()
    //{
    //    previousValue = displayInfo.text;
    //}
    public void SetInititalValue(string value)
    {
        this.displayInfo.text = value;
        this.gameObject.GetComponent<ToolTipTrigger>().SetInitialContent();
    }

    public void SetValue(string value)
    {
        this.displayInfo.text = value;
    }

    public string ValueGetToolTipInfo()
    {
        return displayInfo.text;
    }
}

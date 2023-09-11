using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    private static ToolTipSystem current;

    public ToolTip tooltip;

    private void Awake()
    {
        current = this;
    }

    public static void Show(string content, string header = "")
    {
        if (current != null)
        {
            current.tooltip.SetText(content, header);
            current.tooltip.gameObject.SetActive(true);
        }
    }

    public static void Hide()
    {
        if (current != null)
        {
            current.tooltip.gameObject.SetActive(false);
        }
    }
}

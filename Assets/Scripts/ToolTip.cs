using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ToolTip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    private RectTransform rt;

    // Update is called once per frame

    private void Awake()
    {
        rt = this.gameObject.GetComponent<RectTransform>();
    }
    void Update()
    {
        if (Application.isEditor)
        {
            layoutElement.enabled = Math.Max(headerField.preferredWidth, contentField.preferredWidth) >= layoutElement.preferredWidth;

        }

        Vector2 position = Input.mousePosition;
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        float finalPivotX = 0f;
        float finalPivotY = 0f;

        if (pivotX < 0.5) //If mouse on left of screen move tooltip to right of cursor and vice vera
        {
            finalPivotX = -0.055f;
        }
        else
        {
            finalPivotX = 1.01f;
        }

        if (pivotY < 0.5) //If mouse on lower half of screen move tooltip above cursor and vice versa
        {
            finalPivotY = 0;
        }
        else
        {
            finalPivotY = 1;
        }


        rt.pivot = new Vector2(finalPivotX, finalPivotY);

        this.transform.position = position;

    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        layoutElement.enabled = Math.Max(headerField.preferredWidth, contentField.preferredWidth) >= layoutElement.preferredWidth;

    }
}

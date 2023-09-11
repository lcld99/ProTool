using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ExpandToFitText : MonoBehaviour
{
    public float padding = 2f; // Padding to add around the text
    private TextMeshProUGUI textComponent;
    private RectTransform rectTransform;
    private string previousText;

    [System.Serializable]
    public class TextChangedEvent : UnityEvent<string> { }
    public TextChangedEvent onTextChanged = new TextChangedEvent();

    void Start()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        previousText = textComponent.text;

        ExpandElement();
    }

    void Update()
    {
        if (textComponent != null)
        {
            if (previousText != textComponent.text)
            {
                previousText = textComponent.text;
                ExpandElement();
                onTextChanged.Invoke(previousText);
            }
        }
        else
        {
            Debug.LogWarning("No TextMeshPro component found in children.");
        }
    }

    void ExpandElement()
    {
        float preferredWidth = textComponent.preferredWidth + padding;
        //float preferredHeight = textComponent.preferredHeight + padding;

        rectTransform.sizeDelta = new Vector2(preferredWidth, rectTransform.sizeDelta.y);
    }
}

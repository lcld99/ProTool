using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WhileBlock : MonoBehaviour, IDropHandler, IExecutable<bool>, IHasNextExecutable<bool>, IResetable<bool>
{
    // Reference to the VariableObject script
    private RectTransform childDropArea;

    private GameObject droppedObject;
    public IFBlockContainer compareText;
    public TextMeshProUGUI operatorText;

    //private GameObject parent;
    public GameObject blockObject; // Assign the next object in the chain in the Inspector
    public GameObject nextObject; // Assign the next object in the chain in the Inspector
    private RectTransform truePathObject;
    public GameObject elseObject;
    private IExecutable<bool> currentExecutable;

    //Coroutine checks
    private bool result;
    private bool check;
    private object propertyValue;

    private string previousValueToCompare;

    // User-defined value to compare with the selected property (exposed in the Inspector)
    [SerializeField]
    private string valueToCompare;

    private LineRenderer lineRenderer;
    private LineRenderer lineRendererElsePath;
    private RectTransform rect1; //this object rect1
    private RectTransform rect2;
    private RectTransform rect3;


    public float curveAmount = 1.0f;

    public StartPosition startPosition;

    public EndPosition endPosition;

    public StartPosition startFrom = StartPosition.Top;
    public EndPosition endAt = EndPosition.Bottom;

    public object ComparisonValue
    {
        get
        {
            if (int.TryParse(valueToCompare, out int intValue))
            {
                return (float)intValue;
            }
            else if (float.TryParse(valueToCompare, out float floatValue))
            {
                return floatValue;
            }
            else
            {
                return valueToCompare;
            }
        }
    }

    [EnumDropdown] // Apply the custom attribute here
    public TypeOfCondition condition;


    public void Awake()
    {
        truePathObject = blockObject.GetComponent<RectTransform>();
        compareText.SetInititalValue(valueToCompare);
        previousValueToCompare = valueToCompare;
        childDropArea = this.transform.Find("DropArea").transform as RectTransform;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRendererElsePath = new GameObject().AddComponent<LineRenderer>();
        lineRendererElsePath.gameObject.transform.SetParent(transform, false);
        // just to be sure reset position and rotation as well
        lineRendererElsePath.gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        lineRenderer.startWidth = 0.1f; // Adjust the width here
        lineRenderer.endWidth = 0.1f; // Adjust the width here
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        if (elseObject != null)
        {
            lineRendererElsePath.startWidth = 0.1f; // Adjust the width here
            lineRendererElsePath.endWidth = 0.1f; // Adjust the width here
            lineRendererElsePath.startColor = Color.red;
            lineRendererElsePath.endColor = Color.red;
        }
    }

    void Update()
    {
        // Update the TextMeshPro component's text property with the current "life" value
        if (previousValueToCompare != valueToCompare)
        {
            compareText.SetValue(valueToCompare);
            previousValueToCompare = valueToCompare;
        }

        operatorText.text = GetOperatorSymbol(condition);
        if (this != null && blockObject != null)
        {
            rect1 = this.GetComponent<RectTransform>();
            rect2 = truePathObject;


            Vector3 p1 = UtilityFunctions.GetStartingPosition(startFrom, rect1);
            Vector3 p2 = UtilityFunctions.GetEndingPosition(endAt, rect2);
            Vector3[] points = UtilityFunctions.GenerateLinkPath(p1, p2, curveAmount);
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);

            if (elseObject != null)
            {
                rect3 = elseObject.GetComponent<RectTransform>();
                Vector3 p3 = UtilityFunctions.GetEndingPosition(endAt, rect3);
                Vector3[] points2 = UtilityFunctions.GenerateLinkPath(p1, p3, curveAmount);
                lineRendererElsePath.positionCount = points2.Length;
                lineRendererElsePath.SetPositions(points2);

            }

        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        // Check if the drop position is within the child's boundaries
        //Debug.Log(childDropArea);

        //Vector2 dropPosition = eventData.pointerDrag.transform.position;
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(childDropArea, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            // Handle the OnDrop event logic
            //Debug.Log("dropped");

            // Get the dropped object's data (assuming your dropped objects have a script)
            //droppedObject = eventData.pointerDrag.GetComponent<VariableObject>();

            if (childDropArea.rect.Contains(localPointerPosition))
            {
                droppedObject = eventData.pointerDrag;
                if (droppedObject != null)
                {
                    droppedObject.transform.SetParent(this.transform, true);
                    Draggable temp = eventData.pointerDrag.GetComponent<Draggable>();
                    //temp.SetChildDropArea(childDropArea);
                    // Process the data from the dropped object
                    if (temp.GetDropArea() == null)
                    {
                        temp.SetDropArea(childDropArea);
                        temp.SetGameObject(this.gameObject);
                        RectTransform droppedObjectRect = droppedObject.GetComponent<RectTransform>();
                        Vector3 scale = this.GetComponent<RectTransform>().localScale;
                        childDropArea.sizeDelta = new Vector2(Mathf.Clamp(droppedObjectRect.sizeDelta.x, 40.0f, 60.0f), droppedObjectRect.sizeDelta.y);
                        droppedObjectRect.sizeDelta = new Vector2(Mathf.Clamp(droppedObjectRect.sizeDelta.x, 40.0f, 60.0f), droppedObjectRect.sizeDelta.y);
                    }

                    droppedObject.transform.position = childDropArea.position;



                    //Debug.Log("Received data from dropped object: " + droppedObject.GetComponent<VariableObject>().GetPropertyValue());
                }

            }

        }
    }

    // Function to check the IF condition
    public bool Execute()
    {
        if (droppedObject != null)
        {
            propertyValue = droppedObject.GetComponent<VariableObject>().GetPropertyValue();
            if (propertyValue != null)
            {
                check = ResolveTypeBool(propertyValue, ComparisonValue, condition);
                Debug.Log(check);
                StartCoroutine(WhileLoop((boolValue) => {
                    result = boolValue; // Assign the result to the variable
                    Debug.Log("Coroutine completed with result: " + boolValue);
                    // Continue with the rest of the code that depends on the coroutine result
                    // ...
                    if(result) {
                        Debug.Log("Passou");
                    }
                }));
                nextObject = elseObject;
                return true;
            }
            else
            {
                throw new InvalidOperationException("Invalid property value or property is not of type 'int'!");
            }

        }
        else
        {
            throw new InvalidOperationException("Variable has not been assigned");
        }
    }    

    IEnumerator WhileLoop(Action<bool> callback)
    {
        Debug.Log("Co: " + check);
        while (check)
        {
            // Execute your loop body code here
            currentExecutable = GetNextExecutableWithinBlock();
            UtilityFunctions.ExecuteNext(currentExecutable);
            check = ResolveTypeBool(propertyValue, ComparisonValue, condition);
            // Simulate a yield for the next frame
            yield return null;
        }

        // Once the condition is false, continue with the rest of the code
        callback.Invoke(check);
    }


    public void ExecuteNext()
    {
        // Execute the current object's logic
        if (currentExecutable != null)
        {
            bool shouldProceed = currentExecutable.Execute();

            // Check if the current executable implements IHasNextExecutable<bool>
            if (shouldProceed && currentExecutable is IHasNextExecutable<bool> nextExecutable)
            {
                IExecutable<bool> nextObject = nextExecutable.GetNextExecutable();
                if (nextObject != null)
                {
                    currentExecutable = nextObject;
                    ExecuteNext();
                }
            }
            else if (!shouldProceed)
            {
                throw new InvalidOperationException("Condition failed");
            }
        }


    }

    public bool ResolveTypeBool(object propertyValue, object comparisonValue, TypeOfCondition type)
    {
        switch (type)
        {
            case TypeOfCondition.GreaterThan:
                return CompareValues(propertyValue, comparisonValue) > 0;
            case TypeOfCondition.LessThan:
                return CompareValues(propertyValue, comparisonValue) < 0;
            case TypeOfCondition.Equal:
                return CompareValues(propertyValue, comparisonValue) == 0;
            case TypeOfCondition.GreaterThanOrEqual:
                return CompareValues(propertyValue, comparisonValue) >= 0;
            case TypeOfCondition.LessThanOrEqual:
                return CompareValues(propertyValue, comparisonValue) <= 0;

            // Add more cases for other conditions

            default:
                throw new ArgumentException("Invalid TypeOfCondition value");
        }
    }

    private int CompareValues(object value1, object value2)
    {
        if (value1 is int)
        {
            value1 = (float)(int)value1;
        }

        if (value1 is int intValue1 && value2 is float intValue2)
        {
            return intValue1.CompareTo(intValue2);
        }
        else if (value1 is float floatValue1 && value2 is float floatValue2)
        {
            return floatValue1.CompareTo(floatValue2);
        }
        else if (value1 is string stringValue1 && value2 is string stringValue2)
        {
            return stringValue1.CompareTo(stringValue2);
        }
        // Add more cases for other types

        throw new ArgumentException("Unsupported value types");
    }

    public bool ResetValue()
    {
        if (droppedObject != null)
        {
            droppedObject.transform.SetParent(this.transform.parent, true);
            droppedObject = null;
            return true;
        }
        return false;
    }

    public IExecutable<bool> GetNextExecutable()
    {
        if (nextObject != null)
        {
            return nextObject.GetComponent<IExecutable<bool>>();
        }
        return null;
    }

    public IExecutable<bool> GetNextExecutableWithinBlock()
    {
        if (blockObject != null)
        {
            return blockObject.GetComponent<IExecutable<bool>>();
        }
        return null;
    }

    string GetOperatorSymbol(TypeOfCondition condition)
    {
        switch (condition)
        {
            case TypeOfCondition.GreaterThan:
                return ">";
            case TypeOfCondition.LessThan:
                return "<";
            case TypeOfCondition.Equal:
                return "=";
            case TypeOfCondition.GreaterThanOrEqual:
                return "≥";
            case TypeOfCondition.LessThanOrEqual:
                return "≤";
            default:
                return "";
        }
    }

}




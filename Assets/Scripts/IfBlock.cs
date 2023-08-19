using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class IfStatement : MonoBehaviour, IDropHandler, IExecutable<bool>, IHasNextExecutable<bool>, IResetable<bool>
{
    // Reference to the VariableObject script
    private RectTransform childDropArea;

    public GameObject droppedObject;

    //public VariableObject droppedObject;

    //private GameObject parent;
    public GameObject nextObject; // Assign the next object in the chain in the Inspector

    // User-defined value to compare with the selected property (exposed in the Inspector)
    [SerializeField]
    private string valueToCompare;

    public object ComparisonValue
    {
        get
        {
            if (int.TryParse(valueToCompare, out int intValue))
            {
                return (float) intValue;
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
        childDropArea = this.transform.GetChild(0).transform as RectTransform;
    }

    //public void Update()
    //{
    //    if(droppedObject != null && !IsImageWithinArea(droppedObject.transform as RectTransform, childDropArea))
    //    {
    //        droppedObject = null;
    //        Debug.Log("Reseted");
    //    }
    //}

    //private bool IsImageWithinArea(RectTransform draggedObject, RectTransform DragArea)
    //{
    //    RectTransform rectTransformA = draggedObject;
    //    RectTransform rectTransformB = DragArea;

    //    // Calculate the boundaries of both images
    //    Rect rectA = new Rect(rectTransformA.position.x, rectTransformA.position.y, rectTransformA.rect.width, rectTransformA.rect.height);
    //    Rect rectB = new Rect(rectTransformB.position.x, rectTransformB.position.y, rectTransformB.rect.width, rectTransformB.rect.height);

    //    // Check if the bounds of imageToCheck are within the bounds of areaImage
    //    return rectB.Contains(rectA.min) && rectB.Contains(rectA.max);
    //}

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
                    Draggable temp = eventData.pointerDrag.GetComponent<Draggable>();
                    temp.SetDropArea(childDropArea);
                    temp.SetGameObject(this.gameObject);
                    temp.SetChildDropArea(childDropArea);
                    // Process the data from the dropped object
                    eventData.pointerDrag.transform.position = childDropArea.position;
                    Debug.Log("Received data from dropped object: " + droppedObject.GetComponent<VariableObject>().GetPropertyValue());
                }

            }

        }
    }

    // Function to check the IF condition
    public bool Execute()
    {
        if(droppedObject != null)
        {
            object propertyValue = droppedObject.GetComponent<VariableObject>().GetPropertyValue();
            if (propertyValue != null)
            {
                return ResolveTypeBool(propertyValue, ComparisonValue, condition);                                 
            }
            else
            {
                throw new InvalidOperationException ("Invalid property value or property is not of type 'int'!");
            }

        }
        else
        {
            throw new InvalidOperationException("Variable has not been assigned");
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
        if(value1 is int)
        {
            value1 = (float) (int) value1;
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

    //public GameObject GetNextObject()
    //{
    //    return nextObject;
    //}

}




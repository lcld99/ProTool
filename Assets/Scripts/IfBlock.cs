using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class IfStatement : MonoBehaviour, IDropHandler, IExecutable<bool>, IHasNextExecutable<bool>, IResetable<bool>
{
    // Reference to the VariableObject script
    private RectTransform childDropArea;

    private GameObject droppedObject;
    public TextMeshProUGUI compareText;
    public TextMeshProUGUI operatorText;

    //private GameObject parent;
    public GameObject nextObject; // Assign the next object in the chain in the Inspector

    // User-defined value to compare with the selected property (exposed in the Inspector)
    [SerializeField]
    private string valueToCompare;

    private LineRenderer lineRenderer;
    public float curveAmount = 1.0f;
    public enum StartPosition
    {
        Top,
        Right,
        Bottom,
        Left
    }

    public enum EndPosition
    {
        Top,
        Right,
        Bottom,
        Left
    }
    public StartPosition startFrom = StartPosition.Top;
    public EndPosition endAt = EndPosition.Bottom;

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
        childDropArea = this.transform.Find("DropArea").transform as RectTransform;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f; // Adjust the width here
        lineRenderer.endWidth = 0.1f; // Adjust the width here
    }

    void Update()
    {
        // Update the TextMeshPro component's text property with the current "life" value
        compareText.text = valueToCompare;
        operatorText.text = GetOperatorSymbol(condition);
        if (this != null && nextObject != null)
        {
            RectTransform rect1 = this.GetComponent<RectTransform>();
            RectTransform rect2 = nextObject.GetComponent<RectTransform>();

            Vector3 p1 = GetStartingPosition(rect1);
            Vector3 p2 = GetEndingPosition(rect2);

            Vector3[] points = GenerateLinkPath(p1, p2, curveAmount);

            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }

    private Vector3[] GenerateLinkPath(Vector3 p1, Vector3 p2, float scale)
    {
        Vector3 delta = p2 - p1;
        float dy = delta.y;
        float dx = delta.x;
        float nodeWidth = 10.0f; // Adjust this based on your node width
        float nodeHeight = 10.0f; // Adjust this based on your node height

        float scaleY = 0;

        float deltaLength = Mathf.Sqrt(dy * dy + dx * dx);
        float scaleModifier = 1.0f;

        if (dx * scaleModifier > 0)
        {
            if (deltaLength < nodeWidth)
            {
                scale = 0.75f - 0.75f * ((nodeWidth - deltaLength) / nodeWidth);
            }

            // Construct the path string using cubic Bezier curves
            Vector3 cp1 = p1 + new Vector3(scaleModifier * nodeWidth * scale, scaleY * nodeHeight, 0);
            Vector3 cp2 = p2 - new Vector3(scaleModifier * scale * nodeWidth, scaleY * nodeHeight, 0);

            return BezierCurve(p1, cp1, cp2, p2);
        }
        else
        {
            // Handle case where the wire has to loop back on itself
            float midX = Mathf.Floor((p2.x - dx / 2));
            float midY = Mathf.Floor((p2.y - dy / 2));

            if (dy == 0)
            {
                midY = p2.y + nodeHeight;
            }

            float cpHeight = nodeHeight / 2;
            float y1 = (p2.y + midY) / 2;
            float topX = p1.x + scaleModifier * nodeWidth * scale;
            float topY = dy > 0 ? Mathf.Min(y1 - dy / 2, p1.y + cpHeight) : Mathf.Max(y1 - dy / 2, p1.y - cpHeight);
            float bottomX = p2.x - scaleModifier * scale * nodeWidth;
            float bottomY = dy > 0 ? Mathf.Max(y1, p2.y - cpHeight) : Mathf.Min(y1, p2.y + cpHeight);
            float x1 = (p1.x + topX) / 2;
            float scy = dy > 0 ? 1 : -1;

            Vector3[] cp = new Vector3[]
            {
            // Orig -> Top
            new Vector3(x1, p1.y, 0),
            new Vector3(topX, dy > 0 ? Mathf.Max(p1.y, topY - cpHeight) : Mathf.Min(p1.y, topY + cpHeight), 0),
            // Top -> Mid
            new Vector3(x1, dy > 0 ? Mathf.Min(midY, topY + cpHeight) : Mathf.Max(midY, topY - cpHeight), 0),
            // Mid -> Bottom
            new Vector3(bottomX, dy > 0 ? Mathf.Max(midY, bottomY - cpHeight) : Mathf.Min(midY, bottomY + cpHeight), 0),
            // Bottom -> Dest
            new Vector3((p2.x + bottomX) / 2, p2.y, 0)
            };

            if (cp[2].y == topY + scy * cpHeight)
            {
                if (Mathf.Abs(dy) < cpHeight * 10)
                {
                    cp[1].y = topY - scy * cpHeight / 2;
                    cp[3].y = bottomY - scy * cpHeight / 2;
                }
                cp[2].x = topX;
            }

            Vector3[] curve1 = BezierCurve(p1, cp[0], cp[1], cp[2]);
            Vector3[] curve2 = BezierCurve(cp[2], cp[3], cp[4], p2);

            Vector3[] combined = new Vector3[curve1.Length + curve2.Length - 1];
            curve1.CopyTo(combined, 0);
            curve2.CopyTo(combined, curve1.Length - 1);

            return combined;
        }
    }



    private Vector3[] BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int segments = 20;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float oneMinusT = 1 - t;

            points[i] = oneMinusT * oneMinusT * oneMinusT * p0 +
                        3 * oneMinusT * oneMinusT * t * p1 +
                        3 * oneMinusT * t * t * p2 +
                        t * t * t * p3;
        }

        return points;
    }

    //private void OnDrawGizmos()
    //{
    //    if (this != null && nextObject != null)
    //    {
    //        RectTransform rect1 = this.GetComponent<RectTransform>();
    //        RectTransform rect2 = nextObject.GetComponent<RectTransform>();

    //        Vector3 p1 = GetStartingPosition(rect1);
    //        Vector3 p2 = GetEndingPosition(rect2);

    //        Vector3 dir = p2 - p1;
    //        Vector3 midPoint = p1 + 0.5f * dir;
    //        Vector3 normal = new Vector3(-dir.y, dir.x, 0).normalized;

    //        Vector3 controlPoint = midPoint + curveAmount * normal;

    //        Gizmos.color = Color.blue;

    //        int segments = 20;
    //        float step = 1.0f / segments;

    //        for (int i = 0; i < segments; i++)
    //        {
    //            float t1 = i * step;
    //            float t2 = (i + 1) * step;

    //            Vector3 segmentStart = CalculateBezierPoint(p1, controlPoint, p2, t1);
    //            Vector3 segmentEnd = CalculateBezierPoint(p1, controlPoint, p2, t2);

    //            Gizmos.DrawLine(segmentStart, segmentEnd);
    //        }
    //    }
    //}

    private Vector3 GetStartingPosition(RectTransform rectTransform)
    {
        Vector3 center = rectTransform.rect.center;

        switch (startFrom)
        {
            case StartPosition.Top:
                return rectTransform.TransformPoint(center + Vector3.up * rectTransform.rect.height / 2);
            case StartPosition.Right:
                return rectTransform.TransformPoint(center + Vector3.right * rectTransform.rect.width / 2);
            case StartPosition.Bottom:
                return rectTransform.TransformPoint(center + Vector3.down * rectTransform.rect.height / 2);
            case StartPosition.Left:
                return rectTransform.TransformPoint(center + Vector3.left * rectTransform.rect.width / 2);
            default:
                return rectTransform.position;
        }
    }

    private Vector3 GetEndingPosition(RectTransform rectTransform)
    {
        Vector3 center = rectTransform.rect.center;

        switch (endAt)
        {
            case EndPosition.Top:
                return rectTransform.TransformPoint(center + Vector3.up * rectTransform.rect.height / 2);
            case EndPosition.Right:
                return rectTransform.TransformPoint(center + Vector3.right * rectTransform.rect.width / 2);
            case EndPosition.Bottom:
                return rectTransform.TransformPoint(center + Vector3.down * rectTransform.rect.height / 2);
            case EndPosition.Left:
                return rectTransform.TransformPoint(center + Vector3.left * rectTransform.rect.width / 2);
            default:
                return rectTransform.position;
        }
    }

    //private Vector3 CalculateBezierPoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    //{
    //    float u = 1 - t;
    //    float tt = t * t;
    //    float uu = u * u;
    //    float uuu = uu * u;
    //    float ttt = tt * t;

    //    Vector3 p = uuu * p1;
    //    p += 3 * uu * t * p2;
    //    p += 3 * u * tt * p3;
    //    p += ttt * p3;

    //    return p;
    //}

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


    //public GameObject GetNextObject()
    //{
    //    return nextObject;
    //}

}




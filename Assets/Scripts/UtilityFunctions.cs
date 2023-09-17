using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IfStatement;

public static class UtilityFunctions
{
    public static Vector3[] GenerateLinkPath(Vector3 p1, Vector3 p2, float scale)
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

    public static Vector3[] BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
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

    public static Vector3 GetStartingPosition(StartPosition startFrom, RectTransform rectTransform)
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

    public static Vector3 GetEndingPosition(EndPosition endAt, RectTransform rectTransform)
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

    public static void ExecuteNext(IExecutable<bool> currentExecutable)
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
                    ExecuteNext(currentExecutable);
                }
            }
            else if (!shouldProceed)
            {
                throw new InvalidOperationException("Condition failed");
            }
        }


    }
}

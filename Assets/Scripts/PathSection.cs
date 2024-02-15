using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public abstract class PathSection
{
    protected Vector3 startPoint;
    protected Vector3 endPoint;
    protected float playerHeight;
    public abstract Vector3 GetPositionOnPath(float d);
}

public class StraightPathSection : PathSection
{
    public StraightPathSection(Vector3 startPoint, Vector3 endPoint, float height)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.playerHeight = height;
    }

    public override Vector3 GetPositionOnPath(float d)
    {
        Vector3 dir = endPoint - startPoint;

        float xVal = startPoint.x + (d % 1) * dir.x;
        float yVal = startPoint.z + (d % 1) * dir.z;

        return new Vector3(xVal, playerHeight, yVal);
    }
}

public class CurvedPathSection: PathSection
{
    Vector3 midPoint;

    float startAngle;
    float endAngle;
    float angleChange;
    Vector2 circleCenter;
    float radius;

    public CurvedPathSection(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float height)
    {
        this.startPoint = startPoint;
        this.midPoint = midPoint;
        this.endPoint = endPoint;
        this.playerHeight = height;

        Vector2 startPoint2d = new Vector2(startPoint.x, startPoint.z);
        Vector2 midPoint2d = new Vector2(midPoint.x, midPoint.z);
        Vector2 endPoint2d = new Vector2(endPoint.x, endPoint.z);

        Vector2 line1Direction = midPoint2d - startPoint2d;
        Vector2 line2Direction = endPoint2d - midPoint2d;

        //get tangent vectors
        Vector2 line1AntiClockwisePerp = new Vector2(-line1Direction.y, line1Direction.x);
        Vector2 line2AntiClockwisePerp = new Vector2(-line2Direction.y, line2Direction.x);
        Vector2 line1ClockwisePerp = new Vector2(line1Direction.y, -line1Direction.x);
        Vector2 line2ClockwisePerp = new Vector2(line2Direction.y, -line2Direction.x);

        Vector2 intermediate1 = (startPoint2d + midPoint2d) / 2;
        Vector2 intermediate2 = (midPoint2d + endPoint2d) / 2;

        circleCenter = GetLineIntersection(line1AntiClockwisePerp, intermediate1, line2AntiClockwisePerp, intermediate2);
        radius = (intermediate1 - circleCenter).magnitude;

        float intermediate1Angle = AngleWith(Vector2.up, line1ClockwisePerp);
        float intermediate2Angle = AngleWith(Vector2.up, line2ClockwisePerp);
        /*Debug.Log("Angle1: " + intermediate1Angle * 180 / Mathf.PI);
        Debug.Log("Angle2: " + intermediate2Angle * 180 / Mathf.PI);*/

        startAngle = intermediate2Angle % (Mathf.PI * 2);
        endAngle = intermediate1Angle % (Mathf.PI * 2);
        Debug.Log("Angle1: " + startAngle);
        Debug.Log("Angle2: " + endAngle);


        // If left turn, fix corner being on wrong side of circle
        Debug.Log((AngleWith(line1ClockwisePerp, line2ClockwisePerp) + Mathf.PI * 2) % (Mathf.PI * 2));
        if ((AngleWith(line1ClockwisePerp, line2ClockwisePerp) + Mathf.PI * 2) % (Mathf.PI * 2) < Mathf.PI)
        {
            startAngle = (startAngle + Mathf.PI) % (Mathf.PI * 2);
            endAngle = (endAngle + Mathf.PI) % (Mathf.PI * 2);
        }

        angleChange = endAngle - startAngle;

        //If angle is too big. make it small and negative
        if (angleChange > Mathf.PI)
        {
            angleChange = -(Mathf.PI * 2 - angleChange);
        }
        else if (angleChange < -Mathf.PI)
        {
            angleChange = (Mathf.PI * 2 + angleChange);
        }
    }

    float AngleWith(Vector2 vec1, Vector2 vec2)
    {
        return (Mathf.Atan2(vec1.y, vec1.x) - Mathf.Atan2(vec2.y, vec2.x)) % (Mathf.PI * 2);
    }

    Vector2 GetLineIntersection(Vector2 line1_direction, Vector2 line1Point, Vector2 line2_direction, Vector2 line2Point)
    {
        // Normalize line directions
        line1_direction.Normalize();
        line2_direction.Normalize();
        Vector2 line1Normal = new Vector2(-line1_direction.y, line1_direction.x);
        Vector2 line2Normal = new Vector2(-line2_direction.y, line2_direction.x);

        // Rewrite lines to general form: Ax + By = k1; Cx + Dy = k2
        // Normal vectors is A, B
        float A = line1Normal.x;
        float B = line1Normal.y;
        
        float C = line2Normal.x;
        float D = line2Normal.y;

        // Use one point on line to get k
        float k1 = (A * line1Point.x) + (B * line1Point.y);
        float k2 = (C * line2Point.x) + (D * line2Point.y);

        //check for parallel or same line
        if (Vector2.Angle(line1Normal, line2Normal) == 0f || Vector2.Angle(line2Normal, line1Normal) == 180f) 
            //is parallel
            return Vector2.zero;

        if (Mathf.Abs(Vector2.Dot(line1Point-line2Point, line1Normal)) < 0.000001f)
            //is orthogonal
            return Vector2.zero;


        float x_intersect = (D * k1 - B * k2) / (A * D - B * C);
        float y_intersect = (-C * k1 + A * k2) / (A * D - B * C);

        return new Vector2(x_intersect, y_intersect);
    }

    public override Vector3 GetPositionOnPath(float d)
    {
        float angle = startAngle + angleChange * (d % 1);
        Vector2 point = circleCenter + new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
        return new Vector3(point.x, playerHeight, point.y);
    }
}
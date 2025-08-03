using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Formula
{
    public static float Positive(float value)
    {
        return value < 0 ? value * -1 : value;
    }
    public static float InMinMax(float min, float max, float cursor)
    {
        return Mathf.Lerp(min, max, cursor);
    }
    public static float ExcessToBend(float a, float b, float clamp)
    {
        float full = a + b;
        if (full > clamp)
        {
            float excess = (full - clamp);
            float result = clamp - excess;
            return result;
        }
        else
        {
            return full;
        }
    }
    public static bool InRadian(float limit, float cursor1, float cursor2)
    {
        float additional_1 = cursor1 + limit / 2; // 363
        float additional_2 = cursor2 + limit / 2; // 13
        if (additional_1 > 360)
        {
            float excess = additional_1 - 360; //3
            if (cursor2 < excess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (additional_2 > 360)
        {
            float excess = additional_2 - 360; //
            if (cursor1 < excess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            float excess = Mathf.Abs(cursor1 - cursor2);
            if (excess <= limit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    static float AddCycle(float a, float b, float clamp)
    {
        clamp = 360;
        float additional = a + b;
        if (additional > clamp)
        {
            float excess = additional - clamp;
            return excess;
        }
        else
        {
            return additional;
        }
    }

    public static Vector3 RelativeToParent(Vector3 child, Vector3 parent)
    {
        Vector3 result = new Vector3(child.x + parent.x, child.y + parent.y, child.z + parent.z);
        return result;
    }

    public static float NormalizedByRange(float value, float minValue,float maxValue)
    {
        return Mathf.Clamp01((value - minValue) / (maxValue - minValue));
    }
}

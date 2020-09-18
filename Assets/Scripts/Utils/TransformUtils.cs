using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtils
{
    public static bool RectTransformContainsMouse(RectTransform rectTransform)
    {
        Vector2 mousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return rectTransform.rect.Contains(mousePosition);
    }
}

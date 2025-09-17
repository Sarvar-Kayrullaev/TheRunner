using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAdapter : MonoBehaviour
{
    private TMP_Text text;
    void Awake()
    {
        if (TryGetComponent(out TMP_Text text)) this.text = text;
    }

    public void FixParentAspect(TMP_Text tmpText)
    {
        var preferredSize = tmpText.GetPreferredValues();
        var preferredWidth = preferredSize.x;
        var heightPadding = 10;
        var preferredHeight = preferredSize.y + heightPadding;
        if (tmpText.transform.parent.TryGetComponent(out RectTransform parent))
        {
            parent.sizeDelta = new Vector2(parent.sizeDelta.x, preferredHeight);
        }
    }

    public void IntegerToText(int value)
    {
        if(text) text.text = "" + value;
        else if (TryGetComponent(out TMP_Text text)) text.text = ""+value;
    }

    public void FloatToText(float value)
    {
        if(text) text.text = "" + value;
        else if (TryGetComponent(out TMP_Text text)) text.text = ""+value;
    }

    public void BooleanToText(bool value)
    {
        if(text) text.text = "" + value;
        else if (TryGetComponent(out TMP_Text text)) text.text = ""+value;
    }

    public void SetText(string value)
    {
        if(text) text.text = value;
        else if (TryGetComponent(out TMP_Text text)) text.text = value;
    }
}

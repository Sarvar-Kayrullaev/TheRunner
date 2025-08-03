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

    public void IntegetToText(int value)
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

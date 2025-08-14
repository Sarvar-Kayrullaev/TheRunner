using System;
using Code.Scripts.Data.Language;
using Code.Scripts.GUI.Adapter;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LanguageAdapter : MonoBehaviour
{
    [SerializeField] private FontType fontType;
    [SerializeField] private string key;
    
    FontAdapter fontAdapter;
    private TMP_Text text;
    private void Awake()
    {
        if (text != null) return;
        if (TryGetComponent(out TMP_Text t)) text = t;
        fontAdapter = FindFirstObjectByType<FontAdapter>();
    }

    private void Start()
    {
        if(text == null || fontAdapter == null) return;
        text.font = fontAdapter.GetFont(fontType, out var languageCode);
        text.text = Language.Get(key, languageCode);
    }
}

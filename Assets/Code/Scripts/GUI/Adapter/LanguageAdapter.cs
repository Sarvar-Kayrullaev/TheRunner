using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Data.Language;
using Code.Scripts.GUI.Adapter;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LanguageAdapter : MonoBehaviour
{
    [SerializeField] private FontType fontType;
    [SerializeField] private FontStyle fontStyle;
    [SerializeField] private string key;

    private FontAdapter _fontAdapter;
    private TMP_Text _text;
    private void Awake()
    {
        if (TryGetComponent(out TMP_Text t)) _text = t;
        _fontAdapter = FindFirstObjectByType<FontAdapter>();
    }

    private void Start()
    {
        StartCoroutine(LoadFontAndSetText());
    }
    
    private IEnumerator LoadFontAndSetText()
    {
        if (!_text || !_fontAdapter) yield break;

        TMP_FontAsset requiredFont = _fontAdapter.GetFont(fontType,fontStyle, out var languageCode);

        while (!requiredFont)
        {
            yield return null; 
            requiredFont = _fontAdapter.GetFont(fontType, fontStyle, out languageCode);
        }

        // Once the font is loaded, continue
        _text.font = requiredFont;
        _text.text = Language.Get(key, languageCode);
        
        Debug.Log("");
    }
}

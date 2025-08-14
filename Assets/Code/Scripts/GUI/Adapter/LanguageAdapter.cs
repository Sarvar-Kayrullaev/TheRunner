using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LanguageAdapter : MonoBehaviour
{

    [SerializeField] private string key;
    private TMP_Text text;
    private void Awake()
    {
        if (text != null) return;
        if (TryGetComponent(out TMP_Text t))
        {
            this.text = t;
        }
    }

    private void Start()
    {
        if (text != null)
        {
            text.text = Language.Get(key, Language.LanguageCode.Korean);
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupToast : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;
    
    
    private string _message;
    private Sprite _sprite;

    public void Initialize(string message, Sprite sprite)
    {
        _message = message;
        _sprite = sprite;
        gameObject.SetActive(false);
    }

    public void Play(float duration)
    {
        text.text = _message;
        image.sprite = _sprite;
        gameObject.SetActive(true);
        var preferredSize = text.GetPreferredValues();
        var preferredWidth = preferredSize.x;
        text.rectTransform.sizeDelta = new Vector2(preferredWidth, text.rectTransform.sizeDelta.y);
        text.transform.parent.TryGetComponent(out RectTransform rectTransform);
        rectTransform.sizeDelta = new Vector2(preferredWidth + image.rectTransform.rect.width+5, rectTransform.sizeDelta.y);
        StartCoroutine(Destroy(duration));
    }
    
    private IEnumerator Destroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
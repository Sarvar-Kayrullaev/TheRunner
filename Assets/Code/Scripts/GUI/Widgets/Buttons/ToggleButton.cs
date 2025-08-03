using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

namespace Widget
{
    public class ToggleButton : MonoBehaviour, IPointerClickHandler
    {
        [Header("Setup")]
        [SerializeField] RectTransform handler;
        [SerializeField] TMP_Text text;

        [Space]
        [Header("State Color")]
        [SerializeField] Color enabledColor;
        [SerializeField] Color disabledColor;

        [Space(3)]
        public UnityEvent<bool> OnChanged;

        [HideInInspector] public bool Enabled;

        private RectTransform rectTransform;
        private Image image;

        void Awake()
        {
            if (TryGetComponent(out RectTransform rectTransform)) this.rectTransform = rectTransform;
            if (handler.TryGetComponent(out Image image)) this.image = image;
        }

        public void Build(bool value)
        {
            if (TryGetComponent(out RectTransform rectTransform)) this.rectTransform = rectTransform;
            if (handler.TryGetComponent(out Image image)) this.image = image;
            ChangeValue(value);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ChangeValue(!Enabled);
        }

        public void ChangeValue(bool enabled)
        {
            Enabled = enabled;
            OnChanged.Invoke(Enabled);
            float weight = rectTransform.sizeDelta.x;
            float height = rectTransform.sizeDelta.y;
            float handlerWeight = weight / 2;
            float handlerPosition = handlerWeight / 2;
            float offsetHeight = 0;
            if (enabled)
            {
                handler.sizeDelta = new Vector2(handlerWeight, height + offsetHeight);
                handler.anchoredPosition = new Vector2(handlerPosition, offsetHeight / 2);
                if (image) image.color = enabledColor;
                if (text) text.text = "On";
            }
            else
            {
                handler.sizeDelta = new Vector2(handlerWeight, height + offsetHeight);
                handler.anchoredPosition = new Vector2(-handlerPosition, offsetHeight / 2);
                if (image) image.color = disabledColor;
                if (text) text.text = "Off";
            }
        }
    }

}

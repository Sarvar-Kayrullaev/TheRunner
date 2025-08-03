using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Widget
{
    public class ElectorBar : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] Sprite selectedSprite;
        [SerializeField] Sprite unselectedSprite;
        [Header("Background Color")]
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color unselectedColor;
        [Header("Text Color")]
        [SerializeField] private Color selectedTextColor;
        [SerializeField] private Color unselectTextedColor;

        public UnityEvent<int> OnChanged;

        private CanvasGroup canvasGroup;
        void Awake()
        {
            if (TryGetComponent(out CanvasGroup canvasGroup)) this.canvasGroup = canvasGroup;
            Initialize();
        }

        void Initialize()
        {
            foreach (Transform item in transform)
            {
                if (item.TryGetComponent(out ElectorButton button))
                {
                    button.OnClick.AddListener(() => Select(item.GetSiblingIndex()));
                }
            }
        }

        public void Build(int index)
        {
            Select(index);
        }

        void Select(int selectedIndex)
        {
            int index = 0;
            foreach (Transform item in transform)
            {
                Image image = item.GetComponent<Image>();
                ElectorButton button = item.GetComponent<ElectorButton>();
                TMP_Text text = item.GetComponentInChildren<TMP_Text>();
                if (index == selectedIndex)
                {
                    Changed(index);
                    if (image) image.color = selectedColor;
                    if (image) image.sprite = selectedSprite;
                    if (text) text.color = selectedTextColor;
                    if (button) button.Selected = true;
                }
                else
                {
                    if (image) image.color = unselectedColor;
                    if (image) image.sprite = unselectedSprite;
                    if (text) text.color = unselectTextedColor;
                    if (button) button.Selected = false;
                }
                index++;
            }
        }

        void Changed(int index)
        {
            OnChanged?.Invoke(index);
        }

        public void Active(bool enabled)
        {
            if (TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = enabled ? 1 : 0.5f;
                canvasGroup.interactable = enabled;
                canvasGroup.blocksRaycasts = enabled;
            }
        }
    }
}


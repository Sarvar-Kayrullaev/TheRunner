using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Widget
{
    public class Button : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Range(0,1)]
        public float pressedOpacity = 0.8f;

        [Space(3)]
        public UnityEvent OnClick;
        public UnityEvent OnDown;
        public UnityEvent OnUp;

        private Image image;
        private void Awake()
        {
            if(TryGetComponent(out Image image)) this.image = image;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDown?.Invoke();
            if(image) image.color = new Color(image.color.r, image.color.g, image.color.b, pressedOpacity);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnUp?.Invoke();
            if(image) image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }
    }
}


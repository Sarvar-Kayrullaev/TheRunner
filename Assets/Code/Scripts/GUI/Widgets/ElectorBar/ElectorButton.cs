using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Widget
{
    public class ElectorButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Range(0, 1)]
        public float pressedOpacity = 0.8f;
        public float selectedScale = 1.2f;

        [Space(3)]
        public UnityEvent OnClick;
        public UnityEvent OnDown;
        public UnityEvent OnUp;

        [HideInInspector] public bool Selected = false;
        private Image image;
        private void Awake()
        {
            if (TryGetComponent(out Image image)) this.image = image;
        }

        private void Update()
        {
            if (Selected) transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(selectedScale,selectedScale,selectedScale),5 * Time.deltaTime);
            else transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(1,1,1), 5 * Time.deltaTime);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDown?.Invoke();
            if (image) image.color = new Color(image.color.r, image.color.g, image.color.b, pressedOpacity);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnUp?.Invoke();
            if (image) image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }
    }
}


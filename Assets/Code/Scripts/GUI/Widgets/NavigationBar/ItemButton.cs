using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Widget
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ItemButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Range(0, 1)]
        public float pressedOpacity = 0.8f;

        [Header("Scaling")]
        public float pressedScale = 0.9f;
        public float selectedScale = 1.1f;

        [Space(3)]
        public UnityEvent OnClick;
        public UnityEvent OnDown;
        public UnityEvent OnUp;
        public UnityEvent OnExit;

        [HideInInspector] public bool Selected = false;
        [HideInInspector] public bool Pressed = false;

        private CanvasGroup canvasGroup;
        private void Awake()
        {
            if (TryGetComponent(out CanvasGroup canvasGroup)) this.canvasGroup = canvasGroup;
        }

        private void Update()
        {
            if (Selected)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(selectedScale, selectedScale, selectedScale), 5 * Time.deltaTime);
            }
            else
            {
                if (Pressed) return; // If pressed, do not scale back to normal
                transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(1, 1, 1), 5 * Time.deltaTime);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDown?.Invoke();
            Pressed = true;
            if (canvasGroup) canvasGroup.alpha = pressedOpacity;
            if (!Selected) transform.localScale = new Vector3(pressedScale, pressedScale, pressedScale);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnUp?.Invoke();
            Pressed = false;
            if (canvasGroup) canvasGroup.alpha = 1f;
            if (!Selected) transform.localScale = new Vector3(1, 1, 1);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            OnExit?.Invoke();
            Pressed = false;
            if (canvasGroup) canvasGroup.alpha = 1f;
            if(!Selected) transform.localScale = new Vector3(1, 1, 1);
        }
    }
}

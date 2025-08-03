using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GUI
{
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerClickHandler
    {
        [Range(0.1f, 1f)] public float holdDuration = 0.3f;
        [Space]
        [Header("Event Listener")]
        public UnityEvent onClick;
        public UnityEvent onLongPress;
        public UnityEvent onDown;
        public UnityEvent onUp;
        
        private bool isPressing = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            onDown.Invoke();
            isPressing = true;
            Invoke(nameof(OnLongPress), holdDuration);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CancelInvoke(nameof(OnLongPress));
            onUp.Invoke();
            isPressing = false;
        }
        public void OnPointerMove(PointerEventData eventData)
        {
            float pointerDistance = Vector2.Distance(eventData.delta, Vector2.zero);
            if (pointerDistance >= 10)
            {
                isPressing = false;
            }
        }

        void OnLongPress()
        {
            if (isPressing)
            {
                onLongPress.Invoke();
            }
        }
    }
}


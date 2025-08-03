using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Widget
{
    public class ActionBar : MonoBehaviour
    {
        public RectTransform SwipeTransform;
        RectTransform rectTransform;

        [SerializeField]private float previousSwipeValue = 0;
        [SerializeField]private float startPosition;
        [SerializeField]private float actionHeight;
        [SerializeField]private bool collapse = false;
        public void Awake()
        {
            if (TryGetComponent(out RectTransform rectTransform)) this.rectTransform = rectTransform;
            startPosition = rectTransform.anchoredPosition.y;
            actionHeight = rectTransform.sizeDelta.y;
        }

        public void Update()
        {
            if (SwipeTransform.anchoredPosition.y < 10) collapse = false;
            if (collapse)
            {
                Vector2 targetPosition = new (rectTransform.anchoredPosition.x, startPosition + actionHeight);
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * 15);
            }
            else
            {
                Vector2 targetPosition = new (rectTransform.anchoredPosition.x, startPosition);
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * 15);
            }
        }

        public void SwipeAction(Vector2 value)
        {
            float y = previousSwipeValue - SwipeTransform.anchoredPosition.y;
            float sensor = 5f;
            if (y > sensor || y < -sensor)
            {
                collapse = y < 0;
            }
            previousSwipeValue = SwipeTransform.anchoredPosition.y;
        }
    }
}



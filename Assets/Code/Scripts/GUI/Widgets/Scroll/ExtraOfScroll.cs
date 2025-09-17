using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExtraOfScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [Space] [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    private void Start()
    {
        if (leftArrow.TryGetComponent(out Button leftButton))
        {
            leftButton.onClick.AddListener((ScrollLeft));
        }

        if (rightArrow.TryGetComponent(out Button rightButton))
        {
            rightButton.onClick.AddListener((ScrollRight));
        }
    }

    public void UpdateScrollArrow()
    {
        var normalized = scrollRect.horizontalNormalizedPosition;
        var isNotScrollable = scrollRect.content.rect.width < scrollRect.viewport.rect.width;
        if (isNotScrollable)
        {
            rightArrow.SetActive(false);
            leftArrow.SetActive(false);
        }
        else
            switch (normalized)
            {
                case < 0.1f:
                    rightArrow.SetActive(true);
                    break;
                case > 0.9f:
                    leftArrow.SetActive(true);
                    break;
                default:
                    leftArrow.SetActive(false);
                    rightArrow.SetActive(false);
                    break;
            }
    }

    public void ScrollRight()
    {
        scrollRect.horizontalNormalizedPosition = 1;
        rightArrow.SetActive(false);
    }

    public void ScrollLeft()
    {
        scrollRect.horizontalNormalizedPosition = 0;
        leftArrow.SetActive(false);
    }
}
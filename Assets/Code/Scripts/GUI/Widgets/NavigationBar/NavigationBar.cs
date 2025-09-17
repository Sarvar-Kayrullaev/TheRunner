using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Widget
{
    public class NavigationBar : MonoBehaviour
    {
        [Header("Item Roots")]
        [SerializeField] RectTransform barRoot;
        [SerializeField] RectTransform fragmentRoot;

        [Header("Selection Color")]
        [SerializeField] Color selectedColor;
        [SerializeField] Color unselectedColor;

        public void Start()
        {
            Initialize();
            Select(0);
        }

        private void Initialize()
        {
            foreach (Transform item in barRoot)
            {
                if (item.TryGetComponent(out ItemButton button))
                {
                    button.OnClick.AddListener(() => Select(item.GetSiblingIndex()));
                }
            }
        }

        void Select(int selectedIndex)
        {
            int index = 0;
            foreach (Transform item in barRoot)
            {
                if (item.GetChild(0).TryGetComponent(out TMP_Text text) && item.TryGetComponent(out ItemButton button))
                {
                    if (index == selectedIndex)
                    {
                        SelectFragment(index);
                        text.color = selectedColor;
                        button.Selected = true;
                    }
                    else
                    {
                        text.color = unselectedColor;
                        button.Selected = false;
                    }
                    index++;
                }
            }
        }

        public void SelectFragment(int index)
        {
            int i = 0;
            foreach (Transform window in fragmentRoot)
            {
                if (index == i)
                {
                    window.gameObject.SetActive(true);
                }
                else
                {
                    window.gameObject.SetActive(false);
                }
                i++;
            }
        }
    }
}
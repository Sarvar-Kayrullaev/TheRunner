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
        [SerializeField] private RectTransform barRoot;
        [SerializeField] private RectTransform fragmentRoot;

        [Header("Selection Color")]
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color unselectedColor;
        [HideInInspector] public int currentIndex = 0;
        [HideInInspector] public List<ItemButton> barButtons = new List<ItemButton>();

        public void Awake()
        {
            Initialize();
            Select(0);
        }

        public void Initialize()
        {
            barButtons.Clear();
            foreach (Transform item in barRoot)
            {
                if (item.TryGetComponent(out ItemButton button))
                {
                    barButtons.Add(button);
                    button.OnClick.RemoveAllListeners();
                    button.OnClick.AddListener(() => Select(item.GetSiblingIndex()));
                }
            }
        }

        private void Select(int selectedIndex)
        {
            var index = 0;
            currentIndex =  selectedIndex;
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
            var i = 0;
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
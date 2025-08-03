using System.Collections;
using System.Collections.Generic;
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
                Image image = item.GetComponent<Image>();
                ItemButton button = item.GetComponent<ItemButton>();
                if (index == selectedIndex)
                {
                    SelectFragment(index);
                    if (image) image.color = selectedColor;
                    if (button) button.Selected = true;
                }
                else
                {
                    if (image)image.color = unselectedColor;
                    if (button) button.Selected = false;
                }
                index++;
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
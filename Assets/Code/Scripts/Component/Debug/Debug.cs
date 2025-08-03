using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class Debug : MonoBehaviour
    {
        [SerializeField] private Transform item;
        [SerializeField] private Transform parent;
        [SerializeField] private bool collape = true;

        private void Awake()
        {
            //parent.GetChild(0).gameObject.SetActive(true);
            item = parent.GetChild(0);
            //Destroy(parent.GetChild(0).gameObject);

        }

        public void Log(string message, int ID)
        {
            foreach (Transform item in parent)
            {
                if (item.TryGetComponent(out Item itemCode))
                {
                    if (itemCode.ID == ID)
                    {
                        if (item.GetChild(0).TryGetComponent(out Text text))
                        {
                            text.text = message;
                        }
                        return;
                    }
                }
            }

            //Else Instantiate
            Transform instanced = Instantiate(item, parent);
            instanced.gameObject.SetActive(true);
            instanced.SetAsLastSibling();
            if (instanced.TryGetComponent(out Item itemCode2)) itemCode2.ID = ID;
            if (instanced.GetChild(0).TryGetComponent(out Text text2))
            {
                text2.text = message;
            }
        }

        public void Log(string message, Color textColor, int ID)
        {
            foreach (Transform item in parent)
            {
                if (item.TryGetComponent(out Item itemCode))
                {
                    if (itemCode.ID == ID)
                    {
                        if (item.GetChild(0).TryGetComponent(out Text text))
                        {
                            text.text = message;
                            text.color = textColor;
                        }
                        return;
                    }
                }
            }

            //Else Instantiate
            Transform instanced = Instantiate(item, parent);
            instanced.SetAsLastSibling();
            instanced.gameObject.SetActive(true);
            if (instanced.TryGetComponent(out Item itemCode2)) itemCode2.ID = ID;
            if (instanced.GetChild(0).TryGetComponent(out Text text2))
            {
                text2.text = message;
                text2.color = textColor;
            }
        }

        public void Log(string message, Color textColor, Color backgroundColor, int ID)
        {
            foreach (Transform item in parent)
            {
                if (item.TryGetComponent(out Item itemCode))
                {
                    if (itemCode.ID == ID)
                    {
                        if (item.GetChild(0).TryGetComponent(out Text text))
                        {
                            text.text = message;
                            text.color = textColor;
                        }
                        if (item.TryGetComponent(out Image image))
                        {
                            image.color = backgroundColor;
                        }
                        return;
                    }
                }
            }

            //Else Instantiate
            Transform instanced = Instantiate(item, parent);
            instanced.SetAsLastSibling();
            instanced.gameObject.SetActive(true);
            if (instanced.TryGetComponent(out Item itemCode2)) itemCode2.ID = ID;
            if (instanced.GetChild(0).TryGetComponent(out Text text2))
            {
                text2.text = message;
                text2.color = textColor;
            }
            if (item.TryGetComponent(out Image image2))
            {
                image2.color = backgroundColor;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace PlayerRoot
{
    public class HealthBar : MonoBehaviour
    {
        public int RowByValue;
        public int MaxValue;
        public int Value;

        [Space]
        [Header("Params")]
        public float spacing;
        public float PaddingX;
        public float PaddingY;


        [Space]
        [Header("Graphic")]
        public Sprite RowBackground;

        [Space]
        [Header("Opjects")]
        public RectTransform fillParent;

        private float parentHeight;
        private float parentWidth;
        private float sumWidth = 0;
        private float barWidth;

        private readonly List<RectTransform> bars = new();


        public void Initialize(int RowByValue, int MaxValue, int Value)
        {
            this.RowByValue = RowByValue;
            this.MaxValue = MaxValue;
            this.Value = Value;

            parentHeight = fillParent.rect.height;
            parentWidth = fillParent.rect.width;
            int barCount = MaxValue / RowByValue;

            for (int i = 0; i < barCount; i++)
            {
                CreateBar(i, barCount);
            }
        }
        private void CreateBar(int index, int barCount)
        {
            float width = parentWidth / barCount - spacing - (PaddingX / barCount * 2) + spacing / barCount;
            float height = parentHeight - (PaddingY * 2);
            float positionX = sumWidth;
            positionX += PaddingX;
            float positionY = -parentHeight / 2;

            GameObject bar = new ("bar " + index);
            bar.transform.SetParent(fillParent);

            RectTransform barRect = bar.AddComponent<RectTransform>();
            barRect.anchorMin = new(0, 1);
            barRect.anchorMax = new(0, 1);
            barRect.pivot = new(0, 0.5f);
            barRect.sizeDelta = new(width, height);
            barRect.anchoredPosition = new(positionX, positionY);
            barRect.localScale = new(1,1,1);

            Image barImage = bar.AddComponent<Image>();
            barImage.sprite = RowBackground;
            barImage.color = Color.white;
            barImage.type = Image.Type.Sliced;
            barImage.pixelsPerUnitMultiplier = 2;
            sumWidth += width + spacing;

            bars.Add(barRect);
            barWidth = width;
        }

        public void ChangeValue(int value)
        {
            Value = value;
            float percentage = 100 * ((float)Value / (float)MaxValue);

            float totalBarWidth = barWidth * bars.Count;
            float currentBarWidth = totalBarWidth * percentage / 100;
            foreach (RectTransform bar in bars)
            {
                int index = bar.GetSiblingIndex();
                float hithertoWidth = barWidth * index;
                if (currentBarWidth > hithertoWidth)
                {
                    bar.gameObject.SetActive(true);
                    if (InValue(hithertoWidth, hithertoWidth + barWidth, currentBarWidth))
                    {
                        float barFixedSize = currentBarWidth - hithertoWidth;
                        bar.sizeDelta = new(barFixedSize, bar.rect.height);
                    }
                    else
                    {
                        bar.sizeDelta = new(barWidth, bar.rect.height);
                    }
                }
                else
                {
                    bar.sizeDelta = new(0, bar.rect.height);
                    bar.gameObject.SetActive(false);
                }
            }
        }

        bool InValue(float min, float max, float cursor)
        {
            if (cursor > min && cursor < max) return true;
            else return false;
        }
    }
}


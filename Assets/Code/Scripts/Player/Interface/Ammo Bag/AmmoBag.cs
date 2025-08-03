using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoot
{
    public class AmmoBag : MonoBehaviour
    {
        [SerializeField] float ParentMaxWidth;
        [SerializeField] float BulletWidth;
        [SerializeField] float Spacing;
        [SerializeField] float PaddingX;
        [SerializeField] float PaddingY;

        [SerializeField] int bulletCount = 40;

        [Space]
        [Header("Setup")]
        [SerializeField] RectTransform Parent;
        [SerializeField] Sprite AmmoSprite;

        private readonly List<Image> bulletsImage = new();
        private float SumPositionX;

        public void Build(int currentAmmo, int magazineSize)
        {
            Clear();
            bulletsImage.Clear();
            SumPositionX = 0;
            RectTransform bulletTemplate = BulletTemplate();
            for (int i = 0; i < magazineSize; i++)
            {
                float positionX = PaddingX + SumPositionX;
                RectTransform bullet = Instantiate(bulletTemplate, Vector2.zero, Quaternion.identity, Parent);
                bullet.anchoredPosition = new(positionX, -Parent.rect.height / 2);
                if (bullet.TryGetComponent(out Image image))
                {
                    bulletsImage.Add(image);
                    if (i < currentAmmo)
                    {
                        image.color = new Color(256, 256, 256, 1f);
                    }
                    else
                    {
                        image.color = new Color(256, 256, 256, 0.1f);
                    }
                }
                SumPositionX += BulletWidth + Spacing;
            }
            Destroy(bulletTemplate.gameObject);

            Parent.sizeDelta = new(Mathf.Clamp((SumPositionX + PaddingX * 2) - Spacing, PaddingX * 2, ParentMaxWidth), Parent.rect.height);
        }

        public void TakeBullet(int currentAmmo)
        {
            if (bulletsImage.Count > 0)
                bulletsImage[currentAmmo - 1].color = new Color(256, 256, 256, 0.1f);
        }

        public RectTransform BulletTemplate()
        {
            float width = BulletWidth;
            float height = Parent.rect.height - (PaddingY * 2);

            GameObject ammo = new("Ammo");

            RectTransform ammoRect = ammo.AddComponent<RectTransform>();
            ammoRect.anchorMin = new(0, 1);
            ammoRect.anchorMax = new(0, 1);
            ammoRect.pivot = new(0, 0.5f);
            ammoRect.sizeDelta = new(width, height);
            ammoRect.anchoredPosition = new(0, 0);
            ammoRect.localScale = new(1, 1, 1);

            Image ammoImage = ammo.AddComponent<Image>();
            ammoImage.sprite = AmmoSprite;
            ammoImage.color = Color.white;
            return ammoRect;
        }

        void Clear()
        {
            foreach (Transform item in Parent)
            {
                Destroy(item.gameObject);
            }
        }
    }
}


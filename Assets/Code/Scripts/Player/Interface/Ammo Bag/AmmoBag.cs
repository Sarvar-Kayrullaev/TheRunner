using System;
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
        private RectTransform _bulletTemplate;

        private void Awake()
        {
            _bulletTemplate = BulletTemplate();
        }

        public void Build(int currentAmmo, int magazineSize)
        {
            Clear();
            bulletsImage.Clear();
            SumPositionX = 0;
            for (var i = 0; i < magazineSize; i++)
            {
                var positionX = PaddingX + SumPositionX;
                var bullet = Instantiate(_bulletTemplate, Vector2.zero, Quaternion.identity, Parent);
                bullet.anchoredPosition = new Vector2(positionX, -Parent.rect.height / 2);
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

            Parent.sizeDelta = new(Mathf.Clamp((SumPositionX + PaddingX * 2) - Spacing, PaddingX * 2, ParentMaxWidth), Parent.rect.height);
        }

        public void ResetBullets(int currentAmmo, int magazineSize)
        {
            var filledColor = new Color(256, 256, 256, 1f);
            var unfilledColor = new Color(256, 256, 256, 0.1f);

            var i = 0;
            foreach (var image in bulletsImage)
            {
                image.color = i < currentAmmo ? filledColor : unfilledColor;
                i++;
            }
        }

        public void TakeBullet(int currentAmmo)
        {
            if (bulletsImage.Count > 0)
                bulletsImage[currentAmmo - 1].color = new Color(256, 256, 256, 0.1f);
        }

        private RectTransform BulletTemplate()
        {
            var width = BulletWidth;
            var height = Parent.rect.height - (PaddingY * 2);

            GameObject ammo = new("Ammo");

            var ammoRect = ammo.AddComponent<RectTransform>();
            ammoRect.anchorMin = new Vector2(0, 1);
            ammoRect.anchorMax = new Vector2(0, 1);
            ammoRect.pivot = new Vector2(0, 0.5f);
            ammoRect.sizeDelta = new Vector2(width, height);
            ammoRect.anchoredPosition = new Vector2(0, 0);
            ammoRect.localScale = new Vector3(1, 1, 1);

            Image ammoImage = ammo.AddComponent<Image>();
            ammoImage.sprite = AmmoSprite;
            ammoImage.color = Color.white;
            return ammoRect;
        }

        private void Clear()
        {
            foreach (Transform item in Parent)
            {
                Destroy(item.gameObject);
            }
        }
    }
}


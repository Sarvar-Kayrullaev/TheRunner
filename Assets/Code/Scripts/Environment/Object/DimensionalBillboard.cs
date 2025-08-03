using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Required Shader:  Legacy Shader > Transparent > Cutout > Bumpet Diffuse.shader*/

namespace Environment
{
    public class DimensionalBillboard : MonoBehaviour
    {
        [Space]
        [Header("Properties")]
        public Vector2 Tiling;
        public int Vertical;
        public int SpriteOffset = 0;

        private Material material;
        private Transform target;
        private Vector3 startPosition;
        private Vector3 transformForward;
        private Vector2 offset;
        private Vector2 tiling;

        private void Start()
        {
            startPosition = transform.position;
            transformForward = transform.forward;
            material = GetComponent<Renderer>().material;
            target = Camera.main.transform;

            InvokeRepeating(nameof(Refresh), 0, 0.5f);
        }

        private void Refresh()
        {
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            SpriteUpdate();
        }

        void SpriteUpdate()
        {
            Vector3 targetDir = target.position - startPosition;
            targetDir.y = 0;
            float angle = Vector3.SignedAngle(targetDir, transformForward, Vector3.up);

            float oneItemSizeX = 1 / Tiling.x;
            float oneITemSizeY = 1 / Tiling.y;

            offset.y = oneITemSizeY * Vertical;
            tiling.x = oneItemSizeX;
            tiling.y = oneITemSizeY;

            float unit = 360 / Tiling.x;
            int Cursor = CalculateUnits(angle, unit, (int)Tiling.x);

            offset.x = oneItemSizeX * (Cursor + SpriteOffset);
            material.SetTextureOffset("_MainTex", offset);
            material.SetTextureScale("_MainTex", tiling);
            material.SetTextureOffset("_BumpMap", offset);
            material.SetTextureScale("_BumpMap", tiling);
        }

        public int CalculateUnits(float angle, float unit, int TileX)
        {
            angle -= unit / 2;
            int calculeted = (int)Math.Ceiling(angle / unit) + (TileX / 2);
            if (calculeted == 0) return TileX;
            else return calculeted;
        }
    }
}

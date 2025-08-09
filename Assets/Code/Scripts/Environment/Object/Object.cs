using System;
using UnityEngine;

namespace Environment
{
    public class Object : MonoBehaviour
    {
        [Space]
        [Header("Object Properties")]
        public float UpdateTime = 0.5f;
        public float EasingUpdateTime = 3;
        [Space]
        public float EasingUpdateDistance = 100;
        public float ColliderCullingDistance = 50;
        [Space]
        public GameObject FracturedPrefab;
        [Space]
        [Header("Billboard Properties")]
        public Transform billboard;
        [Space]
        public Vector2 Tiling;
        public int Vertical;
        public int SpriteOffset = 0;
        public bool inverseOffcet = false;

        private Material material;
        private Transform target;
        private MeshCollider meshCollider;
        private Rigidbody rigidBody;
        private Vector3 startPosition;
        private Vector3 transformForward;
        private Vector2 offset;
        private Vector2 tiling;
        private float cameraDistance;
        private bool isEasingUpdate = false;
        private bool isPhysic = true;
        private bool fractured = false;

        private void Start()
        {
            startPosition = billboard.position;
            transformForward = billboard.forward;
            if (billboard.TryGetComponent(out Renderer renderer)) material = renderer.material;
            if (TryGetComponent(out MeshCollider meshCollider)) this.meshCollider = meshCollider;
            if (TryGetComponent(out Rigidbody rigidBody)) this.rigidBody = rigidBody;
            target = Camera.main.transform;
            
            InvokeRepeating(nameof(Refresh), 0, UpdateTime);
        }
        public void Fracturing()
        {
            if(fractured) return;
            FracturedPrefab.transform.localScale = transform.parent.localScale;
            Instantiate(FracturedPrefab, transform.parent.position, transform.parent.rotation);
            Debug.Log("Fractued");
            Destroy(transform.parent.gameObject);
            fractured = true;
        }

        private void Refresh()
        {
            cameraDistance = Vector3.Distance(target.position, startPosition);
            InvokeController(cameraDistance);
            ColliderCulling(cameraDistance);
            billboard.LookAt(new Vector3(target.position.x, billboard.position.y, target.position.z));
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
            if(inverseOffcet) unit = -unit;
            int Cursor = CalculateUnits(angle, unit, (int)Tiling.x);

            offset.x = oneItemSizeX * (Cursor + SpriteOffset);
            material.SetTextureOffset("_BaseMap", offset);
            material.SetTextureScale("_BaseMap", tiling);
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

        private void InvokeController(float playerDistance)
        {
            bool Easing = EasingUpdateDistance <= playerDistance;
            if (Easing && !isEasingUpdate)
            {
                CancelInvoke(nameof(Refresh));
                InvokeRepeating(nameof(Refresh), EasingUpdateTime, EasingUpdateTime);
                isEasingUpdate = true;
            }
            else if (!Easing && isEasingUpdate)
            {
                CancelInvoke(nameof(Refresh));
                InvokeRepeating(nameof(Refresh), UpdateTime, UpdateTime);
                isEasingUpdate = false;
            }
        }

        private void ColliderCulling(float playerDistance)
        {
            bool culling = ColliderCullingDistance <= playerDistance;
            if (culling && isPhysic)
            {
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
                meshCollider.enabled = false;
                isPhysic = false;
            }
            else
            {
                if (!culling && !isPhysic)
                {
                    rigidBody.isKinematic = false;
                    rigidBody.useGravity = false;
                    meshCollider.enabled = true;
                    isPhysic = true;
                }
            }
        }
    }
}

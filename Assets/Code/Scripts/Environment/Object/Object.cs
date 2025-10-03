using System;
using UnityEngine;

namespace Environment
{
    public class Object : MonoBehaviour
    {
        private static readonly int BaseMap = UnityEngine.Shader.PropertyToID("_BaseMap");
        private static readonly int BumpMap = UnityEngine.Shader.PropertyToID("_BumpMap");
        
        [Space]
        [Header("Object Properties")]
        public float updateTime = 0.5f;
        public float easingUpdateTime = 3;
        public float easingUpdateDistance = 100;
        public float colliderCullingDistance = 50;
        public GameObject fracturedPrefab;
        [Space]
        [Header("Billboard Properties")]
        public Transform billboard;
        public Vector2 tiling;
        public int vertical;
        public int spriteOffset;
        public bool inverseOffcet = true;
        [Space] [Header("Sounds")] 
        public GameObject audioSourcePrefab;
        public AudioClip fracturingSound;

        private Material _material;
        private Transform _target;
        private MeshCollider _meshCollider;
        private Rigidbody _rigidBody;
        private Vector3 _startPosition;
        private Vector3 _transformForward;
        private Vector2 _offset;
        private Vector2 _tiling;
        private float _cameraDistance;
        private bool _isEasingUpdate;
        private bool _isPhysic = true;
        private bool _fractured;

        private void Start()
        {
            _startPosition = billboard.position;
            _transformForward = billboard.forward;
            if (billboard.TryGetComponent(out Renderer rendererComponent)) _material = rendererComponent.material;
            if (TryGetComponent(out MeshCollider meshCollider)) this._meshCollider = meshCollider;
            if (TryGetComponent(out Rigidbody rigidBody)) this._rigidBody = rigidBody;
            _target = Camera.main?.transform;
            
            InvokeRepeating(nameof(Refresh), 0, updateTime);
        }
        public void Fracturing()
        {
            _meshCollider.enabled = false;
            if(_fractured) return;
            fracturedPrefab.transform.localScale = transform.parent.localScale;
            Instantiate(fracturedPrefab, transform.parent.position, transform.parent.rotation);
            //Debug.Log("Fractued");
            Destroy(transform.parent.gameObject);
            _fractured = true;
        }

        public void PlaySound()
        {
            var audioSourceObject = Instantiate(audioSourcePrefab, transform.position, transform.rotation);
            if (audioSourceObject.TryGetComponent(out AudioSource audioSourceComponent))
            {
                audioSourceComponent.PlayOneShot(fracturingSound, 1);
            }
        }

        private void Refresh()
        {
            _cameraDistance = Vector3.Distance(_target.position, _startPosition);
            InvokeController(_cameraDistance);
            ColliderCulling(_cameraDistance);
            billboard.LookAt(new Vector3(_target.position.x, billboard.position.y, _target.position.z));
            SpriteUpdate();
        }

        private void SpriteUpdate()
        {
            var targetDir = _target.position - _startPosition;
            targetDir.y = 0;
            var angle = Vector3.SignedAngle(targetDir, _transformForward, Vector3.up);

            var oneItemSizeX = 1 / tiling.x;
            var oneITemSizeY = 1 / tiling.y;

            _offset.y = oneITemSizeY * vertical;
            _tiling.x = oneItemSizeX;
            _tiling.y = oneITemSizeY;

            var unit = 360 / tiling.x;
            if(inverseOffcet) unit = -unit;
            var cursor = CalculateUnits(angle, unit, (int)tiling.x);

            _offset.x = oneItemSizeX * (cursor + spriteOffset);
            _material.SetTextureOffset(BaseMap, _offset);
            _material.SetTextureScale(BaseMap, _tiling);
            _material.SetTextureOffset(BumpMap, _offset);
            _material.SetTextureScale(BumpMap, _tiling);
        }

        private static int CalculateUnits(float angle, float unit, int tileX)
        {
            angle -= unit / 2;
            var calculeted = (int)Math.Ceiling(angle / unit) + (tileX / 2);
            return calculeted == 0 ? tileX : calculeted;
        }

        private void InvokeController(float playerDistance)
        {
            var easing = easingUpdateDistance <= playerDistance;
            switch (easing)
            {
                case true when !_isEasingUpdate:
                    CancelInvoke(nameof(Refresh));
                    InvokeRepeating(nameof(Refresh), easingUpdateTime, easingUpdateTime);
                    _isEasingUpdate = true;
                    break;
                case false when _isEasingUpdate:
                    CancelInvoke(nameof(Refresh));
                    InvokeRepeating(nameof(Refresh), updateTime, updateTime);
                    _isEasingUpdate = false;
                    break;
            }
        }

        private void ColliderCulling(float playerDistance)
        {
            var isCulling = colliderCullingDistance <= playerDistance;
            switch (isCulling)
            {
                case true when _isPhysic:
                    _rigidBody.isKinematic = true;
                    _rigidBody.useGravity = false;
                    _meshCollider.enabled = false;
                    _isPhysic = false;
                    break;
                case false when !_isPhysic:
                    _rigidBody.isKinematic = true;
                    _rigidBody.useGravity = false;
                    _meshCollider.enabled = true;
                    _isPhysic = true;
                    break;
            }
        }
    }
}

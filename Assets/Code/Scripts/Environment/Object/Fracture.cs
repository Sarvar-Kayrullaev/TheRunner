using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
    public class Fracture : MonoBehaviour
    {
        public int health = 10;
        public Transform disintegrationParticle;
        public Transform destructionParticle;
        public Transform hitParticle;
        public Vector3 destructionPosition;
        public GameObject audioSourcePrefab;
        public AudioClip[] destructionSounds;
        public AudioClip[] fracturingSounds;
        public AudioClip[] hitSounds;

        private int _maxHealth;
        private bool _isFixedJointDestroyed = false;
        private Rigidbody _rigidbody;
        private FixedJoint _fixedJoint;
        private GameObject _audioSourceObject;
        private AudioSource _audioSource;
        private void Start()
        {
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _fixedJoint);
            
            gameObject.tag = "Destructible";
            _maxHealth = health;
            var newDestructionPosition = destructionPosition + transform.position;
            var destruction = Instantiate(hitParticle,newDestructionPosition, transform.rotation);
            Destroy(destruction.gameObject, 3);
        }

        public void TakeHealth(int value)
        {
            health -= value;
            if (health < _maxHealth / 2)
            {
                if (!_isFixedJointDestroyed)
                {
                    if (!_audioSource)
                    {
                        _audioSourceObject =  Instantiate(audioSourcePrefab, transform.position, transform.rotation, transform);
                        if (_audioSourceObject.TryGetComponent(out _audioSource));
                    }
                    var randomClipIndex = UnityEngine.Random.Range(0, destructionSounds.Length);
                    _audioSource.PlayOneShot(destructionSounds[randomClipIndex], 1);
                    
                    var newDestructionPosition = destructionPosition + transform.position;
                    var destruction = Instantiate(destructionParticle,newDestructionPosition, transform.rotation);
                    Destroy(destruction.gameObject, 3);
                    Destroy(_fixedJoint);
                    _isFixedJointDestroyed  = true;
                }
            }
            if (health > 0) return;
            if (!_audioSource)
            {
                _audioSourceObject =  Instantiate(audioSourcePrefab, transform.position, transform.rotation, transform.parent);
                if (_audioSourceObject.TryGetComponent(out _audioSource));
            }
            var randomClipIndex2 = UnityEngine.Random.Range(0, fracturingSounds.Length);
            _audioSource.PlayOneShot(fracturingSounds[randomClipIndex2], 1);
            
            var disintegration = Instantiate(disintegrationParticle, transform.position, transform.rotation);
            Destroy(disintegration.gameObject, 3);
            Unchild();
            Destroy(gameObject);
        }

        public void HitSoundPlay()
        {
            if (!_audioSource)
            {
                _audioSourceObject =  Instantiate(audioSourcePrefab, transform.position, transform.rotation, transform);
                if (_audioSourceObject.TryGetComponent(out _audioSource));
            }
            var randomClipIndex = UnityEngine.Random.Range(0, hitSounds.Length);
            _audioSource.PlayOneShot(hitSounds[randomClipIndex], 1);
        }

        private void Unchild()
        {
            foreach (Transform child in transform)
            {
                child.parent = transform.parent ? transform.parent : null;
                if(child.TryGetComponent(out FixedJoint childFixedJoint)) Destroy(childFixedJoint);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var position = destructionPosition + transform.position;
            var gizmoColor = Color.deepPink;
            const float gizmoRadius = 0.1f;
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(position, gizmoRadius);
        }
    }
}
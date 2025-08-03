using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerRoot;

namespace BotRoot
{
    public class BotBullet : MonoBehaviour
    {
        private int damage;
        [SerializeField] private float speed;
        [SerializeField] private float gravity;
        [SerializeField] BotSetup setup;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 startForward;

        [Space(2)]
        [Header("ExplosionForce")]
        public float power;
        public float radius;

        [SerializeField] private bool isInitialized = false;
        [SerializeField] private float startTime = -1;
        [SerializeField] private LayerMask obstaclesMask;
        [SerializeField] Transform Particle;
        [SerializeField] private Transform InstanceSound;
        [SerializeField] private bool StopUpdate = false;
        float destroySelfTime;

        public void Initialize(Transform startPoint, float speed, float gravity, int damage, BotSetup owner)
        {
            startPosition = startPoint.position;
            startForward = startPoint.forward;
            this.speed = speed;
            this.gravity = gravity;
            this.damage = damage;
            this.setup = owner;
            isInitialized = true;
            startTime = -1;
            destroySelfTime = 5;
        }

        private Vector3 FindPointOnParabole(float time)
        {
            Vector3 point = startPosition + (startForward * speed * time);
            Vector3 gravityVec = Vector3.down * gravity * time * time;
            return point + gravityVec;
        }

        private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
        {
            return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude, obstaclesMask);
        }
        private void FixedUpdate()
        {
            if (StopUpdate) return;
            if (!isInitialized) return;
            if (startTime < 0) startTime = Time.time;

            RaycastHit hit;
            float curretTime = Time.time - startTime;
            float nextTime = curretTime + Time.fixedDeltaTime;

            Vector3 currentPoint = FindPointOnParabole(curretTime);
            Vector3 nextPoint = FindPointOnParabole(nextTime);

            if (CastRayBetweenPoints(currentPoint, nextPoint, out hit))
            {
                if (hit.transform.TryGetComponent(out HitableObject hitable))
                {
                    if (hitable.hitBulletSound) if (Instantiate(InstanceSound, hit.point, Quaternion.identity).TryGetComponent(out AudioSource audioSource))
                        {
                            audioSource.PlayOneShot(hitable.hitBulletSound);
                        }
                    if (hit.transform.TryGetComponent(out PlayerBody playerBody))
                    {
                        playerBody.player.damageNavigatorRegister.CreateIndicator(setup.transform, playerBody.player.transform);
                    }
                    if (hitable) hitable.HitVisualize(hit.point, hit.normal);
                    if (hitable) hitable.HitBullet(damage, setup.transform);
                    if (hitable)
                    {
                        hitable.power = power;
                        hitable.radius = radius;
                        hitable.forceDirection = startPosition;
                        Destroy(gameObject);
                    }
                }
            }
        }
        private void Update()
        {
            if (StopUpdate) return;
            if (!isInitialized || startTime < 0) return;

            float currentTime = Time.time - startTime;
            Vector3 currentPoint = FindPointOnParabole(currentTime);
            transform.position = currentPoint;
            destroySelfTime -= Time.deltaTime;
            if (destroySelfTime <= 0)
            {

                Destroy(gameObject);
            }
        }
    }
}


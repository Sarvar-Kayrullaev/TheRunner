using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerRoot;
using BotRoot;
using Environment;
using UnityEngine.Serialization;

public class ParabolicBullet : MonoBehaviour
{
    private int _damage;
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] Player player;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 startForward;
    [SerializeField] private LayerMask listenerMask;

    [Space(2)]
    [Header("ExplosionForce")]
    public float power;
    public float radius;

    [SerializeField] private bool isInitialized = false;
    [SerializeField] private float startTime = -1;
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] private Transform instanceSound;
    private bool _childOuted = false;
    private float _destroySelfTime;
    private List<Collider> _ignoredColliders = new();

    public void Initialize(Transform startPoint, float speed, float gravity, int damage, Player player)
    {
        startPosition = startPoint.position;
        startForward = startPoint.forward;
        this.speed = speed;
        this.gravity = gravity;
        this._damage = damage;
        this.player = player;
        isInitialized = true;
        startTime = -1;
        _destroySelfTime = 5;

        float distanceOfStartPoint = Vector3.Distance(player.playerCamera.transform.position, startPosition);
        if (Physics.Raycast(player.playerCamera.transform.position, startPosition - player.playerCamera.transform.position, out RaycastHit hit, distanceOfStartPoint, obstaclesMask))
        {
            if (hit.transform.TryGetComponent(out HitableObject hitable))
            {
                hitable.HitVisualize(hit.point, hit.normal);
                hitable.HitBullet(damage, player.transform);
                hitable.power = power;
                hitable.radius = radius;
                hitable.forceDirection = player.transform.position;
            }
            Destroy(gameObject);
        }
    }

    public void ContinueBullet(Vector3 position, Vector3 direction, int damage)
    {
        startPosition = position;
        startForward = direction;
        this._damage = damage;
        startTime = -1;
    }

    private Vector3 FindPointOnParabole(float time)
    {
        var point = startPosition + (startForward * (speed * time));
        var gravityVec = Vector3.down * (gravity * time * time);
        return point + gravityVec;
    }

    private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
    {
        return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude, obstaclesMask);
    }
    private void FixedUpdate()
    {
        if (!isInitialized) return;
        if (startTime < 0) startTime = Time.time;

        var curretTime = Time.time - startTime;
        var nextTime = curretTime + Time.fixedDeltaTime;
        var currentPoint = FindPointOnParabole(curretTime);
        var nextPoint = FindPointOnParabole(nextTime);

        if (CastRayBetweenPoints(currentPoint, nextPoint, out var hit))
        {
            print("Hit " + hit.collider.name);
            if (!_childOuted)
            {
                if (transform.childCount > 0)
                {
                    var trail = transform.GetChild(0);
                    trail.position = hit.point;
                    trail.SetParent(null);
                }
                _childOuted = true;
            }

            var ignored = _ignoredColliders.Contains(hit.collider);

            if (!ignored)
            {
                _ignoredColliders.Add(hit.collider);

                if (hit.transform.TryGetComponent(out HitableObject hitable))
                {
                    hitable.HitVisualize(hit.point, hit.normal);
                    hitable.HitBullet(_damage, player.transform);

                    hitable.power = power;
                    hitable.radius = radius;
                    hitable.forceDirection = startPosition;

                    if (hitable.hitBulletSound)
                        if (Instantiate(instanceSound, hit.point, Quaternion.identity).TryGetComponent(out AudioSource audioSource))
                        {
                            audioSource.PlayOneShot(hitable.hitBulletSound);
                        }
                    Destroy(gameObject);
                }
                else if (hit.transform.TryGetComponent(out Environment.Object element))
                {
                    element.Fracturing();
                }
                else if (hit.transform.tag == "Destructible")
                {
                    if (hit.transform.TryGetComponent(out Fracture fracture))
                    {
                        fracture.HitSoundPlay();
                        fracture.TakeHealth(1);
                        var playerDirection = fracture.transform.position - startPosition;
                        var normalizedDirection = playerDirection.normalized;
                        if(hit.transform.TryGetComponent(out Rigidbody hitRigidBody)) hitRigidBody.AddForce(normalizedDirection * 10, ForceMode.Impulse);
                        var destruction = Instantiate(fracture.hitParticle,hit.point, transform.rotation);
                        Destroy(destruction.gameObject, 3);
                    }
                }
                if (hit.collider.CompareTag("AI/Listener"))
                {
                    if (hit.transform.TryGetComponent(out BotSensor sensor))
                    {
                        sensor.SetSuspectPoint(player.transform, 4, true);
                        hit.collider.enabled = false;
                        sensor.InvokeEnableListener(5);
                    }
                    var direction = (nextPoint - currentPoint).normalized;
                    ContinueBullet(hit.point, direction, _damage);
                }
                else
                {
                    if (!hit.collider.CompareTag("Live/Evil"))
                    {
                        BulletSoundNoise(20);
                    }
                }
            }
        }
    }
    private void Update()
    {
        if (!isInitialized || startTime < 0) return;

        float currentTime = Time.time - startTime;
        Vector3 currentPoint = FindPointOnParabole(currentTime);
        transform.position = currentPoint;
        _destroySelfTime -= Time.deltaTime;
        if (_destroySelfTime <= 0)
        {

            Destroy(gameObject);
        }
    }

    public void BulletSoundNoise(float distance)
    {
        Collider[] listeners = Physics.OverlapSphere(transform.position, distance, listenerMask);
        float shortestDistance = Mathf.Infinity;
        Transform nearestListener = null;
        for (int i = 0; i < listeners.Length; i++)
        {
            Transform listener = listeners[i].transform;
            if (listener.GetComponent<BotSetup>())
            {
                float distanceEnemy = Vector3.Distance(transform.position, listener.position);
                if (distanceEnemy <= shortestDistance)
                {
                    shortestDistance = distanceEnemy;
                    nearestListener = listener;
                }
            }
        }
        if (nearestListener)
        {
            if (nearestListener.TryGetComponent(out BotSetup setup))
            {
                setup.sensor.SetSuspectPoint(player.transform, 4, true);
            }
            // BotSetup setup = nearestListener.GetComponent<BotSetup>();
            // setup.status.MentalState = BotEnum.MentalState.Suspicion;
            // setup.memory.suspectionID = Random.Range(1000000, 9999999);
            // setup.objects.ChangeSuspectionPoint(player.transform.position);
        }
    }
}

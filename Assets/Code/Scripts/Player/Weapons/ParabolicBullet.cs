using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerRoot;
using BotRoot;
public class ParabolicBullet : MonoBehaviour
{
    private int damage;
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
    [SerializeField] Transform Particle;
    [SerializeField] private Transform InstanceSound;
    [SerializeField]
    private bool StopUpdate = false;
    private bool childOuted = false;
    float destroySelfTime;
    private List<Collider> ignoredColliders = new();

    public void Initialize(Transform startPoint, float speed, float gravity, int damage, Player player)
    {
        startPosition = startPoint.position;
        startForward = startPoint.forward;
        this.speed = speed;
        this.gravity = gravity;
        this.damage = damage;
        this.player = player;
        isInitialized = true;
        startTime = -1;
        destroySelfTime = 5;

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
        this.damage = damage;
        startTime = -1;
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

        float curretTime = Time.time - startTime;
        float nextTime = curretTime + Time.fixedDeltaTime;
        Vector3 currentPoint = FindPointOnParabole(curretTime);
        Vector3 nextPoint = FindPointOnParabole(nextTime);

        if (CastRayBetweenPoints(currentPoint, nextPoint, out RaycastHit hit))
        {
            if (!childOuted)
            {
                if (transform.childCount > 0)
                {
                    Transform trail = transform.GetChild(0);
                    trail.position = hit.point;
                    trail.SetParent(null);
                }
                childOuted = true;
            }

            bool ignored = ignoredColliders.Contains(hit.collider);

            if (!ignored)
            {
                ignoredColliders.Add(hit.collider);

                if (hit.transform.TryGetComponent(out HitableObject hitable))
                {
                    hitable.HitVisualize(hit.point, hit.normal);
                    hitable.HitBullet(damage, player.transform);

                    hitable.power = power;
                    hitable.radius = radius;
                    hitable.forceDirection = startPosition;

                    if (hitable.hitBulletSound)
                        if (Instantiate(InstanceSound, hit.point, Quaternion.identity).TryGetComponent(out AudioSource audioSource))
                        {
                            audioSource.PlayOneShot(hitable.hitBulletSound);
                        }
                    Destroy(gameObject);
                }

                if (hit.collider.CompareTag("AI/Listener"))
                {
                    if (hit.transform.TryGetComponent(out BotSensor sensor))
                    {
                        sensor.SetSuspectPoint(player.transform, 4, true);
                        hit.collider.enabled = false;
                        sensor.InvokeEnableListener(5);
                    }
                    Vector3 direction = (nextPoint - currentPoint).normalized;
                    ContinueBullet(hit.point, direction, damage);
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

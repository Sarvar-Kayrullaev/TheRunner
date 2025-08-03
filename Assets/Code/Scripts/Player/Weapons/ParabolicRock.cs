using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BotRoot;

public class ParabolicRock : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 startForward;

    [SerializeField] private bool isInitialized = false;
    [SerializeField] private float startTime = -1;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] Transform Particle;
    private bool StopUpdate = false;

    public void Initialize(Transform startPoint, float speed, float gravity)
    {
        startPosition = startPoint.position;
        startForward = startPoint.forward;
        this.speed = speed;
        this.gravity = gravity;
        isInitialized = true;
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

        RaycastHit hit;
        float curretTime = Time.time - startTime;
        float nextTime = curretTime + Time.fixedDeltaTime;

        Vector3 currentPoint = FindPointOnParabole(curretTime);
        Vector3 nextPoint = FindPointOnParabole(nextTime);

        if (CastRayBetweenPoints(currentPoint, nextPoint, out hit))
        {
            transform.position = hit.point;
            Transform particle = Instantiate(Particle);
            particle.position = transform.position;
            Destroy(particle.gameObject, 2);
            Destroy(gameObject, 15);
            StopUpdate = true;
            Collider[] listeners = Physics.OverlapSphere(transform.position, 20, enemyMask);
            float shortestDistance = Mathf.Infinity;
            Transform nearestListener = null;
            for (int i = 0; i < listeners.Length; i++)
            {
                Transform listener = listeners[i].transform;
                BotSetup setup = listener.GetComponent<BotSetup>();
                if (setup)
                {
                    if (setup.health.currentHealth <= 0) continue;

                    setup.status.MentalState = BotEnum.MentalState.Suspicion;
                    setup.memory.suspectionID = Random.Range(1000000, 9999999);
                    setup.objects.ChangeSuspectionPoint(transform.position);
                    setup.memory.NotNecessarilyMoveToWonderingPoint = true;
                    setup.author.SetWonderSignal(true);
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
                BotSetup setup = nearestListener.GetComponent<BotSetup>();
                setup.status.MentalState = BotEnum.MentalState.Suspicion;
                setup.memory.suspectionID = Random.Range(1000000, 9999999);
                setup.objects.ChangeSuspectionPoint(transform.position);
                setup.memory.NotNecessarilyMoveToWonderingPoint = false;
                setup.memory.NumberOfSuspicions++;
                setup.botAudio.Play(PanicTalking.SUSPECTED,setup);
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
    }
}

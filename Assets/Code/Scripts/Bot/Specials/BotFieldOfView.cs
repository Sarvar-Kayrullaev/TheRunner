using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerRoot;
namespace BotRoot
{
    public class BotFieldOfView : MonoBehaviour
    {
        public float viewRadius;
        public float Y = 0;
        [Range(0, 360)]
        public float viewAngle;
        [Range(0, 360)]
        public float diedAllyViewAngle = 140f;

        public LayerMask targetMask;
        public LayerMask hidingMask;
        public LayerMask obstacleMask;
        public LayerMask allyMask;
        public IEnumerator FindTarget;
        public IEnumerator FindDiedAlly;
        public IEnumerator UpdateAlly;

        public Collider[] TARGETS;
        public List<BotAuthor> authors = new();

        [HideInInspector]
        public List<Transform> visibleTargets = new List<Transform>();
        [HideInInspector] public BotSetup setup;

        private void Start()
        {
            InvokeRepeating(nameof(UpdateAllies), 0, 5f);
            FindTarget = FindTargetsWithDelay(0.3f);
            FindDiedAlly = FindDiedAllyWithDelay(0.5f);
            //StartCoroutine("FindTargetsWithDelay", 0.3f);
            //StartCoroutine("FindDiedAllyWithDelay", 0.5f);
            StartCoroutine(FindTarget);
            StartCoroutine(FindDiedAlly);
        }

        public void OnDisable()
        {
            StopCoroutine(FindTarget);
            StopCoroutine(FindDiedAlly);
        }

        void UpdateAllies()
        {
            authors = new();
            Collider[] botsCollider = Physics.OverlapSphere(transform.position, viewRadius, allyMask);
            foreach (Collider collider in botsCollider)
            {
                if (collider.TryGetComponent(out BotAuthor author))
                {
                    authors.Add(author);
                }
            }
        }

        IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        IEnumerator FindDiedAllyWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleDiedAlly();
            }
        }

        void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            TARGETS = targetsInViewRadius;
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                if (targetsInViewRadius[i].gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Transform target = targetsInViewRadius[i].transform;
                    Vector3 dirToTarget = (target.position - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                    {
                        float dstToTarget = Vector3.Distance(transform.position, target.position);

                        if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                        {
                            visibleTargets.Add(target);
                        }
                    }
                }
                else if (targetsInViewRadius[i].gameObject.layer == LayerMask.NameToLayer("Vehicle"))
                {
                    
                    Transform target = targetsInViewRadius[i].transform;
                    Vector3 dirToTarget = (target.position - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                    {
                        float dstToTarget = Vector3.Distance(transform.position, target.position);

                        if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                        {
                            if (target.TryGetComponent(out CarController carController))
                            {
                                if (carController.activate)
                                {
                                    if (carController.player)
                                    {
                                        visibleTargets.Add(carController.player.transform);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;
            foreach (Transform enemy in visibleTargets)
            {
                float distanceEnemy = Vector3.Distance(transform.position, enemy.position);
                if (enemy.gameObject != gameObject)
                {
                    if (distanceEnemy <= shortestDistance)
                    {
                        shortestDistance = distanceEnemy;
                        nearestEnemy = enemy.gameObject;
                    }
                }

            }

            if (nearestEnemy != null && shortestDistance <= viewRadius)
            {
                if (nearestEnemy.TryGetComponent(out PlayerBody playerBody))
                {
                    if (playerBody.body == PlayerBodyName.Body)
                    {
                        setup.objects.futureEnemy = nearestEnemy.transform;
                        setup.memory.OnlyEnemyHeadVisible = false;
                    }
                    else
                    {
                        setup.objects.futureEnemy = playerBody.player.transform;
                        bool bodyVisible = visibleTargets.Contains(playerBody.player.transform);
                        setup.memory.OnlyEnemyHeadVisible = !bodyVisible;
                    }
                }
            }
            else
            {
                setup.objects.futureEnemy = null;
                setup.objects.enemy = null;
                setup.memory.EnemyPassed = false;

            }
        }

        void FindVisibleDiedAlly()
        {

            foreach (BotAuthor author in authors)
            {
                if (author.transform == setup.author.transform) continue;
                if (author.transform.GetInstanceID() == setup.author.transform.GetInstanceID()) continue;
                if (author.setup.health.died && author.setup.status.isBuried == false)
                {
                    List<Body> visibleBody = new();
                    foreach (Body body in author.bodies)
                    {
                        Vector3 dir = body.transform.position - transform.position;
                        Vector3 dirNormalized = (body.transform.position - transform.position).normalized;
                        float bodyDistance = Vector3.Distance(transform.position, body.transform.position);
                        bool inAngle = Vector3.Angle(transform.forward, dirNormalized) < diedAllyViewAngle / 2;
                        if (!inAngle) continue;
                        bool visible = !Physics.Raycast(transform.position, dir, out _, bodyDistance, obstacleMask);
                        if (visible) visibleBody.Add(body);
                    }
                    float visibleCount = (float)visibleBody.Count;
                    float allBodyCount = 11;
                    if (visibleCount > 0)
                    {
                        float percentage = visibleCount / allBodyCount;
                        float newViewRadius = (viewRadius / 5) * percentage;
                        float bodyDistance = Vector3.Distance(transform.position, author.setup.transform.position);
                        if (newViewRadius > bodyDistance)
                        {
                            Call(author);
                        }
                    }
                }

            }

            void Call(BotAuthor allyAuthor)
            {
                StartCoroutine(allyAuthor.setup.status.SetBurried());
                setup.botAudio.Play(PanicTalking.DEAD_ALARM, setup);
                setup.memory.EnemyIsAround = true;
                if (setup.objects.enemy == null)
                {
                    setup.status.Purpose = BotEnum.Purpose.Patrolling;
                    setup.status.MentalState = BotEnum.MentalState.Panic;
                }
                if (Time.time - allyAuthor.setup.memory.timeToDied <= 2)
                {
                    setup.memory.suspectionID = Random.Range(1000000, 9999999);
                    setup.objects.ChangeSuspectionPoint(allyAuthor.setup.memory.whoKilled.position);
                    if (!allyAuthor.setup.status.outOfCount)
                    {
                        allyAuthor.setup.status.outOfCount = true;
                        Debug.Log("Called To Died Enemy This: Caller is = " + setup.transform.parent.parent.name);
                        setup.utility.CallingToDiedEnemy(allyAuthor.setup.memory.whoKilled);
                        setup.overall.SetLastEnemyVisiblePoint(allyAuthor.setup.memory.whoKilled.position);
                        setup.overall.SetDangerAreas(allyAuthor.setup.memory.whoKilled.position, 30, true);
                        setup.author.SetAlarmSignal(true);
                    }
                }

                else
                {
                    setup.memory.suspectionID = Random.Range(1000000, 9999999);
                    setup.objects.ChangeSuspectionPoint(allyAuthor.setup.transform.position);
                    setup.overall.SetDangerAreas(allyAuthor.setup.transform.position, 30, true);
                    if (!allyAuthor.setup.status.outOfCount)
                    {
                        allyAuthor.setup.status.outOfCount = true;
                        Debug.Log("Called To Died Enemy Null: Caller is = " + setup.transform.parent.parent.name);
                        setup.utility.CallingToDiedEnemy(allyAuthor.setup.transform);
                        setup.author.SetAlarmSignal(true);
                    }
                }
            }

            if (true) return;

            Collider[] diedAllyInViewRadius;
            for (int i = 0; i < diedAllyInViewRadius.Length; i++)
            {
                Transform ally = diedAllyInViewRadius[i].transform;
                if (!ally.CompareTag("Live/Evil")) continue;
                if (ally == setup.transform) continue;
                if (ally.GetInstanceID() == setup.transform.GetInstanceID()) continue;

                Health health = ally.GetComponent<Health>();
                if (!health) continue;
                if (!health.died) continue;
                BotStatus status = ally.GetComponent<BotStatus>();
                if (!status) continue;
                if (status.isBuried) continue;
                BotBase memory = ally.GetComponent<BotBase>();
                Vector3 dirToAlly = (ally.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToAlly) < viewAngle / 2)
                {
                    float dstToAlly = Vector3.Distance(transform.position, ally.position);
                    if (!Physics.Raycast(transform.position, dirToAlly, dstToAlly, obstacleMask))
                    {
                        StartCoroutine(status.SetBurried());
                        memory.allyIsKilled = true;
                        setup.botAudio.Play(PanicTalking.DEAD_ALARM, setup);
                        if (setup.objects.enemy == null)
                        {
                            setup.status.Purpose = BotEnum.Purpose.Patrolling;
                            setup.status.MentalState = BotEnum.MentalState.Panic;
                        }
                        setup.memory.EnemyIsAround = true;
                        if (Time.time - memory.timeToDied <= 2)
                        {
                            setup.memory.suspectionID = Random.Range(1000000, 9999999);
                            setup.objects.ChangeSuspectionPoint(memory.whoKilled.position);
                            if (!status.outOfCount)
                            {
                                status.outOfCount = true;
                                Debug.Log("Called To Died Enemy This: Caller is = " + setup.transform.parent.parent.name);
                                setup.utility.CallingToDiedEnemy(memory.whoKilled);
                                setup.overall.SetLastEnemyVisiblePoint(memory.whoKilled.position);
                                setup.overall.SetDangerAreas(memory.whoKilled.position, 30, true);
                                setup.author.SetAlarmSignal(true);
                            }
                        }
                        else
                        {
                            setup.memory.suspectionID = Random.Range(1000000, 9999999);
                            setup.objects.ChangeSuspectionPoint(ally.position);
                            setup.overall.SetDangerAreas(ally.position, 30, true);
                            if (!status.outOfCount)
                            {
                                status.outOfCount = true;
                                Debug.Log("Called To Died Enemy Null: Caller is = " + setup.transform.parent.parent.name);
                                setup.utility.CallingToDiedEnemy(ally);
                                setup.author.SetAlarmSignal(true);
                            }
                        }

                        break;
                    }
                }
            }
        }

        List<Transform> hidingPoints = new List<Transform>();
        public Transform GetHidingPosition()
        {
            hidingPoints.Clear();
            Collider[] pointsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, hidingMask);

            for (int i = 0; i < pointsInViewRadius.Length; i++)
            {
                Transform point = pointsInViewRadius[i].transform;
                Vector3 dirToTarget = (point.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    hidingPoints.Add(point);
                }
            }

            setup.objects.getOneTimeHidingPosition = false;
            int randomPosition = Random.Range(0, hidingPoints.Count - 1);
            if (hidingPoints.Count > 0)
            {
                return hidingPoints[randomPosition];
            }
            else
            {
                return null;
            }
        }
        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }

}

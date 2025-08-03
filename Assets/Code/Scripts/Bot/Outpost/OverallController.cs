using System.Collections.Generic;
using PlayerRoot;
using UnityEngine;
using MissionManager;

namespace BotRoot
{
    public class OverallController : MonoBehaviour
    {
        public float EffectRadius = 200;
        public LayerMask botMask;
        public LayerMask patrollPointMask;
        public List<BotAuthor> authors;
        public List<PatrollingPoint> patrollingPoints;
        public MissionSequence missionManager;
        [HideInInspector] public bool IsSpawnable = true;
        [HideInInspector] public AgentPathFinder pathFinder;
        [HideInInspector] public Player player;
        [HideInInspector] public Vector3 lastSuspectionPoint;
        [HideInInspector] public Vector3 lastDangerPoint;
        [HideInInspector] public Vector3 lastEnemyPoint;

        public Transform LastEnemyPointTransform;

        public bool IsLastEnemyPointChecked = true;
        public bool IsLastDiedPointChecked = true;
        public bool EnemyVisibled = false;

        public Transform spawnerParent;
        public Transform patrollingParent;

        [Header("Indicator Params")]
        [SerializeField] GameObject IndicatorParent;
        [SerializeField] GameObject IndicatorPrefab;

        bool initialized = false;

        [System.Obsolete]
        public void Initialize()
        {
            if(initialized) return;

            initialized = true;
            if (IsSpawnable) Spawn();
            else return;
            RelistBot();
            foreach (Transform point in patrollingParent)
            {
                if (point.TryGetComponent(out PatrollingPoint patrollingPoint))
                {
                    patrollingPoints.Add(patrollingPoint);
                }
            }

            if (TryGetComponent(out AgentPathFinder component))
            {
                pathFinder = component;
                if (authors.Count > 0)
                    pathFinder.agent = authors[0].setup.transform;
                else
                    pathFinder.agent = transform;
            }

            player = FindFirstObjectByType<Player>();
            if (missionManager) missionManager.Initialize();
            //InvokeRepeating(nameof(ChangePatrollingStrategy), 3, 5);
        }

        void Spawn()
        {
            foreach (Transform spawnerTransform in spawnerParent)
            {
                if (spawnerTransform.TryGetComponent(out BotSpawner spawner))
                {
                    spawner.Spawn(this);
                }
            }
        }

        public void Unspawn()
        {
            foreach (Transform spawnerTransform in spawnerParent)
            {
                if (spawnerTransform.TryGetComponent(out BotSpawner spawner))
                {
                    spawner.Unspawn();
                }
            }
            initialized = false;
        }

        void Update()
        {
            LastEnemyPointTransform.position = lastEnemyPoint;
            if (IsLastEnemyPointChecked)
            {
                LastEnemyPointTransform.gameObject.SetActive(false);
            }
            else
            {
                LastEnemyPointTransform.gameObject.SetActive(true);
            }
        }

        public void RelistBot()
        {
            authors = new();
            Collider[] botsCollider = Physics.OverlapSphere(transform.position, EffectRadius, botMask);
            foreach (Collider collider in botsCollider)
            {
                if (collider.TryGetComponent(out BotAuthor author))
                {
                    if (author.setup.health.died) continue;
                    author.setup.overall = this;
                    authors.Add(author);
                }
            }
            SetCaptain();
        }

        public void SetCaptain()
        {
            int index = 0;
            foreach (BotAuthor author in authors)
            {
                if (index == 0) author.setup.status.Captain = true;
                else author.setup.status.Captain = false;
                index++;
            }
        }

        public bool IsAreaSafe()
        {
            if (IsLastEnemyPointChecked == false) return false;
            if (IsLastDiedPointChecked == false) return false;

            foreach (PatrollingPoint point in patrollingPoints)
            {
                if (!point.SafePoint) return false;
            }
            return true;
        }

        public void SetDangerAreas(Vector3 dangerPosition, float dangerRadiusDistance, bool diedBody)
        {
            float changedDistance = Vector3.Distance(lastDangerPoint, dangerPosition);
            if (changedDistance < 10) return;

            IsLastDiedPointChecked = diedBody ? false : true;
            lastDangerPoint = dangerPosition;
            foreach (PatrollingPoint point in patrollingPoints)
            {
                float distance = Vector3.Distance(dangerPosition, point.transform.position);
                if (distance <= dangerRadiusDistance)
                {
                    point.SetAreaIsDanger();
                }
                else
                {
                    point.SetAreaIsSafe();
                }
            }
        }

        public void SetLastEnemyVisiblePoint(Vector3 lastEnemyPosition)
        {
            Debug.Log("Call: Last Enemy Point");

            float changedDistance = Vector3.Distance(lastEnemyPoint, lastEnemyPosition);
            lastEnemyPoint = lastEnemyPosition;
            IsLastEnemyPointChecked = false;
            EnemyVisibled = true;

            if (changedDistance < 10) return;
            foreach (PatrollingPoint point in patrollingPoints)
            {
                float distance = Vector3.Distance(lastEnemyPosition, point.transform.position);
                if (distance <= 30)
                {
                    point.SetAreaIsDanger();
                }
                else
                {
                    point.SetAreaIsSafe();
                }
            }

        }

        public void PatrollingStrategyBuilder(Vector3 suspectionPoint)
        {
            float changedDistance = Vector3.Distance(lastSuspectionPoint, suspectionPoint);
            Debug.Log("-------------------------------------| " + changedDistance);
            if (changedDistance < 10) return;

            lastSuspectionPoint = suspectionPoint;
            int index = 0;
            foreach (BotAuthor author in authors)
            {
                if (index == 0)
                {
                    author.setup.status.patrollCommand = BotEnum.PatrollCommand.MainPoint;
                }
                else if (index == 1 || index == 2)
                {
                    PatrollingPoint patrollingPoint = FindClosestPoint(author.setup.transform, suspectionPoint);
                    if (patrollingPoint)
                    {
                        author.setup.objects.patrolling = patrollingPoint;
                        author.setup.status.patrollCommand = BotEnum.PatrollCommand.PatrollPoint;
                        patrollingPoint.SetIsBusy();
                    }
                    else
                    {
                        author.setup.status.patrollCommand = BotEnum.PatrollCommand.Avoid;
                    }
                }
                else
                {
                    author.setup.status.patrollCommand = BotEnum.PatrollCommand.Avoid;
                }
                index++;
            }
        }

        // public void ChangePatrollingStrategy()
        // {
        //     int index = 0;
        //     foreach (BotAuthor author in authors)
        //     {
        //         if (CountByCommand(BotEnum.PatrollCommand.MainPoint) == 0)
        //         {
        //             author.setup.status.patrollCommand = BotEnum.PatrollCommand.MainPoint;
        //         }
        //         else if (CountByCommand(BotEnum.PatrollCommand.PatrollPoint) < 2)
        //         {
        //             PatrollingPoint patrollingPoint = FindClosestPoint(author.setup.transform, lastSuspectionPoint);
        //             if (patrollingPoint)
        //             {
        //                 author.setup.objects.patrolling = patrollingPoint;
        //                 author.setup.status.patrollCommand = BotEnum.PatrollCommand.PatrollPoint;
        //                 patrollingPoint.SetIsBusy();
        //             }
        //             else
        //             {
        //                 author.setup.status.patrollCommand = BotEnum.PatrollCommand.Avoid;
        //             }
        //         }
        //         else
        //         {
        //             author.setup.status.patrollCommand = BotEnum.PatrollCommand.Avoid;
        //         }
        //         index++;
        //     }
        // }

        int CountByCommand(BotEnum.PatrollCommand patrollCommand)
        {
            int count = 0;
            foreach (BotAuthor author in authors)
            {
                if (author.setup.status.patrollCommand == patrollCommand)
                {
                    count++;
                }
            }
            return count;
        }

        public PatrollingPoint FindClosestPoint(Transform bot, Vector3 suspectionPoint)
        {
            PatrollingPoint closestPoint = null;
            float closestDistance = Mathf.Infinity;

            foreach (PatrollingPoint point in patrollingPoints)
            {

                if (point.IsBusy) continue;
                if (point.SafePoint) continue;
                float distance = Vector3.Distance(bot.position, point.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = point;
                }
            }

            if (closestPoint)
            {
                float suspectionDistance = Vector3.Distance(closestPoint.transform.position, suspectionPoint);
                if (suspectionDistance > 100)
                {
                    return null;
                }
                else
                {
                    return closestPoint;
                }
            }
            else
            {
                return null;
            }


        }

        public int CountOfPatrollingMainPoint()
        {
            int count = 0;
            foreach (BotAuthor author in authors)
            {
                if (author.setup.status.patrollCommand == BotEnum.PatrollCommand.MainPoint) count++;
            }
            return count;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace BotRoot
{
    public class AgentUtility : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        public int currentPathId = 0;
        public int nextPathId = 1;
        public int moveCorner = 1;
        public Vector3 nextCornerPosition;
        public List<Vector3> pathCorners = new List<Vector3>();
        [HideInInspector] public bool pathCompleted;
        [HideInInspector] public bool NoPath = false;
        [HideInInspector] public NavMeshPathStatus pathStatus;
        [HideInInspector] public bool usedPathFinder = false;

        [Space]
        [Header("Invalid Path Finder")]

        public int circleRayCount = 32;
        public int multipleCircleCount = 3;
        public float multipleLength = 3;
        public float height = 1;

#if UNITY_EDITOR
        public void Update()
        {
            DebugPath(pathCorners);
        }
#endif
        public bool AgentHasPath(Vector3 target)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (
                path.status == NavMeshPathStatus.PathPartial ||
                path.status == NavMeshPathStatus.PathInvalid
            )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetMoveIndicator(Vector3 target)
        {

        }

        public void CreatePathCorners(Vector3 target, OverallController overallController, BotSetup setup)
        {
            if (currentPathId == nextPathId) return;
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            pathCompleted = false;
            usedPathFinder = false;
            pathStatus = path.status;
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                pathCorners.Clear();
                foreach (var item in path.corners)
                {
                    pathCorners.Add(item);
                }
                currentPathId = nextPathId;
                moveCorner = 1;
                pathCompleted = true;
                DebugPath(pathCorners);
                NoPath = false;
                usedPathFinder = false;
            }
            else
            {
                if (path.status == NavMeshPathStatus.PathPartial)
                {
                    Vector3 targetClosePoint = overallController.pathFinder.GetWalkablePointByCenter(target);
                    usedPathFinder = true;
                    pathCompleted = false;
                    if (overallController.pathFinder.FindPathCompleted)
                    {
                        if (overallController.pathFinder.PathInvalid)
                        {
                            Debug.LogWarning("Path: Invalid By AgentPathFinder, Caller: " + transform.parent.parent.parent.name);
                            NoPath = true;
                            pathCompleted = true;
                        }
                        else
                        {
                            NavMesh.CalculatePath(transform.position, targetClosePoint, NavMesh.AllAreas, path);
                            pathCompleted = false;
                            pathStatus = path.status;
                            if (path.status == NavMeshPathStatus.PathComplete)
                            {
                                pathCorners.Clear();
                                foreach (var item in path.corners)
                                {
                                    pathCorners.Add(item);
                                }
                                setup.movePosition.SetPositionStealthly(targetClosePoint);
                                currentPathId = nextPathId;
                                moveCorner = 1;
                                pathCompleted = true;
                                DebugPath(pathCorners);
                                NoPath = false;
                            }
                        }

                        // if(setup.objects.enemy)
                        // {
                        //     overallController.SetLastEnemyVisiblePoint(targetClosePoint);
                        // }
                        // else
                        // {
                        //     if(setup.memory.frightened)
                        //     {
                        //         float distance = Vector3.Distance(setup.objects.suspicionPoint.position, overallController.player.transform.position);
                        //         if(distance < 1)
                        //         {
                        //             overallController.SetLastEnemyVisiblePoint(targetClosePoint);
                        //         }
                        //     }
                        // }
                    }
                    else
                    {
                        Debug.LogWarning("Waiting Path Complete: " + transform.parent.parent.parent.name);
                    }
                }
                else if (path.status == NavMeshPathStatus.PathInvalid)
                {
                    if (NavMesh.SamplePosition(target, out NavMeshHit navHit, 1, NavMesh.AllAreas))
                    {
                        usedPathFinder = false;
                        NavMesh.CalculatePath(transform.position, navHit.position, NavMesh.AllAreas, path);
                        pathCompleted = false;
                        pathStatus = path.status;
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            pathCorners.Clear();
                            foreach (var item in path.corners)
                            {
                                pathCorners.Add(item);
                            }
                            currentPathId = nextPathId;
                            moveCorner = 1;
                            pathCompleted = true;

                            DebugPath(pathCorners);
                            NoPath = false;
                        }
                        else
                        {
                            Vector3 targetClosePoint = overallController.pathFinder.GetWalkablePointByCenter(target);
                            usedPathFinder = true;
                            pathCompleted = false;
                            if (overallController.pathFinder.FindPathCompleted)
                            {
                                if (overallController.pathFinder.PathInvalid)
                                {
                                    Debug.LogWarning("Path: Invalid By AgentPathFinder, Caller: " + transform.parent.parent.parent.name);
                                    NoPath = true;
                                    pathCompleted = true;
                                }
                                else
                                {
                                    NavMesh.CalculatePath(transform.position, targetClosePoint, NavMesh.AllAreas, path);
                                    pathCompleted = false;
                                    pathStatus = path.status;
                                    if (path.status == NavMeshPathStatus.PathComplete)
                                    {
                                        pathCorners.Clear();
                                        foreach (var item in path.corners)
                                        {
                                            pathCorners.Add(item);
                                        }
                                        setup.movePosition.SetPositionStealthly(targetClosePoint);
                                        currentPathId = nextPathId;
                                        moveCorner = 1;
                                        pathCompleted = true;
                                        DebugPath(pathCorners);
                                        NoPath = false;
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Waiting Path Complete: " + transform.parent.parent.parent.name);
                            }
                        }
                    }
                    else
                    {
                        Vector3 targetClosePoint = overallController.pathFinder.GetWalkablePointByCenter(target);
                            usedPathFinder = true;
                            pathCompleted = false;
                            if (overallController.pathFinder.FindPathCompleted)
                            {
                                if (overallController.pathFinder.PathInvalid)
                                {
                                    Debug.LogWarning("Path: Invalid By AgentPathFinder, Caller: " + transform.parent.parent.parent.name);
                                    NoPath = true;
                                    pathCompleted = true;
                                }
                                else
                                {
                                    NavMesh.CalculatePath(transform.position, targetClosePoint, NavMesh.AllAreas, path);
                                    pathCompleted = false;
                                    pathStatus = path.status;
                                    if (path.status == NavMeshPathStatus.PathComplete)
                                    {
                                        pathCorners.Clear();
                                        foreach (var item in path.corners)
                                        {
                                            pathCorners.Add(item);
                                        }
                                        setup.movePosition.SetPositionStealthly(targetClosePoint);
                                        currentPathId = nextPathId;
                                        moveCorner = 1;
                                        pathCompleted = true;
                                        DebugPath(pathCorners);
                                        NoPath = false;
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Waiting Path Complete: " + transform.parent.parent.parent.name);
                            }
                    }
                }
                else
                {
                    Debug.LogWarning("Path: Invalid Another Type");
                    pathCompleted = true;
                    NoPath = true;
                }
            }
        }

        public Vector3 GetNextCorner()
        {
            if (pathCorners.Count == 1 || pathCorners.Count == 0)
            {
                return transform.position;
            }
            else
            {
                Vector3 _currentCorner = pathCorners[moveCorner];
                float distance = Vector3.Distance(transform.position, _currentCorner);
                if (distance <= 0.25f)
                {
                    if (moveCorner == pathCorners.Count - 1)
                    {
                        return _currentCorner;
                    }
                    else
                    {
                        moveCorner++;
                        return pathCorners[moveCorner];
                    }
                }
                else
                {
                    return _currentCorner;
                }
            }
        }
        public Vector3 GetLastCorner()
        {
            if (pathCorners.Count == 1 || pathCorners.Count == 0)
            {
                return transform.position;
            }
            else
            {
                Vector3 lastCorner = pathCorners[pathCorners.Count-1];
                return lastCorner;
            }
        }
        void DebugPath(List<Vector3> path)
        {
#if UNITY_EDITOR
            for (var i = 1; i < path.Count; i++)
            {
                Vector3 start = path[i - 1];
                Vector3 end = path[i];
                Color lineColor = Color.green;
                Color pointColor = Color.yellow;
                Debug.DrawLine(start, end, lineColor);
                //DrawLine(start, start+(Vector3.up*0.5f), 5, pointColor);
            }
#endif //UNITY_EDITOR

        }

        // void OnDrawGizmos()
        // {
        //     if (pathCorners.Count > 1)
        //     {
        //         for (var i = 0; i < pathCorners.Count; i++)
        //         {
        //             Vector3 start = pathCorners[i];
        //             Gizmos.color = Color.red;
        //             Gizmos.DrawSphere(start, 0.1f);
        //             UnityEditor.Handles.color = Color.yellow;
        //             UnityEditor.Handles.DrawLine(start, start + (Vector3.up * 0.5f), 3);
        //         }
        //     }
        // }
    }
}

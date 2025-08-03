using System.Collections;
using System.Collections.Generic;
using BotRoot;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class AgentPathFinder : MonoBehaviour
{
    [Space]
    [Header("Invalid Path Finder")]

    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private int circleRayCount = 32;
    [SerializeField] private int multipleCircleCount = 3;
    [SerializeField] private float rayLength = 3;
    [SerializeField] private float downRayLength;
    [SerializeField] private float height = 1;

    [HideInInspector] public OverallController overallController;
    [HideInInspector] public Transform agent;
    [HideInInspector] public Vector3 target;
    [HideInInspector] public bool PathInvalid = true;
    [HideInInspector] public bool FindPathCompleted = false;
    [HideInInspector] public bool EnemyIsInPartial = false;
    [HideInInspector] public List<Vector3> walkablePoints = new();

    [HideInInspector] public bool IsLastWalkablePointIsGenerated = false;
    [HideInInspector] public Vector3 LastWalkablePoint = new();

    private float lastUpdatedTime = -1;
    private bool _findPathCompleted = false;

    public Vector3 GetWalkablePointByCenter(Vector3 targetPosition)
    {
        CancelInvoke(nameof(EnemyNoPartial));
        Invoke(nameof(EnemyNoPartial), 1);
        EnemyIsInPartial = true;
        target = targetPosition;
        FindPathCompleted = false;
        if (lastUpdatedTime + 1 < Time.time)
        {
            // You Can Update
            lastUpdatedTime = Time.time;
            walkablePoints = GetGeneratedPoints();
            if (_findPathCompleted)
            {
                FindPathCompleted = true;
                if (walkablePoints.Count > 0)
                {
                    IsLastWalkablePointIsGenerated = true;
                    LastWalkablePoint = walkablePoints[GetCenterPointIndex(walkablePoints)];
                    return LastWalkablePoint;
                }
                else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                FindPathCompleted = false;
                PathInvalid = true;
                return Vector3.zero;
            }
        }
        else
        {
            if (walkablePoints.Count > 0)
            {
                FindPathCompleted = true;
                return walkablePoints[GetCenterPointIndex(walkablePoints)];
            }

            else
            {
                FindPathCompleted = true;
                PathInvalid = true;
                return Vector3.zero;
            }
        }
    }

    void EnemyNoPartial()
    {
        EnemyIsInPartial = false;
    }

    public List<Vector3> GetPoints(Vector3 targetPosition)
    {
        EnemyIsInPartial = true;
        target = targetPosition;
        FindPathCompleted = false;
        if (lastUpdatedTime + 1 < Time.time)
        {
            // You Can Update
            lastUpdatedTime = Time.time;
            walkablePoints = GetGeneratedPoints();
            if (_findPathCompleted)
            {
                FindPathCompleted = true;
                return walkablePoints;
            }
            else
            {
                FindPathCompleted = false;
                return walkablePoints;
            }
        }
        else
        {
            FindPathCompleted = true;
            return walkablePoints;
        }
    }

    List<Vector3> GetGeneratedPoints()
    {
        for (int a = 0; a < multipleCircleCount; a++)
        {
            List<Vector3> completedPoints = new();
            float newRayLenght = rayLength * (a + 1);
            for (int i = 0; i < circleRayCount; i++)
            {
                float angle = i * (360f / circleRayCount);
                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 directionDown = Quaternion.Euler(0, angle, 0) * Vector3.down;
                Vector3 startPoint = target + Vector3.up * height;
                Vector3 endPoint = startPoint + direction * newRayLenght;

                if (Physics.Raycast(startPoint, direction, out RaycastHit hit, newRayLenght, obstacleMask)) // Side Hit
                {
                    /// Invalid Collision
                }
                else
                {
                    if (Physics.Raycast(endPoint, directionDown, out RaycastHit hit2, downRayLength, obstacleMask)) // Down Hit
                    {
                        //UnityEditor.Handles.color = Color.red;
                        NavMeshPath path = CalculatePath(hit2.point);
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            Vector3 endPointDirection = hit2.point - startPoint;
                            float endPointDistance = Vector3.Distance(startPoint, hit2.point);
                            if (Physics.Raycast(startPoint, endPointDirection, out RaycastHit hit3, endPointDistance - 0.3f, obstacleMask))
                            {
                                /// Invalid Collision
                            }
                            else
                            {
                                /// Has Path
                                completedPoints.Add(hit2.point);
                            }
                        }
                    }
                }
            }
            if (completedPoints.Count > 0)
            {
                PathInvalid = false;
                return completedPoints;
            }
        }
        PathInvalid = true;
        return new();
    }

    NavMeshPath GetPathOnInvalid()
    {
        for (int a = 0; a < multipleCircleCount; a++)
        {
            List<Vector3> completedPoints = new();
            List<NavMeshPath> paths = new();
            float newRayLenght = rayLength * (a + 1);
            for (int i = 0; i < circleRayCount; i++)
            {
                float angle = i * (360f / circleRayCount);
                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 directionDown = Quaternion.Euler(0, angle, 0) * Vector3.down;
                Vector3 startPoint = target + Vector3.up * height;
                Vector3 endPoint = startPoint + direction * newRayLenght;
                //Vector3 endDownPoint = endPoint + (Vector3.down * downRayLength);

                if (Physics.Raycast(startPoint, direction, out RaycastHit hit, newRayLenght, obstacleMask)) // Side Hit
                {
                    /// Invalid Collision
                }
                else
                {
                    if (Physics.Raycast(endPoint, directionDown, out RaycastHit hit2, downRayLength, obstacleMask)) // Down Hit
                    {
                        //UnityEditor.Handles.color = Color.red;
                        NavMeshPath path = CalculatePath(hit2.point);
                        if (path == null)
                        {
                            Debug.LogWarning("The path is null or incomplete!: " + path);
                        }
                        else if (path.status == NavMeshPathStatus.PathInvalid)
                        {
                            Debug.LogWarning("The path is Invalid!: " + path);
                        }
                        else if (path.status == NavMeshPathStatus.PathPartial)
                        {
                            Debug.LogWarning("The path is Partial!: " + path);
                        }
                        else if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            Vector3 endPointDirection = hit2.point - startPoint;
                            float endPointDistance = Vector3.Distance(startPoint, hit2.point);
                            if (Physics.Raycast(startPoint, endPointDirection, out RaycastHit hit3, endPointDistance - 0.3f, obstacleMask))
                            {
                                /// Invalid Collision
                            }
                            else
                            {
                                /// Has Path
                                completedPoints.Add(hit2.point);
                                paths.Add(path);
                            }
                        }
                    }
                }
            }
            if (paths.Count > 0)
            {
                return paths[GetCenterPointIndex(completedPoints)];
            }
        }
        return null;
    }

    int GetCenterPointIndex(List<Vector3> positions)
    {
        if (positions.Count > 0)
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3 point in positions)
            {
                sum += point;
            }

            Vector3 averagePosition = sum / positions.Count;

            float closestDistance = Mathf.Infinity;
            int closestPositionIndex = 0;
            for (int i = 0; i < positions.Count; i++)
            {
                float distance = Vector3.Distance(averagePosition, positions[i]);
                if (distance < closestDistance)
                {
                    closestPositionIndex = i;
                    closestDistance = distance;
                }
            }

            return closestPositionIndex;
        }
        else
        {
            Debug.LogError("No List Position.");
            return 0;
        }
    }

    void OnDrawGizmos()
    {
        // for (int a = 0; a < multipleCircleCount; a++)
        // {
        //     float newRayLenght = rayLength * (a + 1);
        //     bool hasPath = false;
        //     for (int i = 0; i < circleRayCount; i++)
        //     {
        //         float angle = i * (360f / circleRayCount);
        //         Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        //         Vector3 directionDown = Quaternion.Euler(0, angle, 0) * Vector3.down;
        //         Vector3 startPoint = transform.position + Vector3.up * height;
        //         Vector3 endPoint = startPoint + direction * newRayLenght;
        //         Vector3 endDownPoint = endPoint + (Vector3.down * downRayLength);

        //         // Use HitRaycast
        //         if (Physics.Raycast(startPoint, direction, out RaycastHit hit, newRayLenght, obstacleMask))
        //         {
        //             UnityEditor.Handles.color = Color.red;
        //             UnityEditor.Handles.DrawLine(startPoint, hit.point, 3);
        //             UnityEditor.Handles.DrawSolidDisc(hit.point, hit.normal, 0.2f);
        //         }
        //         else
        //         {
        //             if (Physics.Raycast(endPoint, directionDown, out RaycastHit hit2, downRayLength, obstacleMask))
        //             {
        //                 UnityEditor.Handles.color = Color.red;
        //                 NavMeshPath path = GetPath(hit2.point);
        //                 if (path.status == NavMeshPathStatus.PathComplete)
        //                 {
        //                     Vector3 previousCorner = Vector3.zero;
        //                     int index = 0;
        //                     foreach (Vector3 corner in path.corners)
        //                     {
        //                         UnityEditor.Handles.color = Color.yellow;
        //                         UnityEditor.Handles.DrawSolidDisc(corner, Vector3.up, 0.05f);
        //                         if (index == 0)
        //                         {
        //                             previousCorner = corner;
        //                             index++;
        //                             continue;
        //                         }
        //                         Vector3 start = previousCorner;
        //                         Vector3 end = corner;
        //                         previousCorner = corner;
        //                         UnityEditor.Handles.DrawLine(start, end, 3);
        //                     }
        //                     UnityEditor.Handles.color = Color.white;
        //                 }
        //                 else
        //                 {
        //                     UnityEditor.Handles.color = Color.red;
        //                 }

        //                 Vector3 endPointDirection = hit2.point - startPoint;
        //                 float endPointDistance = Vector3.Distance(startPoint, hit2.point);
        //                 if (Physics.Raycast(startPoint, endPointDirection, out RaycastHit hit3, endPointDistance - 0.3f, obstacleMask))
        //                 {
        //                     UnityEditor.Handles.color = Color.red;
        //                     UnityEditor.Handles.DrawLine(startPoint, hit3.point, 1);
        //                     UnityEditor.Handles.DrawSolidDisc(hit.point, hit.normal, 0.05f);

        //                     UnityEditor.Handles.color = Color.white;
        //                     UnityEditor.Handles.DrawLine(startPoint, endPoint, 3);
        //                     UnityEditor.Handles.DrawLine(endPoint, hit2.point, 3);
        //                 }
        //                 else
        //                 {
        //                     /// Has Path
        //                     if (path.status == NavMeshPathStatus.PathComplete)
        //                     {
        //                         UnityEditor.Handles.color = Color.green;
        //                         UnityEditor.Handles.DrawLine(startPoint, hit2.point, 1);
        //                         UnityEditor.Handles.DrawSolidDisc(hit2.point, hit2.normal, 0.2f);

        //                         UnityEditor.Handles.DrawLine(startPoint, endPoint, 3);
        //                         UnityEditor.Handles.DrawLine(endPoint, hit2.point, 3);
        //                         hasPath = true;
        //                     }
        //                     else
        //                     {
        //                         UnityEditor.Handles.color = Color.red;
        //                         UnityEditor.Handles.DrawLine(startPoint, hit2.point, 1);
        //                         UnityEditor.Handles.DrawSolidDisc(hit2.point, hit2.normal, 0.2f);

        //                         UnityEditor.Handles.DrawLine(startPoint, endPoint, 3);
        //                         UnityEditor.Handles.DrawLine(endPoint, hit2.point, 3);
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 UnityEditor.Handles.color = Color.yellow;
        //                 UnityEditor.Handles.DrawLine(startPoint, endPoint, 3);
        //                 UnityEditor.Handles.DrawLine(endPoint, endDownPoint, 3);
        //             }
        //         }
        //     }
        //     if(hasPath)
        //     {
        //         break;
        //     }
        // }
        if (UnityEditor.Selection.activeGameObject == gameObject)
        {
            NavMeshPath navMeshPath = GetPathOnInvalid();
            if (navMeshPath != null)
            {
                if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    Vector3 previousCorner = Vector3.zero;
                    int index = 0;
                    foreach (Vector3 corner in navMeshPath.corners)
                    {
                        //UnityEditor.Handles.color = Color.black;
                        //UnityEditor.Handles.DrawSolidDisc(corner, Vector3.up, 0.15f);
                        if (index == 0)
                        {
                            previousCorner = corner;
                            index++;
                            continue;
                        }
                        Vector3 start = previousCorner;
                        Vector3 end = corner;
                        previousCorner = corner;
                        //UnityEditor.Handles.DrawLine(start, end, 5);
                    }
                }
            }
        }

    }

    public NavMeshPath CalculatePath(Vector3 _target)
    {
        if (!agent) return null;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(agent.position, _target, NavMesh.AllAreas, path);
        _findPathCompleted = false;
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            _findPathCompleted = true;
            return path;
        }
        else
        {
            if (path.status == NavMeshPathStatus.PathPartial)
            {
                _findPathCompleted = true;
                return path;
            }
            else if (path.status == NavMeshPathStatus.PathInvalid)
            {
                if (NavMesh.SamplePosition(_target, out NavMeshHit navHit, 1, NavMesh.AllAreas))
                {
                    NavMesh.CalculatePath(agent.position, navHit.position, NavMesh.AllAreas, path);
                    _findPathCompleted = false;
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        _findPathCompleted = true;
                        return path;
                    }
                    else
                    {
                        if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
                        {
                            _findPathCompleted = true;
                        }
                        return path;
                    }
                }
                return path;
            }
            else
            {
                return path;
            }
        }
    }

    public int CheckPath(Vector3 target)
    {
        NavMeshPath path = new();
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        {
            return -1;
        }
    }
}

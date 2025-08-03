using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace BotRoot
{
    public class BotMovement : MonoBehaviour
    {
        public Transform MoveIndicator;
        public Animator animator;
        [HideInInspector] public BotSetup setup;
        BotObjects objects;
        BotUtility utility;
        BotStatus status;
        AgentUtility agent;
        MovementUtility movement;
        void Awake()
        {
            objects = setup.objects;
            utility = setup.utility;
            status = setup.status;
            agent = setup.agent;
            movement = setup.movementUtility;

        }

        void Update()
        {
            //float angle = Angle(transform, MoveIndicator, 0.3f);
            //float distance = Vector3.Distance(transform.position, MoveIndicator.position);
            //print(angle);
            //MoveToAngle(angle, distance);
        }

        public void Move(Transform point)
        {

            if (status.Purpose == BotEnum.Purpose.Alarm)
            {
                //setup.author.Message("Move: Alarm", 12);
            }
            else if (status.Purpose == BotEnum.Purpose.Attacking)
            {
                //setup.author.Message("Move: Attack", 12);
                AttackingMovement(point);
            }
            else if (status.Purpose == BotEnum.Purpose.Avoid)
            {
                //setup.author.Message("Move: Avoid", 12);
            }
            else if (status.Purpose == BotEnum.Purpose.Checking)
            {
                //setup.author.Message("Move: Checking", 12);
                CheckingMovement(point);
            }
            else if (status.Purpose == BotEnum.Purpose.Patrolling)
            {
                //setup.author.Message("Move: Patrolling", 12);
                if (status.patrollCommand == BotEnum.PatrollCommand.MainPoint)
                {
                    if(setup.overall.IsLastEnemyPointChecked)
                    {
                        CheckingMovement(point);
                    }
                    else
                    {
                        AttackingMovement(point);
                    }
                }
                else
                {
                    CheckingMovement(point);
                }
            }
            else if (status.Purpose == BotEnum.Purpose.Command)
            {
                //setup.author.Message("Move: Command", 12);
                CommandMovement(point);
            }
            else
            {
                //setup.author.Message("Move: Noting", 12);
                //Stop;
            }
        }


        void CommandMovement(Transform point)
        {
            if (status.Command == BotEnum.Command.Attacking)
            {

            }
            else if (status.Command == BotEnum.Command.Delivery)
            {

            }
            else if (status.Command == BotEnum.Command.Guarding)
            {
                GuardingMovement(point);
            }
            else if (status.Command == BotEnum.Command.Working)
            {

            }
            else if (status.Command == BotEnum.Command.Special)
            {

            }
            else
            {
                //Stop
            }
        }
        public bool strafeShake = false;
        public void AttackingIdle()
        {
            if (objects.enemy)
            {
                utility.RootLookAt(objects.enemy, 5);
                if (strafeShake)
                {
                    print("STRAFEEEE SHAPE");
                    //animator.CrossFade("Strafe Shake 1", 0.1f, -1, 0);
                    animator.Play("Strafe Shake 1");
                    strafeShake = true;
                }
            }
        }
        void GuardingMovement(Transform point)
        {
            agent.CreatePathCorners(point.position, setup.overall, setup);
            if (!agent.pathCompleted) return;
            bool isPathInvalid = agent.pathStatus == NavMeshPathStatus.PathInvalid;
            bool isPathPartial = agent.pathStatus == NavMeshPathStatus.PathPartial;

            if (!isPathInvalid && !isPathPartial)
            {
                Vector3 MoveToCorner = agent.GetNextCorner();

                objects.moveIndicator.position = MoveToCorner;
                movement.LookAndWalk(objects.moveIndicator, false);
            }
            else
            {
                Debug.LogWarning("No Path");
            }
        }
        void CheckingMovement(Transform point)
        {
            agent.CreatePathCorners(point.position, setup.overall, setup);
            if (!agent.pathCompleted) return;
            bool isPathInvalid = agent.pathStatus == NavMeshPathStatus.PathInvalid;
            bool isPathPartial = agent.pathStatus == NavMeshPathStatus.PathPartial;

            if (!isPathInvalid && !isPathPartial && !agent.NoPath)
            {
                Vector3 MoveToCorner = agent.GetNextCorner();
                objects.moveIndicator.position = MoveToCorner;
                movement.LookAndWalk(objects.moveIndicator, true);

                float currentCrouchValue = Mathf.Lerp(animator.GetFloat("Crouch"), 1, 0.05f);
                animator.SetFloat("Crouch", currentCrouchValue);
            }
            else
            {
                Debug.LogWarning("No Path");
            }
        }
        public void CheckingIdle()
        {
            float currentCrouchValue = Mathf.Lerp(animator.GetFloat("Crouch"), 1, 0.05f);
            animator.SetFloat("Crouch", currentCrouchValue);
            if (setup.memory.EnemyIsAround)
            {
                movement.LookAndDo(objects.suspicionPoint, true, " Idle");
            }
            else
            {
                movement.LookAndDo(objects.suspicionPoint, true, " Idle");
            }
        }
        bool frightening = false;
        void AttackingMovement(Transform point)
        {
            if (!setup.memory.frightened)
            {
                if (!frightening)
                {
                    string[] panicTransform = { "Panic Transform 1", "Panic Transform 2", "Panic Transform 3" };
                    animator.CrossFade(panicTransform[Random.Range(0, panicTransform.Length)], 0.1f, -1, 0);
                    frightening = true;
                }
                else
                {
                    if (objects.enemy) utility.RootLookAt(objects.enemy, 5);
                    else utility.RootLookAt(objects.suspicionPoint, 5);
                }
            }
            else
            {
                if (objects.enemy) utility.RootLookAt(objects.enemy, 5);
                else
                {
                    if(setup.overall.IsLastEnemyPointChecked)
                    {
                        utility.RootLookAt(objects.suspicionPoint, 5);
                    }else
                    {
                        utility.RootLookAt(setup.overall.lastEnemyPoint, 5);
                    }
                }

                agent.CreatePathCorners(point.position, setup.overall, setup);
                if (!agent.pathCompleted)
                {
                    Debug.LogWarning("Path Not Completed");
                    return;
                }
                bool isPathInvalid = agent.pathStatus == NavMeshPathStatus.PathInvalid;
                bool isPathPartial = agent.pathStatus == NavMeshPathStatus.PathPartial;
                if (isPathInvalid || isPathPartial || agent.NoPath)
                {
                    Debug.LogWarning("No Path");
                    setup.author.Message("No Path: Attack Movement");
                    return;
                }

                Vector3 MoveToCorner = agent.GetNextCorner();
                if (objects.enemy)
                {
                    objects.moveIndicator.position = MoveToCorner;
                    //setup.author.Message("move: "+objects.moveIndicator.position, 12);
                    movement.PanicMovement(objects.moveIndicator, true, true, setup.attribute.StopDistanceInAttack);
                }
                else
                {
                    objects.moveIndicator.position = MoveToCorner;
                    //setup.author.Message("move: "+objects.moveIndicator.position, 12);
                    movement.PanicMovement(objects.moveIndicator, true, true, 0.2f);
                }

            }
        }
    }

}

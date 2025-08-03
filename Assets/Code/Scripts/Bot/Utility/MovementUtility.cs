using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class MovementUtility : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        [SerializeField] LayerMask obstacleLayer;
        BotUtility utility;
        AgentUtility agent;
        BotObjects objects;
        Animator animator;
        [SerializeField] Transform perspectionListener;


        bool walking;

        private void Start()
        {
            utility = setup.utility;
            agent = setup.agent;
            objects = setup.objects;
            animator = setup.GetComponent<Animator>();
        }

        public void WalkToStop()
        {
            walking = false;
            CrossFadeTime("Walk To Stop", 0.5f, false);
        }
        public void WalkToStopCrouch()
        {
            walking = false;
            CrossFadeTime("Avoid Idle", 0.5f, false);
        }
        public void PanicMovement(Transform moveIndicator, bool LookAtTarget, bool maxSpeed, float stopDistance)
        {
            if (LookAtTarget)
            {
                float angle = Angle(transform, moveIndicator);
                Vector3 movePos = maxSpeed ? objects.movePoint.position : moveIndicator.position;
                float distance = Vector3.Distance(transform.position, movePos);
                //setup.author.Message("Angle: " + Angle(transform, moveIndicator), 15);
                MoveToAngle(angle, distance, stopDistance);
            }
            else
            {

            }
        }
        float previousNormalizedSpeed = 0.0f; // Initialize to avoid potential errors
        float smoothingFactor = 10f;
        public string lieAnimationInIdle = "";
        public void MoveToAngle(float angle, float distance, float stopDistance)
        {
            bool canFollow = setup.attribute.CancelFollowToEnemyTime < Time.time;
            if (canFollow)
            {
                if (distance <= stopDistance)
                {
                    //Cancel Follow
                    setup.attribute.ChangeFollowToEnemyTime();
                }
                else
                {
                    SetFloat("Angle", angle);
                    float normalizedSpeed = Formula.NormalizedByRange(distance, stopDistance, 60);
                    float smoothedSpeed = Mathf.Lerp(previousNormalizedSpeed, normalizedSpeed, smoothingFactor * Time.deltaTime);
                    previousNormalizedSpeed = smoothedSpeed;
                    SetFloat("Speed", smoothedSpeed);
                    CrossFade("Move", false);
                }
            }
            else
            {
                lieAnimationInIdle = GetLieAnimation();
                Debug.Log(lieAnimationInIdle);
                CrossFade(lieAnimationInIdle, false);
            }

            string GetLieAnimation()
            {
                Debug.DrawRay(objects.obstacleDetectorPoint.position, -objects.obstacleDetectorPoint.right * 4);
                Debug.DrawRay(objects.obstacleDetectorPoint.position, objects.obstacleDetectorPoint.right * 4);
                Debug.DrawRay(objects.obstacleDetectorPoint.position, -objects.obstacleDetectorPoint.forward * 3);
                if (lieAnimationInIdle == "")
                {
                    bool rayLeft = Physics.Raycast(objects.obstacleDetectorPoint.position, -objects.obstacleDetectorPoint.right, 4, obstacleLayer);
                    bool rayRight = Physics.Raycast(objects.obstacleDetectorPoint.position, objects.obstacleDetectorPoint.right, 4, obstacleLayer);
                    bool rayBack = Physics.Raycast(objects.obstacleDetectorPoint.position, -objects.obstacleDetectorPoint.forward, 3, obstacleLayer);
                    if (rayLeft && rayRight && rayBack) return "Firing Step Back";

                    if (distance < stopDistance * 0.7f)
                    {
                        if (rayBack)
                        {
                            return GetLieLeftOrRight(rayLeft, rayRight);
                        }
                        else
                        {
                            return "Firing Walk Back";
                        }
                    }
                    else
                    {
                        return GetLieLeftOrRight(rayLeft, rayRight);
                    }
                }
                else return lieAnimationInIdle;
            }

            string GetLieLeftOrRight(bool leftInvalid, bool rightInvalid)
            {
                if (leftInvalid && rightInvalid) return "Firing Step Back";
                string[] anims = { "Idle To Lie Left", "Idle To Lie Right" };
                if (leftInvalid)
                {
                    return anims[1];
                }
                else if (rightInvalid)
                {
                    return anims[0];
                }
                else
                {
                    int randomIndex = new System.Random().Next(0, anims.Length);
                    return anims[randomIndex];
                }
            }
        }
        public void LookAndWalk(Transform moveIndicator, bool surprised)
        {
            perspectionListener.localEulerAngles = new Vector3(0, utility.Angle(objects.root, objects.movePoint), 0);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                utility.RootLookAt(moveIndicator, 5);
            }

            if (walking)
            {
                //utility.RootLookAt(moveIndicator, 2);
                if (setup.memory.currentSuspectionID == setup.memory.suspectionID)
                {
                    return;
                }
                else
                {
                    setup.memory.currentSuspectionID = setup.memory.suspectionID;
                }
            }

            float angle;
            if (surprised)
            {
                angle = utility.Angle(objects.root, objects.suspicionPoint);
            }
            else
            {
                angle = utility.Angle(objects.root, moveIndicator);
            }

            float smoothTransform = 0.1f;
            if (angle <= -158)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 180 Degree Left", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("180 Degree Left", smoothTransform, false);
                }
            }
            else if (angle <= -113)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 135 Degree Left", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("135 Degree Left", smoothTransform, false);
                }
            }
            else if (angle <= -68)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 90 Degree Left", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("90 Degree Left", smoothTransform, false);
                }
            }
            else if (angle <= -23)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 45 Degree Left", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("45 Degree Left", smoothTransform, false);
                }
            }
            else if (angle <= 22)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised Walk", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("Walk", smoothTransform, false);
                }
            }
            else if (angle <= 67)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 45 Degree Right", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("45 Degree Right", smoothTransform, false);
                }
            }
            else if (angle <= 113)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 90 Degree Right", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("90 Degree Right", smoothTransform, false);
                }
            }
            else if (angle <= 158)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 135 Degree Right", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("135 Degree Right", smoothTransform, false);
                }
            }
            else if (angle <= 180)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 180 Degree Right", smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("180 Degree Right", smoothTransform, false);
                }
            }
            else
            {
                //CrossFadeTime("Walk");
            }
            walking = true;
        }

        public void LookAndDo(Transform moveIndicator, bool surprised, string animationEndName)
        {
            perspectionListener.localEulerAngles = new Vector3(0, utility.Angle(objects.root, objects.movePoint), 0);
            if (walking)
            {
                //utility.RootLookAt(moveIndicator, 2);
                if (setup.memory.currentSuspectionID == setup.memory.suspectionID)
                {
                    return;
                }
                else
                {
                    setup.memory.currentSuspectionID = setup.memory.suspectionID;
                }
            }
            float angle;
            if (surprised)
            {
                angle = utility.Angle(objects.root, objects.suspicionPoint);
            }
            else
            {
                angle = utility.Angle(objects.root, moveIndicator);
            }

            float smoothTransform = 0.1f;
            if (angle <= -158)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 180 Degree Left" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("180 Degree Left" + animationEndName, smoothTransform, false);
                }
            }
            else if (angle <= -113)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 135 Degree Left" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("135 Degree Left" + animationEndName, smoothTransform, false);
                }
            }
            else if (angle <= -68)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 90 Degree Left" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("90 Degree Left" + animationEndName, smoothTransform, false);
                }
            }
            else if (angle <= -23)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 45 Degree Left" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("45 Degree Left" + animationEndName, smoothTransform, false);
                }
            }
            else if (angle <= 22)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised Walk" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("Walk", smoothTransform, false);
                }
            }
            else if (angle <= 67)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 45 Degree Right" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("45 Degree Right" + animationEndName, smoothTransform, false);
                }
            }
            else if (angle <= 113)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 90 Degree Right" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("90 Degree Right" + animationEndName, smoothTransform, false);
                }
            }
            else if (angle <= 158)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 135 Degree Right" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("135 Degree Right" + animationEndName, smoothTransform, false);
                }
            }
            else if (angle <= 180)
            {
                if (surprised)
                {
                    CrossFadeTime("Surprised 180 Degree Right" + animationEndName, smoothTransform, false);
                }
                else
                {
                    CrossFadeTime("180 Degree Right" + animationEndName, smoothTransform, false);
                }
            }
            else
            {
                //CrossFadeTime("Walk");
            }
            walking = true;
        }

        Vector3 previousPosition;
        public float Angle(Transform from, Transform target, float smooth)
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x,
                    from.transform.position.y,
                    target.transform.position.z);

            Vector3 targetSmoothPosition =
                Vector3.MoveTowards(previousPosition, targetPosition, smooth);
            previousPosition = targetSmoothPosition;
            float angle = Vector3.Angle((targetSmoothPosition - from.position), -from.forward);
            float angle2 =
                Vector3.Angle((targetSmoothPosition - from.position), -from.right);

            if (angle2 > 90)
            {
                angle = (360 - angle) - 180;
            }
            else
            {
                angle = angle - 180;
            }
            return angle;
        }

        public float Angle(Transform from, Transform target)
        {
            Vector3 targetPosition = new Vector3(target.position.x, from.position.y, target.position.z);
            float angle = Vector3.Angle(targetPosition - from.position, -from.forward);
            float angle2 = Vector3.Angle(targetPosition - from.position, -from.right);

            if (angle2 > 90)
            {
                angle = 360 - angle - 180;
            }
            else
            {
                angle -= 180;
            }
            return angle;
        }

        string currentState;
        public void CrossFade(string newState, bool fixedTime)
        {
            if (currentState == newState) return;
            if (fixedTime)
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                double normalizedTime = state.normalizedTime - System.Math.Truncate(state.normalizedTime);
                animator.CrossFade(newState, 0.2f, 0, (float)normalizedTime);
                currentState = newState;
            }
            else
            {
                animator.CrossFade(newState, 0.2f, 0, 0);
                currentState = newState;
            }
        }
        public void CrossFadeTime(string newState, float smooth, bool fixedTime)
        {
            var nextState = animator.GetNextAnimatorStateInfo(0);
            //if(nextState.IsName("Walk")) return;
            if (currentState == newState) return;
            //if(!animator.GetCurrentAnimatorStateInfo(0).IsName(newState)) return;

            if (fixedTime)
            {
                float newNormalizedTime = GetCurrentAnimatorTime(animator, 0);
                animator.CrossFade(newState, smooth, 0, newNormalizedTime);
            }
            else
            {
                animator.CrossFade(newState, smooth, 0, 0.0f);
            }
            currentState = newState;
        }
        public float GetCurrentAnimatorTime(Animator targetAnim, int layer = 0)
        {
            AnimatorStateInfo animState = targetAnim.GetCurrentAnimatorStateInfo(layer);
            float currentTime = animState.normalizedTime % 1;
            return currentTime;
        }
        public void SetFloat(string name, float value)
        {
            animator.SetFloat(name, value);
        }
        void IndicatorController(Vector3 movePosition, bool LookTarget)
        {
            if (LookTarget)
            {
                if (objects.enemy)
                {
                    objects.lookPoint.position = objects.enemy.position;
                }
                else
                {
                    objects.lookPoint.position = objects.moveIndicator.position;
                }
                utility.RootLookAt(objects.lookPoint, 2);
                setup.movePosition.SetPosition(movePosition);
            }
            else
            {
                setup.movePosition.SetPosition(movePosition);
                utility.RootLookAt(objects.moveIndicator, 2);
            }
        }
    }

}

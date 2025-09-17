using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerRoot;
namespace BotRoot
{
    public class BotUtility : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        BotObjects objects;
        BotSourceOfAction sourceOfAction;
        bool initialized = false;

        private void Awake()
        {
            objects = setup.objects;
            sourceOfAction = setup.sourceOfAction;
            initialized = true;
        }
        Vector3 previousPosition;

        public float Angle(Transform from, Transform target)
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x, from.transform.position.y, target.transform.position.z);

            previousPosition = targetPosition;
            float angle = Vector3.Angle((targetPosition - from.position), -from.forward);
            float angle2 = Vector3.Angle((targetPosition - from.position), -from.right);

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
        public void ChangeGuardingPoint()
        {
            if (objects.guardingPoints.childCount <= 1)
            {
                objects.currentGuardingPoint = objects.guardingPoints.GetChild(0);
                setup.movePosition.SetPosition(objects.currentGuardingPoint.position);
            }
            else
            {
                Transform guardingPoint = objects.currentGuardingPoint;
                while (objects.currentGuardingPoint == guardingPoint)
                {
                    objects.currentGuardingPoint = objects.guardingPoints.GetChild(Random.Range(0, objects.guardingPoints.childCount));
                    setup.movePosition.SetPosition(objects.currentGuardingPoint.position);
                }
            }
        }
        public void RootLookAt(Transform target, float smooth)
        {
            Vector3 _target = new(target.position.x, objects.root.position.y, target.position.z);
            Vector3 direction = (_target - transform.position).normalized; ;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smooth * Time.deltaTime);
        }
        public void RootLookAt(Vector3 target, float smooth)
        {
            Vector3 _target = new(target.x, objects.root.position.y, target.z);
            Vector3 direction = (_target - transform.position).normalized; ;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smooth * Time.deltaTime);
        }
        public void RootLookAt(Transform target)
        {
            Vector3 _target = new Vector3(target.position.x, objects.root.position.y, target.position.z);
            Quaternion targetRotation = Quaternion.LookRotation(_target - transform.position, Vector3.up);
            transform.rotation = targetRotation;
        }
        public float GuardingPointDistance(Transform point)
        {
            return Vector3.Distance(objects.root.position, point.position);
        }
        public void CallingToDiedEnemy(Transform whoKilled)
        {
            if (setup.health.currentHealth <= 0) { print("Cancelled"); return; }
            
            setup.memory.allyIsDied = true;
            Collider[] friends = Physics.OverlapSphere(transform.position, 200, setup.memory.friendsLayer);
            int calledFriendCount = 0;
            foreach (Collider _friend in friends)
            {
                if (_friend.tag != "Live/Evil" && _friend.name != "character") continue;
                Transform friend = _friend.transform;
                BotObjects friendObjects = friend.GetComponent<BotObjects>();
                BotBase friendMemory = friend.GetComponent<BotBase>();
                BotStatus friendStatus = friend.GetComponent<BotStatus>();
                if (friendObjects && friendMemory)
                {
                    if (friendObjects.enemy) continue;
                    if(!friendMemory.frightened) friendObjects.setup.author.SetAlarmSignal(false);
                    if (whoKilled)
                    {
                        setup.memory.suspectionID = Random.Range(1000000, 9999999);
                        friendMemory.suspectionID = Random.Range(1000000, 9999999);
                        setup.objects.ChangeSuspectionPoint(whoKilled.position);
                        friendObjects.patrollingPoint.position = whoKilled.position;
                    }
                    friendMemory.EnemyIsAround = true;
                    friendMemory.allyIsDied = true;
                    if (friendStatus.Purpose != BotEnum.Purpose.Attacking)
                    {
                        friendStatus.Purpose = BotEnum.Purpose.Patrolling;
                        friendMemory.frightened = true;
                    }
                    friendStatus.MentalState = BotEnum.MentalState.Panic;
                    calledFriendCount++;
                    setup.overall.PatrollingStrategyBuilder(setup.objects.suspicionPoint.position);
                    StartCoroutine(friendMemory.allyIsDiedForget());
                }
            }
            print("Called Friends Count = " + calledFriendCount + " | Caller: " + transform.parent.parent.name);
        }
        public float TargetAngle(Transform target)
        {
            Vector3 targetDir = target.position - transform.position;
            return Vector3.Angle(targetDir, transform.forward);
        }
        public void IdentifyEnemyTime()
        {
            if(!initialized) return;
            if (!setup.objects.futureEnemy)
            {
                setup.attribute._identifyBackTime -= Time.deltaTime;
                if (setup.attribute._identifyBackTime <= 0)
                {
                    if (setup.attribute._identifyEnemyTime <= 0)
                    {
                        setup.attribute._identifyEnemyTime = 0;
                    }
                    else
                    {
                        setup.attribute._identifyEnemyTime -= Time.deltaTime / 3;
                    }
                }
            }
            else
            {

                float enemyDistance = Vector3.Distance(transform.position, objects.futureEnemy.position);
                float detectionQuality = setup.indicatorUtility.detection();

                if (!setup.enemyIndicator && detectionQuality > 0)
                {
                    setup.attribute._identifyEnemyTime = 0;
                    if (objects.futureEnemy.TryGetComponent(out PlayerBody playerBody))
                    {
                        playerBody.indicatorRegister.CreateIndicator(transform, objects.futureEnemy, setup);
                    }
                }

                if (enemyDistance > setup.attribute.sensationDistance)
                {
                    //Reduce
                    setup.attribute._identifyEnemyTime -= setup.attribute._identifyEnemyTime <= 0 ? 0 : Time.deltaTime / (1 * TargetAngle(objects.futureEnemy));
                }
                else
                {
                    var suspection = setup.status.MentalState == BotEnum.MentalState.Suspicion;
                    var panic = setup.status.MentalState == BotEnum.MentalState.Panic;
                    
                    if (!objects.futureEnemy.TryGetComponent(out PlayerBody playerBody)) return;
                    var targetStatus = playerBody.status;
                    var targetSpeed = (float)
                        (targetStatus.movement switch
                        {
                            Movement.CrouchIdle => 5,
                            Movement.CrouchWalking => 10,
                            Movement.CrouchRunning => 40,
                            Movement.Idle => 15,
                            Movement.Walking => 60,
                            Movement.Running => 70,
                            _ => 0
                        });

                    setup.attribute._identifyBackTime = setup.attribute.identifyBackTime;
                    setup.attribute._identifyEnemyTime += ((Time.deltaTime / enemyDistance / TargetAngle(objects.futureEnemy)) * (setup.memory.EnemyIsAround || panic ? targetSpeed * 3 : suspection ? targetSpeed * 2 : targetSpeed)) * detectionQuality;
                }
            }
        }

        public void CallStaticEnemy(Transform staticEnemy)
        {
            if (setup.health.currentHealth <= 0) return;
            var friends = new Collider[50];
            var size = Physics.OverlapSphereNonAlloc(transform.position, 200, friends, setup.memory.friendsLayer);
            var calledFriendCount = 0;
            foreach (var _friend in friends)
            {
                if (_friend.GetInstanceID() == GetInstanceID()) continue;
                if (!_friend.CompareTag("Live/Evil") && _friend.name != "character") continue;
                var friend = _friend.transform;

                if (friend.TryGetComponent(out BotSetup friendSetup))
                {
                    var friendObjects = friendSetup.objects;
                    if (friendObjects) if (friendObjects.enemy) continue;

                    var friendMemory = friendSetup.memory;
                    var friendStatus = friendSetup.status;
                    
                    if(!friendMemory.frightened) friendObjects.setup.author.SetAlarmSignal(false);
                    setup.memory.suspectionID = Random.Range(1000000, 9999999);
                    setup.objects.ChangeSuspectionPoint(staticEnemy.position);
                    friendObjects.patrollingPoint.position = staticEnemy.position;
                    friendObjects.staticEnemy = objects.lastEnemy;
                    friendObjects.lastEnemy = objects.lastEnemy;
                    friendMemory.EnemyIsAround = true;
                    friendMemory.frightened = true;
                    friendStatus.Purpose = BotEnum.Purpose.Attacking;
                    friendStatus.MentalState = BotEnum.MentalState.Panic;
                    calledFriendCount++;  
                }
            }
            print("Called Friends Cound = " + calledFriendCount);
        }

        public void CallToShotPosition(Vector3 point)
        {
            if (setup.health.currentHealth <= 0) return;
            if (!setup.objects.enemy)
            {
                setup.memory.suspectionID = Random.Range(1000000, 9999999);
                setup.objects.ChangeSuspectionPoint(point);
                objects.patrollingPoint.position = point;
                setup.memory.EnemyIsAround = true;
                setup.memory.frightened = true;
                objects.staticEnemy = setup.overall.player.transform;
                objects.lastEnemy = setup.overall.player.transform;
                setup.status.Purpose = BotEnum.Purpose.Attacking;
                setup.status.MentalState = BotEnum.MentalState.Panic;
                setup.overall.SetLastEnemyVisiblePoint(setup.overall.player.transform.position);
            }
            var friends = Physics.OverlapSphere(transform.position, 200, setup.memory.friendsLayer);
            var calledFriendCount = 0;
            foreach (Collider _friend in friends)
            {
                if (_friend.GetInstanceID() == GetInstanceID()) continue;
                if (!_friend.CompareTag("Live/Evil") && _friend.name != "character") continue;
                Transform friend = _friend.transform;
                
                if(friend.TryGetComponent(out BotObjects friendObjects) && friend.TryGetComponent(out BotBase friendMemory) && friend.TryGetComponent(out BotStatus friendStatus))
                {
                    if (friendObjects) if (friendObjects.enemy) continue;
                    if (friendObjects && friendMemory)
                    {
                        if(!friendMemory.frightened) friendObjects.setup.author.SetAlarmSignal(false);
                        setup.memory.suspectionID = Random.Range(1000000, 9999999);
                        setup.objects.ChangeSuspectionPoint(point);
                        friendObjects.patrollingPoint.position = point;
                        friendObjects.staticEnemy = setup.overall.player.transform;
                        friendObjects.lastEnemy = setup.overall.player.transform;
                        friendMemory.EnemyIsAround = true;
                        friendMemory.frightened = true;
                        friendStatus.Purpose = BotEnum.Purpose.Attacking;
                        friendStatus.MentalState = BotEnum.MentalState.Panic;
                        calledFriendCount++;
                    }
                }
                
            }
            print("Called Friends Cound = " + calledFriendCount);
        }

        public void GetPanic()
        {
            if(!initialized) return;
            if (setup.objects.enemy)
            {
                setup.status.MentalState = BotEnum.MentalState.Panic;
            }
        }

        public Transform[] GetFirends()
        {
            var lives = Physics.OverlapSphere(transform.position, 200, setup.memory.friendsLayer);
            var friends = (from friend in lives where friend.GetInstanceID() != GetInstanceID() where friend.CompareTag(transform.tag) select friend.transform).ToList();

            var result = new Transform[friends.Count];
            var index = 0;
            foreach (var friend in friends)
            {
                result[index] = friend;
                index++;
            }
            return result;
        }

        public Transform GetHidingPoint()
        {
            Collider[] points = Physics.OverlapSphere(transform.position, 100, setup.memory.hidingPointsLayer);
            foreach (Collider point in points)
            {
                if (!point.TryGetComponent(out HidingPoints hidingPoint)) continue;
                if (hidingPoint.isBusy) continue;
                hidingPoint.isBusy = true;
                return hidingPoint.GetHidingPoints(objects.lastEnemy ? objects.lastEnemy.position : objects.suspicionPoint.position);
            }
            setup.status.assaultCommand = BotEnum.AssaultCommand.Point;
            return null;
        }
        public void AccessToHiding(bool access, float time)
        {
            if (access) Invoke(nameof(HidingTrue), time);
            else HidingFalse();
        }

        private void HidingTrue()
        {
            setup.sourceOfAction.accesToHiding = true;
        }

        private void HidingFalse()
        {
            setup.sourceOfAction.accesToHiding = false;
        }
        public void CommandFront(float time)
        {
            StartCoroutine(CommandFrontInvoke(time));
        }

        private IEnumerator CommandFrontInvoke(float time)
        {
            yield return new WaitForSeconds(time);
            if (objects.enemy) setup.status.assaultCommand = BotEnum.AssaultCommand.Front;
        }
        public void PlayRandomSound(AudioClip[] clips, AudioSource source)
        {
            int random = Random.Range(0, clips.Length - 1);
            source.PlayOneShot(clips[random], 1);
        }
    }

}

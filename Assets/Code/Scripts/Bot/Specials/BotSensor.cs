using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class BotSensor : MonoBehaviour
    {
        [SerializeField] public BotSetup setup;
        [SerializeField] LayerMask liveMask;
        public LayerMask obstacleMask;

        public float sensitivityDistance;

        //Private//
        [SerializeField] private float areaListenerRate;
        private float nextTimeToListen;
        private Vector3 lastSuspectedPoint;

        private void Start()
        {
            StartCoroutine("AreaListenerWithDelay", 0.4f);
        }
        IEnumerator AreaListenerWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                AreaListener();
            }
        }
        void AreaListener()
        {
            if (Time.time >= nextTimeToListen)
            {
                Collider[] lives = Physics.OverlapSphere(transform.position, 20, liveMask);
                foreach (Collider live in lives)
                {
                    Transform creature = live.transform;
                    float distance = Vector3.Distance(transform.position, creature.position);
                    if (distance < sensitivityDistance && live.tag != "Live/Evil")
                    {
                        LiveAction liveAction = creature.GetComponent<LiveAction>();
                        if (liveAction)
                        {
                            if (liveAction.normalSpeed > 0.4f)
                            {
                                setup.botAudio.Play(PanicTalking.SUSPECTED,setup);
                                if(setup.status.MentalState != BotEnum.MentalState.Panic) setup.author.SetWonderSignal(true);
                                if(setup.status.MentalState != BotEnum.MentalState.Panic) setup.status.MentalState = BotEnum.MentalState.Suspicion;
                                setup.memory.suspectionID = Random.Range(1000000, 9999999);
                                setup.objects.ChangeSuspectionPoint(creature.position);
                                nextTimeToListen = Time.time + 1f / areaListenerRate;
                            }
                        }
                    }
                }
            }
        }

        public void SetSuspectPoint(Transform point,float nextSuspectionDistance, bool panicTalking)
        {
            float lastSuspectedDistance = Vector3.Distance(lastSuspectedPoint, point.position);
            if(lastSuspectedDistance >= nextSuspectionDistance)
            {
                if(panicTalking) setup.botAudio.Play(PanicTalking.SUSPECTED,setup);
                if(setup.status.MentalState != BotEnum.MentalState.Panic) setup.author.SetWonderSignal(true);
                if(setup.status.MentalState != BotEnum.MentalState.Panic) setup.status.MentalState = BotEnum.MentalState.Suspicion;
                setup.memory.suspectionID = Random.Range(1000000, 9999999);
                setup.objects.ChangeSuspectionPoint(point.position);
                lastSuspectedPoint = point.position;
            }
        }

        public void SetEnemyPosition(Transform player, float nextSuspectionDistance, bool panicTalking)
        {
            float lastSuspectedDistance = Vector3.Distance(lastSuspectedPoint, player.position);

            if(lastSuspectedDistance >= nextSuspectionDistance && setup.objects.enemy == null)
            {
                if(setup.status.MentalState != BotEnum.MentalState.Panic) setup.author.SetAlarmSignal(true);
                if(panicTalking) setup.botAudio.Play(PanicTalking.SUSPECTED,setup);
                setup.memory.suspectionID = Random.Range(1000000, 9999999);
                setup.objects.ChangeSuspectionPoint(player.position);
                setup.overall.SetLastEnemyVisiblePoint(player.position);

                setup.objects.patrollingPoint.position = player.position;
                setup.objects.staticEnemy = player;
                setup.objects.lastEnemy = player;
                setup.memory.EnemyIsAround = true;
                //setup.memory.frightened = true;
                setup.status.Purpose = BotEnum.Purpose.Attacking;
                setup.status.MentalState = BotEnum.MentalState.Panic;
            }
        }

        public void InvokeEnableListener(float time)
        {
            Invoke("EnableListener", time);
        }

        void EnableListener()
        {
            GetComponent<Collider>().enabled = true;
        }
    }

}

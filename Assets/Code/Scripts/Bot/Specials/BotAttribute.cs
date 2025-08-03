using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class BotAttribute : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        public float sensationDistance;
        public float identifyEnemyTime;
        public float identifySpeed;
        public float enemyPassTime;
        public float nextSuspectionTime;

        //AttackingAttributes
        public float StopDistanceInAttack;
        public float CancelFollowToEnemyTime;

        [Range(0,100)]
        public int MoveToWonderingRate = 50;

        [HideInInspector] public float _identifyEnemyTime;
        [HideInInspector] public float _identifyBackTime;
        [HideInInspector] public float _nextSuspectionTime;
        public float identifyBackTime;

        public void Awake()
        {
            InvokeRepeating(nameof(ChangeStopDistanceInAttack), 0, 20);
        }

        void ChangeStopDistanceInAttack()
        {
            StopDistanceInAttack = Random.Range(10, 40);
        }

        public void ChangeFollowToEnemyTime()
        {
            CancelFollowToEnemyTime = Time.time + Random.Range(4, 15);
        }
    }
}


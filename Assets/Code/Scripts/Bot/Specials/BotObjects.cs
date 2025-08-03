using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class BotObjects : MonoBehaviour
    {
        [Space][Header("Root Object")]
        public Transform root;
        [HideInInspector] public BotSetup setup;

        [Space][Header("Equipments")]
        public Transform Weapon;

        [Space][Header("Guarding Point Objects")]
        public Transform guardingPoints;
        [HideInInspector] public Transform currentGuardingPoint;
        [HideInInspector] public Transform nextGuardingPoint;
        [Space][Header("Obstacle Detector Position")]
        public Transform obstacleDetectorPoint;

        [Space][Header("Indicators")]
        public Transform moveIndicator;
        public Transform movePoint;
        public Transform lookPoint;
        public Transform enemy;
        public Transform futureEnemy;
        public Transform staticEnemy;
        public Transform lastEnemy;
        public bool getOneTimeHidingPosition;

        public Transform suspicionPoint;
        public Transform patrollingPoint;
        public PatrollingPoint patrolling;

        public Transform hidingPoint;

        [Space][Header("Sounds")]
        public AudioClip[] dyingSound;

        public void ChangeSuspectionPoint(Vector3 point)
        {
            suspicionPoint.position = point;
            setup.indicatorUtility.ChangeDistance(point);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerRoot;
namespace BotRoot
{
    public class IndicatorUtility : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        public AnimationCurve distanceCurve;
        public AnimationCurve angleCurve;
        [Range(0, 200)]
        public float maxDistance;
        [Range(30, 90)]
        public float maxAngle;
        [Range(0, 50)]
        public float strenth;

        public float DistanceBackToDefaultTime = 10;
        public float _distanceBackToDefaultTime;
        public float DefaultDistance;
        private bool ChangedDistance = false;

        float progressDistance;
        float progressAngle;
        float result;

        private void Start()
        {
            DefaultDistance = maxDistance;
        }

        public void Update()
        {
            if(!ChangedDistance) return;
            _distanceBackToDefaultTime -= Time.deltaTime;
            if(_distanceBackToDefaultTime <= 0)
            {
                maxDistance = DefaultDistance;
                ChangedDistance = false;
            }
        }

        public void ChangeDistance(Vector3 point)
        {
            Debug.Log("Sus Point Changed");
            if(!ChangedDistance)
            {
                float distance = Vector3.Distance(point, setup.transform.position);
                if(distance < DefaultDistance) maxDistance = DefaultDistance + 100;
                else maxDistance = distance + 100;
                ChangedDistance = true;
                _distanceBackToDefaultTime = DistanceBackToDefaultTime;
            }
        }
        public float detection()
        {
            if (!setup.objects.futureEnemy) return -3;

            if (setup.objects.futureEnemy.TryGetComponent(out PlayerBody playerBody))
            {
                TargetStatus status = playerBody.status;
                bool crouch = status.crouch;
                bool enemyIsAround = setup.memory.EnemyIsAround;
                bool onlyHead = setup.memory.OnlyEnemyHeadVisible;

                float distanceQuality = enemyIsAround ? maxDistance * 2f : onlyHead ? maxDistance / 2 : maxDistance;
                distanceQuality = crouch ? distanceQuality / 2 : distanceQuality;

                float strenthQuality = enemyIsAround ? strenth * 2f :onlyHead ? strenth / 2: strenth;
                strenthQuality = crouch ? strenthQuality / 2 : strenthQuality;

                float enemyDistance = Vector3.Distance(setup.transform.position, setup.objects.futureEnemy.position);
                float enemyAngle = setup.utility.TargetAngle(setup.objects.futureEnemy);

                progressDistance = Mathf.Clamp(enemyDistance / distanceQuality * 1, 0, 1);
                progressAngle = Mathf.Clamp(enemyAngle / maxAngle * 1, 0, 1);

                float distanceValue = distanceCurve.Evaluate(progressDistance);
                float angleValue = angleCurve.Evaluate(progressAngle);

                result = angleValue + distanceValue;
                return result * strenthQuality;
            }
            else
            {
                return 0;
            }
        }
    }

}

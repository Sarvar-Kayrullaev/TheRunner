using UnityEngine;

namespace BotRoot
{
    public class BotSpawner : MonoBehaviour
    {
        public GameObject BotPrefab;
        public BotEnum.BotType Type;

        [Space]
        public Transform GuardingPoints;
        public Transform PatrollingPoints;

        [HideInInspector] public GameObject bot;
        [HideInInspector] public GameObject indicatorParent;
        private BotSetup setup;

        public void Spawn(OverallController overall)
        {
            bot = Instantiate(BotPrefab, transform);
            setup = bot.GetComponent<BotSetup>();
            InstantiateIndicators();

            setup.global = FindFirstObjectByType<BotGlobal>();
            setup.botAudio = setup.global.Audio;
            setup.objects.guardingPoints = GuardingPoints;
            setup.overall = overall;
        }

        public void Unspawn()
        {
            if (bot)
            {
                Destroy(bot);
                Destroy(indicatorParent);
                setup = null;
            }
        }

        public void InstantiateIndicators()
        {
            indicatorParent = new("Indicators");

            GameObject moveIndicator = new("moveIndicator");
            GameObject movePoint = new("movePoint");
            GameObject lookPoint = new("lookPoint");
            GameObject suspectionPoint = new("suspectionPoint");
            GameObject patrollingPoint = new("patrollingPoint");

            indicatorParent.transform.parent = transform;

            moveIndicator.transform.parent = indicatorParent.transform;
            movePoint.transform.parent = indicatorParent.transform;
            lookPoint.transform.parent = indicatorParent.transform;
            suspectionPoint.transform.parent = indicatorParent.transform;
            patrollingPoint.transform.parent = indicatorParent.transform;

            setup.objects.moveIndicator = moveIndicator.transform;
            setup.objects.movePoint = movePoint.transform;
            setup.objects.lookPoint = lookPoint.transform;
            setup.objects.suspicionPoint = suspectionPoint.transform;
            setup.objects.patrollingPoint = patrollingPoint.transform;

            MovePosition movePosition = movePoint.AddComponent<MovePosition>();
            movePosition.Owner = bot.transform;
            setup.movePosition = movePosition;
        }
    }
}

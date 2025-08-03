using UnityEngine;
using System.Collections.Generic;
using System;
using PlayerRoot;
using TMPro;
using System.Threading.Tasks;
using BotRoot;

namespace MissionManager
{
    [Serializable]
    public class MissionSequence : MonoBehaviour
    {
        ///Tab 1 

        public List<MissionModel> missions;

        ///Tab 2
        [SerializeField] private int FailedMissionDelay = 4000;
        [SerializeField] private Transform Player;
        [SerializeField] private GameObject IndicatorParent;
        [SerializeField] private GameObject IndicatorPrefab;
        [SerializeField] private OverallController overallController;

        ///Tab 3
        [SerializeField] private GameObject CompleteParent;
        [SerializeField] private GameObject FailedParent;
        [SerializeField] private GameObject TitleParent;
        [SerializeField] private GameObject ContextParent;

        ///Private
        MissionModel currentMission;
        int currentMissionIndex;
        GameObject indicator;
        bool isFailed = false;
        Player playerController;
        IndicatorManager indicatorManager;

        private TMP_Text title;
        private TMP_Text context;

        [Obsolete]
        public void Initialize()
        {
            if (TitleParent.transform.GetChild(0).TryGetComponent(out TMP_Text title)) this.title = title;
            if (ContextParent.transform.GetChild(0).TryGetComponent(out TMP_Text context)) this.context = context;

            playerController = Player.GetComponent<Player>();
            indicatorManager = FindFirstObjectByType<IndicatorManager>();
            if (overallController == null) overallController = FindObjectOfType<OverallController>();
            StartMission(0);
            InvokeRepeating(nameof(UpdateMission), 0, 0.5f);
            SetNewTitle();
        }

        private void StartMission(int missionIndex)
        {
            if (missions.Count > missionIndex)
            {
                overallController.EnemyVisibled = false;
                currentMission = missions[missionIndex];
                currentMissionIndex = missionIndex;
                Destroy(indicator);
            }
            else return;

            if (currentMission.missionType == MissionType.MovePoint)
            {
                indicator = Instantiate(IndicatorPrefab, IndicatorParent.transform);
                if (indicator.TryGetComponent(out ObjectiveIndicator objectiveIndicator)) objectiveIndicator.Initialize(currentMission.point, indicatorManager);
            }
            else if (currentMission.missionType == MissionType.MovePointStealth)
            {
                indicator = Instantiate(IndicatorPrefab, IndicatorParent.transform);
                if (indicator.TryGetComponent(out ObjectiveIndicator objectiveIndicator)) objectiveIndicator.Initialize(currentMission.point, indicatorManager);
            }
            else if (currentMission.missionType == MissionType.DestroyPoint)
            {
                Destroy(indicator);
            }
        }

        private void UpdateMission()
        {
            if (isFailed) return;
            if (!currentMission.isCompleted)
            {
                if (currentMission.missionType == MissionType.MovePoint)
                {
                    float distance = Vector3.Distance(Player.position, currentMission.point.position);
                    if (currentMission.triggerDistance >= distance)
                    {
                        ObjectiveCompleted();
                    }
                }
                else if (currentMission.missionType == MissionType.MovePointStealth)
                {
                    if (overallController.EnemyVisibled) MissionFailed();
                    float distance = Vector3.Distance(Player.position, currentMission.point.position);
                    if (currentMission.triggerDistance >= distance)
                    {
                        ObjectiveCompleted();
                    }
                }
                else if (currentMission.missionType == MissionType.DestroyPoint)
                {
                    bool isTargetAlive = false;
                    foreach (BotSpawner spawner in currentMission.targets)
                    {
                        Bot bot = spawner.bot.GetComponent<Bot>();
                        if (!bot.setup.health.died) isTargetAlive = true;
                    }
                    if (!isTargetAlive) ObjectiveCompleted();
                }
                else if (currentMission.missionType == MissionType.DestroyPointStealth)
                {
                    bool isTargetAlive = false;
                    foreach (BotSpawner spawner in currentMission.targets)
                    {
                        Bot bot = spawner.bot.GetComponent<Bot>();
                        if (!bot.setup.health.died) isTargetAlive = true;
                        if (overallController.EnemyVisibled) MissionFailed();
                    }
                    if (!isTargetAlive) ObjectiveCompleted();
                }
                else if (currentMission.missionType == MissionType.DestroyCommander)
                {

                }
                else
                {

                }
            }
        }

        private void ObjectiveCompleted()
        {
            currentMission.isCompleted = true;
            CompletedAnimation();
            if (missions.Count > currentMissionIndex + 1)
            {
                currentMissionIndex++;
                StartMission(currentMissionIndex);
            }
            else
            {
                Destroy(indicator);
            }
        }

        private void MissionFailed()
        {
            isFailed = true;
            FailedAnimation();

            Wait();
            async Task Wait()
            {
                await Task.Delay(FailedMissionDelay);
                if (!playerController.Died) playerController.lostManager.FailedPlayer(playerController, currentMission.title);
            }
        }

        private void CompletedAnimation()
        {
            TitleParent.SetActive(false);
            FailedParent.SetActive(false);
            CompleteParent.SetActive(true);
            Invoke(nameof(SetNewTitle), 4);
        }

        private void FailedAnimation()
        {
            TitleParent.SetActive(false);
            FailedParent.SetActive(true);
            CompleteParent.SetActive(false);
        }

        private void SetNewTitle()
        {
            TitleParent.SetActive(true);
            FailedParent.SetActive(false);
            CompleteParent.SetActive(false);
            if (title) title.text = currentMission.title;
            if (context) context.text = currentMission.description;
        }
    }

    [Serializable]
    public class MissionModel
    {
        public int index;
        public bool identified;
        //public Transform transform;

        public MissionType missionType;
        public Transform point;
        public string title;
        public string description;
        public bool isCompleted;

        public float triggerDistance = 3;
        public List<BotSpawner> targets;
    }

    public enum MissionType
    {
        MovePoint,
        MovePointStealth,
        DestroyPoint,
        DestroyPointStealth,
        DestroyCommander,
    }
}

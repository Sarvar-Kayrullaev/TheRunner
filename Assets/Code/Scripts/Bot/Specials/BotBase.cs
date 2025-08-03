using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class BotBase : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        public bool EnemyIsAround = false;
        public bool allyIsDied = false;
        public bool EnemyDetected = false;
        public bool AlarmIsOn = false;
        public bool EnemyPassed = false;
        public bool frightened = false;
        public bool OnlyEnemyHeadVisible = false;
        public bool MoveToWonderingPoint = true;
        public bool ForceMoveToWonderingPoint = false;
        public bool NotNecessarilyMoveToWonderingPoint = false;
        public int NumberOfSuspicions = 0;
        public Transform whoKilled = null;
        public float timeToDied = -1;
        public LayerMask friendsLayer;
        public LayerMask hidingPointsLayer;

        //TALKING BOOLS
        public bool allyIsKilled = false;
        public bool frightenedAudio = false;
        public bool hitFirstBullet = false;

        private int _suspectionID;
        public int suspectionID
        {
            get { return _suspectionID; }
            set
            {
                _suspectionID = value;
                int randomValue = Random.Range(0, 100);
                MoveToWonderingPoint = randomValue <= setup.attribute.MoveToWonderingRate;
            }
        }
        public int currentSuspectionID = 0;

        public IEnumerator allyIsDiedForget()
        {
            yield return new WaitForSeconds(90);
            allyIsDied = false;
        }
    }
}


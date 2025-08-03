using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BotRoot
{
    public class BotStatus : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        public bool Captain = false;
        public bool isBuried = false;
        public bool outOfCount = false;
        public BotEnum.MentalState MentalState;
        public BotEnum.Purpose Purpose;
        public BotEnum.Command Command;
        public BotEnum.AssaultCommand assaultCommand;
        public BotEnum.PatrollCommand patrollCommand;

        [Header("Debug")]
        public Text[] messages;
        public IEnumerator SetBurried()
        {
            yield return new WaitForSeconds(2);
            isBuried = true;
        }
    }
}


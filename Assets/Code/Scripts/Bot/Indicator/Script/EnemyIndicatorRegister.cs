using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class EnemyIndicatorRegister : MonoBehaviour
    {
        public GameObject indicatorPrefab;
        public Transform registerParent;
        public AudioSource audioSource;

        public void CreateIndicator(Transform owner, Transform target)
        {
            GameObject indicator = Instantiate(indicatorPrefab, registerParent);
            indicator.GetComponent<EnemyIndicator>().Register(owner, target, audioSource);
            owner.GetComponent<BotSetup>().enemyIndicator = indicator.transform;
        }
    }

}

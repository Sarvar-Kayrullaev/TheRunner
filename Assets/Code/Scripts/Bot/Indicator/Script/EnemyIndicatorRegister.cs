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

        public void CreateIndicator(Transform owner, Transform target, BotSetup setup)
        {
            var indicator = Instantiate(indicatorPrefab, registerParent);
            if (indicator.TryGetComponent(out EnemyIndicator indicatorComponent))
            {
                indicatorComponent.Register(owner, target, audioSource, setup);
                setup.enemyIndicator = indicator.transform;
                
            }
            else
            {
#if UnityEditor
                Debug.LogWarning("No indicator registered for " + target.name);
#endif
            }
        }
    }

}

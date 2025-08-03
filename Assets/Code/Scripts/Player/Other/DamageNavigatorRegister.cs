using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BotRoot;

public class DamageNavigatorRegister : MonoBehaviour
{
    public GameObject navigatorPrefab;
    public Transform registerParent;

    public void CreateIndicator(Transform owner, Transform target)
    {
        GameObject navigator = Instantiate(navigatorPrefab, registerParent);
        
        navigator.GetComponent<DamageNavigator>().Register(owner, target);
        owner.GetComponent<BotSetup>().enemyDamageNavigator = navigator.transform;
    }
}

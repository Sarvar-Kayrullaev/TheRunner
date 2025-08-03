using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetaTestToast : MonoBehaviour
{
    public GameObject BetaTestScreen;
    bool betaScreen = false;
    bool visible = false;
    float animTime = 0.5f;
    float currentAnimTime;

    void Update()
    {
        if(!betaScreen) return;
        currentAnimTime -= Time.deltaTime;
        if(currentAnimTime <= 0)
        {
            visible = !visible;
            BetaTestScreen.SetActive(visible);
            currentAnimTime = animTime;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        betaScreen = true;
    }

    void OnTriggerExit(Collider collider)
    {
        betaScreen = false;
        BetaTestScreen.SetActive(false);
    }
}

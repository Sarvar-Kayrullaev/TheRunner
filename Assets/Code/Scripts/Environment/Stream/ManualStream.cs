using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class ManualStream : MonoBehaviour
    {
        [SerializeField] GameObject streamThing;

        [Space]
        [Header("Options")]
        [SerializeField] float cullingDistance = 120;
        [SerializeField] float updateRate = 1;

        // Private

        private Transform player;

        void Awake()
        {
            player = Camera.main.transform;
            if (streamThing)
            {
                InvokeRepeating(nameof(UpdateThing), 0, updateRate);
            }
            else
            {
                Debug.LogWarning("You have not set an THING to manual streaming");
            }
        }

        void UpdateThing()
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= cullingDistance)
            {
                streamThing.SetActive(true);
            }
            else
            {
                streamThing.SetActive(false);
            }
        }
    }

}

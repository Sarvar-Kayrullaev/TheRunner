using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class Bot : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        BotAction action;

        public int TalkingSoundVariant;

        public bool debug = false;
        void Start()
        {
            action = setup.action;
        }

        void Update()
        {
            if (setup.initialized)
            {
                action.Main();
            }
        }
    }

}

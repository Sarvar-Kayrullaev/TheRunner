using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BotRoot;

namespace PlayerRoot
{
    public class PlayerBody : MonoBehaviour
    {
        public PlayerBodyName body;
        public Player player;
        public TargetStatus status;
        public EnemyIndicatorRegister indicatorRegister;
    }

    public enum PlayerBodyName
    {
        Head,
        Body
    }
}


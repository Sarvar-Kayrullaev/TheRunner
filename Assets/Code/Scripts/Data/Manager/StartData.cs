using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class StartData : MonoBehaviour
    {
        public bool CreatePlayerData = false;
        public Transform OutpostParent;
        public OpenWorldModel OpenWorldData = new();
        public PlayerModel PlayerData = new();
    }
}


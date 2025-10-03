using System.Collections.Generic;
using UnityEngine;

namespace BotRoot
{
    public class BotGlobal : MonoBehaviour
    {
        public RectTransform MarkerParent;
        public BotAudio Audio;
        public HitMarker HitMarker;
        
        public List<Bot> bots = new List<Bot>();
        public List<Bot> markedBots = new List<Bot>();
    }
}

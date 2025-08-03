using System.Collections.Generic;
using UnityEngine;

namespace BotRoot
{
    public class PatrollingPoint : MonoBehaviour
    {
        public bool IsBusy = false;
        public bool SafePoint = true;

        public List<Transform> points = new();
        private Transform pointed;
        void Start()
        {
            Reset();
        }

        public Transform GetRandomPoint()
        {
            //if(pointed) return pointed;
            return points[0];
        }

        public void Reset()
        {
            CancelInvoke(nameof(SetIsNotBusy));
            pointed = null;
            IsBusy = false;
            points = new();
            foreach (Transform point in transform)
            {
                points.Add(point);
            }
        }

        public void SetAreaIsDanger()
        {
            SafePoint = false;
            CancelInvoke(nameof(SetAreaIsSafe));
            Invoke(nameof(SetAreaIsSafe),120);
        }

        public void SetAreaIsSafe()
        {
            SafePoint = true;
        }

        public void SetIsBusy()
        {
            IsBusy = true;
            Invoke(nameof(SetIsNotBusy),120);
        }

        public void SetIsNotBusy()
        {
            IsBusy = false;
        }
    }
}


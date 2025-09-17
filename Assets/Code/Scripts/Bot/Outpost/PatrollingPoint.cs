using System.Collections;
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
            if(_setIsNotBusyCoroutine != null) StopCoroutine(_setIsNotBusyCoroutine);
            _setIsNotBusyCoroutine = null;
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
            if (_coroutine != null) StopCoroutine(_coroutine);

            _coroutine = SetAreaIsSafe(120);
            StartCoroutine(_coroutine);
        }
        
        private IEnumerator _coroutine;
        private IEnumerator SetAreaIsSafe(float time)
        {
            yield return new WaitForSeconds(time);
        }
        public void SetAreaIsSafe()
        {
            SafePoint = true;
        }

        public void SetIsBusy()
        {
            IsBusy = true;
            _setIsNotBusyCoroutine = StartCoroutine(SetIsNotBusy(120));
        }

        private Coroutine _setIsNotBusyCoroutine;
        private IEnumerator SetIsNotBusy(float time)
        {
            yield return new WaitForSeconds(time);
            IsBusy = false;
        }
    }
}


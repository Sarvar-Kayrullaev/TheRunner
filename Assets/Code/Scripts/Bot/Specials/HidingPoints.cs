using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class HidingPoints : MonoBehaviour
    {
        public bool isBusy = false;
        public Transform front;
        public Transform back;
        public Transform left;
        public Transform right;

        public Transform GetHidingPoints(Vector3 againstEnemy)
        {
            isBusy = true;
            float angle = Angle(transform, againstEnemy);

            if (angle > 135)
            {
                //back
                return front;
            }
            else if (angle > 45)
            {
                return left;
                //right
            }
            else if (angle > -45)
            {
                return back;
                //front
            }
            else if (angle > -135)
            {
                return right;
                //left
            }
            else
            {
                return front;
                //back
            }
        }
        void CancelBusy()
        {
            isBusy = false;
        }
        public float Angle(Transform from, Vector3 target)
        {
            Vector3 targetPosition = new Vector3(target.x, from.transform.position.y, target.z);

            float angle = Vector3.Angle((targetPosition - from.position), -from.forward);
            float angle2 = Vector3.Angle((targetPosition - from.position), -from.right);

            if (angle2 > 90)
            {
                angle = (360 - angle) - 180;
            }
            else
            {
                angle = angle - 180;
            }
            return angle;
        }
    }

}

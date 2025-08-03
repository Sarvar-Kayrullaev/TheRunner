using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerRoot
{
    public class Utility
    {
        public static bool IsGrounded(Player player)
        {
            Transform transform = player.transform;
            Vector3 groundPosition = transform.position;
            groundPosition.y -= (player.character.height / 2) - player.GroundCheckPositionY;
            return Physics.CheckSphere(groundPosition, player.GroundCheckRadius, player.groundMask);
        }

        public static Vector3 GetSlopeNormal(Player player)
        {
            Transform transform = player.transform;
            Vector3 groundPosition = transform.position;
            groundPosition.y -= (player.character.height / 2) - player.GroundCheckPositionY;
            RaycastHit hit;
            float minDistance = float.MaxValue;
            RaycastHit nearestHit = new RaycastHit();

            if (Physics.Raycast(groundPosition, Vector3.down * 1f, out hit))
            {
                Debug.DrawRay(groundPosition, Vector3.down * 1f, Color.red);
                nearestHit = hit;
            }
            else
            {
                int numRaycasts = 8;
                RaycastHit[] hits = new RaycastHit[numRaycasts];
                for (int a = 1; a < 3; a++)
                {
                    for (int i = 0; i < numRaycasts; i++)
                    {
                        float angle = i * (360 / numRaycasts);
                        Vector3 direction = Quaternion.Euler(35 * a, angle, 0) * (Vector3.down * 1f);
                        Debug.DrawRay(groundPosition, direction, Color.red);
                        if (Physics.Raycast(groundPosition, direction, out hits[i]))
                        {
                            float distance = Vector3.Distance(groundPosition, hits[i].point);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                nearestHit = hits[i];
                            }
                        }
                    }
                }
            }

            return nearestHit.normal;
        }
    }
}

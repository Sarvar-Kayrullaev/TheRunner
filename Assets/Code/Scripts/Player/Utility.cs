using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerRoot
{
    public class Utility
    {
        public static bool IsGrounded(Player player)
        {
            var transform = player.transform;
            var groundPosition = transform.position;
            groundPosition.y -= (player.character.height / 2) - player.GroundCheckPositionY;
            return Physics.CheckSphere(groundPosition, player.GroundCheckRadius, player.groundMask);
        }

        public static bool IsSlopeGrounded(Player player)
        {
            var transform = player.transform;
            var groundPosition = transform.position;
            groundPosition.y -= (player.character.height / 2) - player.GroundCheckPositionY;
            return Physics.CheckSphere(groundPosition, player.GroundCheckRadius * 1.2f, player.groundMask);
        }
        
        public static bool IsGroundedTripleChecked(Player player)
        {
            var transform = player.transform;
            var groundCheckPosition = new Vector3(transform.position.x, transform.position.y - player.character.height * 0.26f, transform.position.z);
            var grounded1 = Physics.CheckSphere(groundCheckPosition, 0.45f, player.groundMask);
            var grounded2 = Physics.Raycast(transform.position, Vector3.down, player.character.height * 0.5f + 0.25f, player.groundMask);
            if (player.character.isGrounded) return true;
            return grounded1 || grounded2;
        }

        public static Vector3 GetSlopeNormal(Player player)
        {
            var transform = player.transform;
            var groundPosition = transform.position;
            groundPosition.y -= (player.character.height / 2) - player.GroundCheckPositionY;
            RaycastHit hit;
            var minDistance = float.MaxValue;
            var nearestHit = new RaycastHit();

            if (Physics.Raycast(groundPosition, Vector3.down * 1f, out hit))
            {
                Debug.DrawRay(groundPosition, Vector3.down * 1f, Color.red);
                nearestHit = hit;
            }
            else
            {
                var numRaycasts = 8;
                var hits = new RaycastHit[numRaycasts];
                for (var a = 1; a < 3; a++)
                {
                    for (var i = 0; i < numRaycasts; i++)
                    {
                        float angle = i * (360 / numRaycasts);
                        var direction = Quaternion.Euler(35 * a, angle, 0) * (Vector3.down * 1f);
                        Debug.DrawRay(groundPosition, direction, Color.red);
                        if (Physics.Raycast(groundPosition, direction, out hits[i]))
                        {
                            var distance = Vector3.Distance(groundPosition, hits[i].point);
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
        
        public static bool OnSlope(Player player, out RaycastHit slopeHit)
        {
            if (Physics.Raycast(player.transform.position, Vector3.down, out slopeHit, player.character.height * 0.5f + 1f))
            {
                var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < 45 && angle != 0;
            }

            return false;
        }
        
        public static Vector3 GetSlopeMoveDirection(Vector3 moveDirection, RaycastHit slopeHit)
        {
            return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
        }
    }
}

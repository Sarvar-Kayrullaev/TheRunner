using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PlayerRoot
{
    public class LostManager : MonoBehaviour
    {
        public float back = 3;
        public float height = 1.75f;
        public float smoothRotate = 6f; // The speed at which the camera follows the player.
        public float smoothDistance = 12;

        private Camera deathCamera;
        private Transform ragdollBody;
        private Player player;
        private bool isInitialized = false;

        [Header("Interface Setup")]
        [SerializeField] GameObject GameplayScreen;
        [SerializeField] GameObject LostScreen;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text descriptionText;

        public void DiedPlayer(Player player)
        {
            player.playerAudio.audio.PlayOneShot(player.playerAudio.MUSIC_DYING_THEME, player.playerAudio.Volume);
            player.character.enabled = false;
            player.collider.enabled = false;
            player.HeadBody.gameObject.SetActive(false);
            player.Bones.gameObject.SetActive(false);
            player.playerCamera.gameObject.SetActive(false);
            player.deathCamera.gameObject.SetActive(true);
            player.RagdollBody.gameObject.SetActive(true);
            player.gravity = 0;
            player.canMove = false;

            GameplayScreen.SetActive(false);
            LostScreen.SetActive(true);
            titleText.text = "You have failed";
            descriptionText.gameObject.SetActive(false);

            this.player = player;
            deathCamera = player.deathCamera;
            ragdollBody = player.RagdollBody;
            isInitialized = true;
        }

        public void FailedPlayer(Player player, string description)
        {
            player.playerAudio.audio.PlayOneShot(player.playerAudio.MUSIC_DYING_THEME, player.playerAudio.Volume);
            player.HeadBody.gameObject.SetActive(false);
            
            player.canMove = false;

            GameplayScreen.SetActive(false);
            LostScreen.SetActive(true);
            titleText.text = "Mission failed";
            descriptionText.text = description;

            this.player = player;
            isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!isInitialized) return;

            // Calculate the camera's target position.
            Vector3 behindPosition = (-ragdollBody.forward * back) + (ragdollBody.up * height);
            Vector3 targetPosition = ragdollBody.position + behindPosition;

            // Smoothly move the camera to the target position.
            Vector3 follow = Vector3.Lerp(deathCamera.transform.position, targetPosition, smoothRotate * Time.deltaTime);
            deathCamera.transform.position = new Vector3(follow.x, follow.y, follow.z);

            // Look at the player.
            deathCamera.transform.LookAt(ragdollBody);
        }
    }
}


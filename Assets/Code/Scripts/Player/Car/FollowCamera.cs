using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowCamera : MonoBehaviour
{
    public Transform player; // The player to follow.
    public float back = 3;
    public float height = 1.75f;
    public float smoothRotate = 6f; // The speed at which the camera follows the player.
    public float smoothDistance = 12;

    private void FixedUpdate()
    {
        // Calculate the camera's target position.
        Vector3 behindPosition = (-player.forward*back)+ (player.up*height);
        Vector3 targetPosition = player.position + behindPosition;

        // Smoothly move the camera to the target position.
        Vector3 follow = Vector3.Lerp(transform.position, targetPosition, smoothRotate * Time.deltaTime);
        transform.position = new Vector3(follow.x,follow.y,follow.z);

        // Look at the player.
        transform.LookAt(player);
    }

    public void Update(){
        if(Input.GetKey(KeyCode.P)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    private float smooth;
    public float startSmooth;
    public float endSmooth;
    public float swayMultiplayer;
    public float moveSwayMultiplayer;
    void Start()
    {
        
    }

    void Update()
    {
        return;
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        float horizontalAxis = (Input.GetAxis("Horizontal") - mouseX);

        mouseX *= swayMultiplayer;
        mouseY *= swayMultiplayer;
        horizontalAxis *= moveSwayMultiplayer;



        Quaternion rotationX = Quaternion.AngleAxis(mouseY, Vector3.left);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion rotationZ = Quaternion.AngleAxis(horizontalAxis, Vector3.back);


        Quaternion targetRotation = rotationX * rotationY * rotationZ;
        smooth = horizontalAxis <= 0.7f && horizontalAxis >= -0.7f ? endSmooth : startSmooth;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    public void Sway(bool mobile ,bool aim, Vector2 screenAxis, float horizontal, float reduseSway)
    {
        float mouseX;
        float mouseY;
        float horizontalAxis;

        if (mobile)
        {
            mouseX = screenAxis.x * 0.02f;
            mouseY = screenAxis.y * 0.02f;
            horizontalAxis = horizontal - mouseX;
        }
        else
        {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");
            horizontalAxis = (Input.GetAxis("Horizontal") - mouseX);
        }
        

        mouseX *= swayMultiplayer * reduseSway;
        mouseY *= swayMultiplayer * reduseSway;
        horizontalAxis *= aim ? moveSwayMultiplayer/3 : moveSwayMultiplayer;



        Quaternion rotationX = Quaternion.AngleAxis(mouseY, Vector3.left);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion rotationZ = Quaternion.AngleAxis(horizontalAxis, Vector3.back);


        Quaternion targetRotation = rotationX * rotationY * rotationZ;
        smooth = horizontalAxis <= 0.7f && horizontalAxis >= -0.7f ? endSmooth : startSmooth;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}

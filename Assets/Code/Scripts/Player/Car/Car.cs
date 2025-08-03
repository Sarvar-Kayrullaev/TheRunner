using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Car : MonoBehaviour
{
    public float Speed;
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public WheelCollider WheelRL;
    public WheelCollider WheelRR;
    public Transform WheelFLtrans;
    public Transform WheelFRtrans;
    public Transform WheelRLtrans;
    public Transform WheelRRtrans;
    public Vector3 eulertest;
    private bool braked = false;
    public float maxBrakeTorque = 500;
    public float maxSpeed = 50;
    private Rigidbody rb;
    public Transform centreofmass;
    public float maxTorque = 1000;
    float Vertical = 0;
    float Horizontal = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreofmass.transform.localPosition;
    }

    void FixedUpdate()
    {
        if (!braked)
        {
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
            WheelRL.brakeTorque = 0;
            WheelRR.brakeTorque = 0;
        }
        //speed of car, Car will move as you will provide the input to it.

        if (Speed < maxSpeed)
        {
            WheelRR.motorTorque = maxTorque * Vertical;
            WheelRL.motorTorque = maxTorque * Vertical;
        }
        else
        {
            WheelRR.motorTorque = 0;
            WheelRL.motorTorque = 0;
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity,maxSpeed);
        }

        /*changing car direction
        Here we are changing the steer angle of the front tyres of the car so that we can change the car direction.*/
        WheelFL.steerAngle = 45 * Horizontal;
        WheelFR.steerAngle = 45 * Horizontal;
    }
    void Update()
    {
        HandBrake();

        //for tyre rotate
        WheelFLtrans.Rotate(WheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        WheelFRtrans.Rotate(WheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        WheelRLtrans.Rotate(WheelRL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        WheelRRtrans.Rotate(WheelRL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        //changing tyre direction
        Vector3 temp = WheelFLtrans.localEulerAngles;
        Vector3 temp1 = WheelFRtrans.localEulerAngles;
        temp.y = WheelFL.steerAngle - (WheelFLtrans.localEulerAngles.z);
        WheelFLtrans.localEulerAngles = temp;
        temp1.y = WheelFR.steerAngle - WheelFRtrans.localEulerAngles.z;
        WheelFRtrans.localEulerAngles = temp1;
        eulertest = WheelFLtrans.localEulerAngles;

        Speed = rb.linearVelocity.magnitude;
    }
    void HandBrake()
    {
        //Debug.Log("brakes " + braked);
        if (Input.GetButton("Jump"))
        {
            braked = true;
        }
        else
        {
            braked = false;
        }
        if (braked)
        {

            WheelRL.brakeTorque = maxBrakeTorque * 20;//0000;
            WheelRR.brakeTorque = maxBrakeTorque * 20;//0000;
            WheelRL.motorTorque = 0;
            WheelRR.motorTorque = 0;
        }
    }

    public void VerticalInput(float value)
    {
        Vertical = value;
    }
    public void HorizontalInput(float value)
    {
        Horizontal = value;
    }
}

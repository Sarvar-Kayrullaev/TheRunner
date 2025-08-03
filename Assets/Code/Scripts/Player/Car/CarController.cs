using System.Collections;
using System.Collections.Generic;
using PlayerRoot;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [Header("Player Settings")]
    public float listenerDistance = 0.5f;
    public Transform playerPosition;
    public Transform playerOutPosition;

    public Transform TransmissionParticle;
    public Transform[] TransmissionParticlePoint;
    public new Camera camera;
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;
    public float enginePower = 1000f;
    public float maxSteerAngle = 30f;
    public float maxBrakeTorque = 100;
    public float autoStopTorque = 5;
    public int numberOfGears = 5;
    public float[] gearTorques;
    public float[] gearSpeeds;

    public AudioClip TireRollingClip;
    public AudioClip IdleEngineClip;
    public AudioClip MovingEngineClip;
    public AudioClip StartEngineClip;
    public AudioClip StopEngineClip;
    public AudioClip[] TransistorSounds;
    public AudioSource TireRollingAudioSource;
    public AudioSource EngineAudioSource;
    public AudioSource OneShotAudioSource;
    private bool gearAudioPlayed = false;

    public int currentGear = 1;
    private int previousGear;

    [HideInInspector] public bool activate = false;
    private bool braked = false;
    private new Rigidbody rigidbody;
    public float maxSpeed = 0;
    private float TransmissionTime = 0.07f;
    private float PreviousGearBackTime = 0.8f;
    private float currentTransmissionTime;
    private float currentPreviousGearBackTime;
    private float currentPitch = 0;


    public Slider slider;
    [HideInInspector] public Player player;
    private bool playerIsClose = false;
    void Start()
    {
        player = FindFirstObjectByType<Player>();

        maxSpeed = gearSpeeds[0];
        rigidbody = GetComponent<Rigidbody>();
        maxSpeed = gearSpeeds[numberOfGears];
        currentTransmissionTime = TransmissionTime;

        InvokeRepeating(nameof(UpdateListener), 0, 0.4f);
    }
    void UpdateListener()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < listenerDistance)
        {
            player.dragableVehicle.Register(this, true);
            playerIsClose = true;
        }
        else
        {
            player.dragableVehicle.Register(this, false);
            playerIsClose = false;
        }
    }
    void Update()
    {
        if (playerIsClose)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (activate) player.dragableVehicle.Disembark();
                else player.dragableVehicle.Ride();
            }
        }
        if (activate == false)
        {
            rearLeftWheel.brakeTorque = maxBrakeTorque * 20;
            rearRightWheel.brakeTorque = maxBrakeTorque * 20;
            return;
        }

        // Get input for acceleration and braking
        float acceleration = Input.GetAxis("Vertical");
        float steering = Input.GetAxis("Horizontal");

        currentTransmissionTime -= Time.deltaTime;
        currentPreviousGearBackTime -= Time.deltaTime;

        if (currentTransmissionTime <= 0)
        {
            ApplyThrottle(acceleration);
            ApplySteering(steering);
            UpdateGear(acceleration);
            HandBrake();
        }
        else
        {
            braked = true;
            TransmissionBrake();
        }

        // Display speed (for testing purposes)
        DisplaySpeed();
        WheelRotation();
        Audio(acceleration);
        Audio2(acceleration);
    }

    void FixedUpdate()
    {
        // if (!braked)
        // {
        //     rearLeftWheel.brakeTorque = 0;
        //     rearRightWheel.brakeTorque = 0;
        // }
    }

    void WheelRotation()
    {
        frontLeftWheelTransform.Rotate(frontLeftWheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        frontRightWheelTransform.Rotate(frontRightWheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        rearLeftWheelTransform.Rotate(rearLeftWheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        rearRightWheelTransform.Rotate(rearRightWheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);

        Vector3 temp = frontLeftWheelTransform.localEulerAngles;
        Vector3 temp1 = frontRightWheelTransform.localEulerAngles;

        temp.y = frontLeftWheel.steerAngle - (frontLeftWheelTransform.localEulerAngles.z);
        frontLeftWheelTransform.localEulerAngles = temp;
        temp1.y = frontRightWheel.steerAngle - frontRightWheelTransform.localEulerAngles.z;
        frontRightWheelTransform.localEulerAngles = temp1;
    }
    void ApplyThrottle(float throttleInput)
    {
        float torque = throttleInput * gearTorques[currentGear];
        float speed = rigidbody.linearVelocity.magnitude * 3.6f; // Convert m/s to km/h
        if (speed <= maxSpeed)
        {
            frontLeftWheel.brakeTorque = 0;
            frontRightWheel.brakeTorque = 0;
            rearLeftWheel.motorTorque = torque;
            rearRightWheel.motorTorque = torque;
        }
        else
        {
            rearLeftWheel.motorTorque = 0;
            rearRightWheel.motorTorque = 0;

            //rigidbody.linearVelocity = Vector3.ClampMagnitude(rigidbody.linearVelocity, gearSpeeds[currentGear - 1]);
        }
        if (throttleInput == 0)
        {
            frontLeftWheel.brakeTorque = autoStopTorque;
            frontRightWheel.brakeTorque = autoStopTorque;
            //rearLeftWheel.brakeTorque = autoStopTorque;
            //rearRightWheel.brakeTorque = autoStopTorque;
        }
    }

    void ApplySteering(float steeringInput)
    {
        float steerAngle = maxSteerAngle * steeringInput;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    }

    void UpdateGear(float acceleration)
    {
        float speed = rigidbody.linearVelocity.magnitude * 3.6f; // Convert m/s to km/h

        if (currentGear < numberOfGears && speed > gearSpeeds[currentGear])
        {
            gearAudioPlayed = false;
            previousGear = currentGear;
            currentGear++;
            currentTransmissionTime = TransmissionTime;
            currentPreviousGearBackTime = PreviousGearBackTime;
            currentPitch = 0;
            foreach (Transform item in TransmissionParticlePoint)
            {
                //Transform particle = Instantiate(TransmissionParticle, item);
                //Destroy(particle.gameObject, 0.3f);
            }
        }
        else if (currentGear > 1 && speed < gearSpeeds[currentGear - 1] && currentPreviousGearBackTime <= 0 && acceleration == 0)
        {
            gearAudioPlayed = false;
            currentTransmissionTime = TransmissionTime;
            previousGear = currentGear;
            currentGear--;
        }
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
            rearLeftWheel.brakeTorque = maxBrakeTorque * 20;
            rearRightWheel.brakeTorque = maxBrakeTorque * 20;
        }
        else
        {
            rearLeftWheel.brakeTorque = 0;
            rearRightWheel.brakeTorque = 0;
        }
    }
    void TransmissionBrake()
    {
        rearLeftWheel.brakeTorque = maxBrakeTorque * 20;
        rearRightWheel.brakeTorque = maxBrakeTorque * 20;
    }

    void DisplaySpeed()
    {
        float speed = rigidbody.linearVelocity.magnitude * 3.6f; // Convert m/s to km/h
        //Debug.Log("Speed: " + Mathf.Round(speed) + " km/h");
    }

    void Audio(float torque)
    {
        currentPitch = Mathf.Lerp(currentPitch, 1, 0.005f);
        //slider.value = Lerp(currentPitch, 0.8f, 1);
        float normalizedPitch = Lerp(currentPitch, 0.8f, 1);
        float speed = rigidbody.linearVelocity.magnitude * 3.6f;

        float tireNormalizedValueVolume = Normalize(speed, 0, maxSpeed, 0);
        float tireMaxVolume = 0.1f;
        float volume = tireMaxVolume * tireNormalizedValueVolume;

        TireRollingAudioSource.volume = volume;

        float normalizedValuePitch = Normalize(speed, 0, maxSpeed, 0.3f);
        EngineAudioSource.pitch = normalizedValuePitch;

        float engineNormalizedValueVolume = Normalize(speed, 0, maxSpeed, 0.05f);
        float engineMaxVolume = 0.2f;
        EngineAudioSource.volume = engineMaxVolume *normalizedValuePitch;
        if (currentGear != previousGear && speed > 0 && !gearAudioPlayed)
        {
            //Play
            gearAudioPlayed = true;
            //StartCoroutine(PlayAudio(TransistorSounds[0], 0.0f, volume, false, true));
            StartCoroutine(PlayAudio(TireRollingAudioSource, TireRollingClip, 0.0f, volume, true, false));
            StartCoroutine(PlayAudio(EngineAudioSource, MovingEngineClip, 0.0f, 0.3f, true, false));
            StartCoroutine(PlayAudio(OneShotAudioSource, StartEngineClip, 0.0f, 0.3f, false, true));
            Debug.Log("CarSound 2");
        }
        else
        {

        }
    }

    void Audio2(float torque)
    {
        //currentPitch = Mathf.Lerp(currentPitch, 1,0.1f);
        //slider.value = MinNormalizedValue(currentPitch,0.9f,1);
    }

    float Lerp(float cursor, float min, float max)
    {
        return (float)(cursor * (max - min) + min);
    }

    IEnumerator PlayAudio(AudioSource source,AudioClip clip, float delay,float volume, bool loop, bool playOneShot)
    {
        yield return new WaitForSeconds(delay);
        if (source.isPlaying) source.Stop();
        source.loop = loop;
        source.clip = clip;
        source.time = 0.1f;
        if (playOneShot) source.PlayOneShot(clip, volume);
        else source.Play();
    }

    public static float Normalize(float currentValue, float minValue, float maxValue, float minNormalizedValue)
    {
        if (currentValue < minValue)
        {
            return 0.0f; // Below the minimum value, normalize to 0.0
        }
        else if (currentValue > maxValue)
        {
            return 1.0f; // Above the maximum value, normalize to 1.0
        }
        else
        {
            float range = maxValue - minValue;
            float normalizedValue = (currentValue - minValue) / range;
            float maxNormalizedValue = 1.0f;

            // Map the normalized value to the desired range
            float mappedValue = minNormalizedValue + normalizedValue * (maxNormalizedValue - minNormalizedValue);
            return mappedValue;
        }
    }
}

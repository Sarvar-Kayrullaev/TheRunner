using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerRoot;
public class HeadBob : MonoBehaviour
{
    [SerializeField] private bool _enable;
    public bool isGrounded;
    [HideInInspector] public float _amplitude = 0.002f;
    [HideInInspector] public float _frequency = 17;
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;

    [Space(2)]
    [Header("Aim")]
    [SerializeField, Range(0, 0.1f)] public float _amplitudeAim = 0.002f;
    [SerializeField, Range(0, 30)] public float _frequencyAim = 17;
    [Space(2)]
    [Header("Run")]
    [SerializeField, Range(0, 0.1f)] public float _amplitudeRun = 0.002f;
    [SerializeField, Range(0, 30)] public float _frequencyRun = 17;
    [Space(2)]
    [Header("Spring")]
    [SerializeField, Range(0, 0.1f)] public float _amplitudeSpring = 0.004f;
    [SerializeField, Range(0, 30)] public float _frequencySpring = 20;
    [Space(4)]
    [Header("Mobile")]
    [SerializeField, Range(0, 0.03f)] public float _ampMin = 0.005f;
    [SerializeField, Range(0, 30)] public float _freqMin = 17;
    [SerializeField, Range(0, 0.03f)] public float _ampMax = 0.01f;
    [SerializeField, Range(0, 30)] public float _freqMax = 20;
    [Space(2)]
    public float _toggleSpeed = 3;

    private Player player;
    private Vector3 _startPos;
    private CharacterController controller;
    Gameplay.Debug debug;

    [System.Obsolete]
    private void Awake()
    {
        if (TryGetComponent(out Player player)) this.player = player;
        if (TryGetComponent(out CharacterController controller)) this.controller = controller;
        _startPos = _camera.localPosition;
        debug = GameObject.FindObjectOfType<Gameplay.Debug>();
    }
    private void Update()
    {
        if (!_enable) return;

        CheckMotion();
        ResetPosition();
        _camera.LookAt(FocusTarget());
        _camera.localEulerAngles = new(0, _camera.localEulerAngles.y, _camera.localEulerAngles.z);
    }
    void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }
    void CheckMotion()
    {
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        if (player.holster.currentWeapon)
        {
            if (player.holster.currentWeapon.aim)
            {
                if (speed < _toggleSpeed / 4) return;
            }
            else
            {
                if (speed < _toggleSpeed) return;
            }
        }
        if (!isGrounded) return;
        PlayMotion(FootStepMotion());
    }
    Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;
        return pos;
    }
    void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 5 * Time.deltaTime);
    }
    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);

        //RaycastHit hit;
        //Vector3 fwd = _cameraHolder.transform.TransformDirection(Vector3.forward);
        //if (Physics.Raycast(_cameraHolder.transform.position, fwd, out hit, 100))
        //{
        //    print(hit.transform.name);
        //    pos = hit.point;
        //}
        //else
        //{
        //    pos += _cameraHolder.forward * 15;
        //}
        pos += _cameraHolder.forward * 15;
        return pos;
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class ControllerManager : MonoBehaviour
{
    public Transform controllersParent;
    private Transform _shootTransform;
    private Transform _assistTransform;
    private Transform _aimTransform;
    private Transform _reloadTransform;
    private Transform _throwTransform;
    private Transform _jumpTransform;
    private Transform _crouchTransform;
    private Transform _joystickTransform;
    private Transform _rideCarTransform;
    
    private AdvancedButton _assistButton;
    private Image _assistImage;

    [Space] 
    [Header("Assist Button References")]
    [SerializeField] private Sprite assistPickUpSprite;
    [SerializeField] private Sprite assistShootSprite;


    private void Awake()
    {
        _shootTransform = controllersParent.Find("Shoot");
        _assistTransform = controllersParent.Find("Assist");
        _aimTransform = controllersParent.Find("Aim");
        _reloadTransform = controllersParent.Find("Reload");
        _throwTransform = controllersParent.Find("Throw");
        _jumpTransform = controllersParent.Find("Jump");
        _crouchTransform = controllersParent.Find("Crouch");
        _joystickTransform = controllersParent.Find("Joystick");
        _rideCarTransform = controllersParent.Find("RideCar");
        
        _assistTransform.TryGetComponent(out _assistButton);
        _assistTransform.TryGetComponent(out _assistImage);
    }

    public void ChangeAssistButtonListener(AssistButtonType type )
    {
        if (type == AssistButtonType.Shootable)
        {
            _assistButton.name = ControllerCases.Shoot;
            _assistImage.sprite = assistShootSprite;
        }
        else if(type == AssistButtonType.Pickable)
        {
            _assistButton.name = ControllerCases.PickUp;
            _assistImage.sprite = assistPickUpSprite;
        }
    }
}

public enum AssistButtonType
{
    Shootable,
    Pickable,
    Doer
}

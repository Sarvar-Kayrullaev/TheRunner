using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using BotRoot;

namespace PlayerRoot
{
    public class Player : MonoBehaviour, ButtonListener
    {
        public bool Mobile = true;
        public bool canMove = true;
        public bool isContollable = true;
        [HideInInspector] public bool Died = false;
        [Space]
        [Header("Health")]
        public int health;
        public int rowByHealth;
        public HealthBar healthBar;

        [Space]
        [Header("Movement Params")]
        public float moveSpeed = 10f;
        public float walkingSpeed = 7.5f;
        public float runningSpeed = 11.5f;

        [Space]
        [Header("Crouching Params")]
        public float crouchHeight = 1;
        public float crouchingSmooth = 1;

        [Space]
        [Header("Jump Params")]
        public float jumpSpeed = 8.0f;
        public float gravity = 9.0f;

        [Space]
        [Header("Slip Params")]
        public float slideFriction = 0.3f;
        public float slipAngle;

        [Space]
        [Header("Camera Params")]
        public float Sensitivity = 2;
        public float lookSpeed = 1;
        public float lookXLimit = 45.0f;

        [Space]
        [Header("Ground Check Params")]
        public bool OnGround = false;
        public LayerMask groundMask;
        public float GroundCheckPositionY;
        public float GroundCheckRadius;

        [Space]
        [Header("Setup")]
        public Camera playerCamera;
        public Camera deathCamera;
        public Camera stackCamera;
        public Transform cameraParent;
        public Transform Bones;
        public Transform Head;
        public Transform HeadBody;
        public Transform RagdollBody;
        public OnceSway weaponOnceSway;
        public WeaponHolster holster;
        public VirtualJoystick joystick;
        public LookController look;

        [Space]
        [Header("Movement Sounds")]
        public new AudioSource audio = default;
        public AudioClip[] walkSounds;
        public AudioClip[] runSounds;
        public AudioClip[] woodWalkSounds;
        public AudioClip[] woodRunSounds;
        private float footstepTimer;

        [Header("Action Sound")]
        public AudioClip[] climbingSound;
        public AudioClip[] jumpDownSound;
        public AudioClip[] woodJumpDownSound;
        public AudioClip[] damageSound;
        public AudioClip takeDamageSound;
        public AudioClip SwooshSound;

        [Space]
        [Header("Scripts")]
        public CharacterController character;
        new public CapsuleCollider collider;
        public HeadBob headBob;
        public LiveAction liveAction;
        public Health playerHealth;
        public Movement movement;
        public TargetStatus status;
        public TakeDamageManager takeDamageManager;
        public DamageNavigatorRegister damageNavigatorRegister;
        public LostManager lostManager;
        public DragableWeapon dragableWeapon;
        public DragableVehicle dragableVehicle;
        public PlayerAudio playerAudio;

        [Space]
        public Vector2 vectorMovement;
        public Vector3 moveDirection = Vector3.zero;
        Vector2 move;
        Vector2 mouse;

        [HideInInspector] public Dragable PendingDragableWeapon;

        [System.Obsolete]
        void Start()
        {
            if (TryGetComponent(out HeadBob headBob)) this.headBob = headBob;
            if (TryGetComponent(out LiveAction liveAction)) this.liveAction = liveAction;
            if (TryGetComponent(out CharacterController character)) this.character = character;
            if (TryGetComponent(out CapsuleCollider collider)) this.collider = collider;
            if (TryGetComponent(out TargetStatus status)) this.status = status;

            dragableVehicle.player = this;

            movement = new Movement();
            movement.Initialize(this);

            playerHealth = new Health();
            playerHealth.Initialize(this);

            lookSpeed = Sensitivity;
            holster.Mobile = Mobile;
            if (!Mobile)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }
        void Update()
        {
            if(!isContollable) return;
            if (Mobile)
            {
                move = new Vector2(joystick.movement.x, joystick.movement.y);
                mouse = new Vector2(look.vector.x, look.vector.y);
            }
            else
            {
                move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                if (Input.GetKeyDown(KeyCode.P))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }

            movement.Update(move, mouse);

            if (holster.currentWeapon) holster.currentWeapon.sway.Sway(Mobile, holster.currentWeapon.aim, mouse, move.x, holster.currentWeapon.ReduceSwayOnAim);
        }

        public void TakeDamage(int damage, Transform owner)
        {
            playerHealth.TakeDamage(damage);
            takeDamageManager.DamageTaken();
            PlaySplitRandom(takeDamageSound, 0.15f, 7, audio);
        }

        public void PlaySplitRandom(AudioClip clip, float splitTime, int clipLength, AudioSource audio)
        {
            audio.clip = clip;
            audio.time = splitTime * Random.Range(0, clipLength - 1);
            audio.Play();

            async Task Wait()
            {
                await Task.Delay((int)splitTime * 100);
                {
                    if (audio.isPlaying) audio.Stop();
                }
            }
            _ = Wait();
        }

        void ButtonListener.OnClickDown(ControllerCases name)
        {
            if (name == ControllerCases.Shoot)
            {
                if (!holster.currentWeapon) return;
                holster.currentWeapon.shootOnce = true;
                holster.currentWeapon.shootPressing = true;
            }
            else if (name == ControllerCases.Aim)
            {
                if (!holster.currentWeapon) return;
                holster.currentWeapon.Aim(!holster.currentWeapon.aim);
            }
            else if (name == ControllerCases.Reload)
            {
                if (!holster.currentWeapon) return;
                holster.currentWeapon.Reload();
            }
            else if (name == ControllerCases.Jump)
            {
                movement.MobileJump();
            }
            else if (name == ControllerCases.Throw)
            {
                holster.RockThrow();
            }
            else if (name == ControllerCases.SwitchWeapon)
            {
                //holster.SwitchWeapon();
            }
            else if (name == ControllerCases.Crouch)
            {
                movement.Crouch();
            }
        }

        void ButtonListener.OnClickUp(ControllerCases name)
        {
            if (name == ControllerCases.Shoot)
            {
                if (holster.currentWeapon)
                    holster.currentWeapon.shootPressing = false;
            }
        }

        void ButtonListener.OnDragChange(PointerEventData eventData, ControllerCases name)
        {
            if (name == ControllerCases.Shoot)
            {
                movement.Rotate(eventData.delta);
                if (holster.currentWeapon)
                    holster.currentWeapon.sway.Sway(Mobile, holster.currentWeapon.aim, eventData.delta * 2, 0, holster.currentWeapon.ReduceSwayOnAim);
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            movement.TriggerEnter(collider);
        }
        void OnTriggerExit(Collider collider)
        {
            movement.TriggerExit(collider);
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.blue;
            Vector3 ground = transform.position;
            ground.y -= (GetComponent<CharacterController>().height / 2) - GroundCheckPositionY;
            Gizmos.DrawSphere(ground, GroundCheckRadius);
        }
    }
}

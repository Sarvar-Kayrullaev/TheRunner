using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BotRoot
{
    public class EnemyIndicator : MonoBehaviour
    {

        public EnemyIndicatorController indicatorController;
        public AudioSource audioSource;
        public float soundVolume = 1;
        public AudioClip identifiedSound;
        public AudioClip identifyStartSound;
        public Transform owner;
        public Transform target;
        private RectTransform rect;
        public RectTransform arrowBackground;
        public RectTransform arrowForeground;
        public Color alphaColor;
        private bool destroyed = false;
        private bool detected = false;

        private float destroySelfTime = 1;
        private float _destroySelfTimer;


        [SerializeField] bool destroy = false;
        Animator animator;
        [SerializeField] AnimationClip detectedAnimationClip;

        public void Register(Transform owner, Transform target, AudioSource audioSource)
        {
            this.owner = owner;
            this.target = target;
            this.audioSource = audioSource;

            if (GetComponent<Animator>())
            {
                animator = GetComponent<Animator>();
            }
            _destroySelfTimer = destroySelfTime;
            if (target == null || owner == null)
            {
                Destroy(gameObject);
                return;
            }
            StartBounceAnimate();

            BotSetup setup = owner.GetComponent<BotSetup>();
            setAdvansedProgress(setup.attribute.identifyEnemyTime, setup.attribute._identifyEnemyTime);
            RotateToTheTarget(owner, target);
        }

        private void Update()
        {
            if (destroy)
            {
                destroyed = true;
                Destroy(gameObject);
                return;
            }
            if (target == null || owner == null)
            {
                Destroy(gameObject);
                return;
            }

            BotSetup setup = owner.GetComponent<BotSetup>();

            if (!detected)
            {
                setAdvansedProgress(setup.attribute.identifyEnemyTime, setup.attribute._identifyEnemyTime);
            }
            RotateToTheTarget(owner, target);

            if (setup.attribute._identifyEnemyTime >= setup.attribute.identifyEnemyTime)
            {

                if (!destroyed && setup.attribute._identifyEnemyTime >= setup.attribute.identifyEnemyTime)
                {
                    destroyed = true;
                    detected = true;
                    audioSource.PlayOneShot(identifiedSound, soundVolume);
                    BotObjects botObjects = owner.GetComponent<BotObjects>();
                    botObjects.enemy = botObjects.futureEnemy;
                    if (botObjects.futureEnemy) setup.overall.SetLastEnemyVisiblePoint(botObjects.futureEnemy.position);
                    botObjects.lastEnemy = botObjects.futureEnemy;
                    setup.status.MentalState = BotEnum.MentalState.Panic;
                    owner.GetComponent<BotBase>().EnemyDetected = true;
                    setup.author.SetAlarmSignal(true);
                    //setup.utility.CallStaticEnemy(botObjects.futureEnemy);
                    Destroy(gameObject, 1f);
                }
                else
                {
                    Destroy(gameObject, 1f);
                }
                IdentificationAnimate();
            }
            else
            {
                if (!destroyed && setup.attribute._identifyEnemyTime <= 0)
                {
                    IdentificationEnd();
                    _destroySelfTimer -= Time.deltaTime;
                    if (_destroySelfTimer <= 0)
                    {
                        destroyed = true;
                        Destroy(gameObject);
                    }
                }
                else
                {
                    _destroySelfTimer = destroySelfTime;
                }
            }
        }

        protected RectTransform Rect
        {
            get
            {
                if (rect == null)
                {
                    rect = GetComponent<RectTransform>();
                    if (rect == null)
                    {
                        rect = gameObject.AddComponent<RectTransform>();
                    }
                }

                return rect;
            }
        }

        private Quaternion rotation = Quaternion.identity;
        private Vector3 position = Vector3.zero;
        public void setMaxProgress(float maxValue)
        {
            // slider.maxValue = maxValue;
        }

        public void setProgress(float currentValue)
        {
            // float progress = Mathf.Clamp01 (currentValue / 0.9f);
            // slider.value = progress;
        }

        public void setAdvansedProgress(float maxValue, float currentValue)
        {
            // setMaxProgress (maxValue);
            // setProgress (maxValue - currentValue);
            //fill.fillAmount = (1 / maxValue * maxValue) - (1 / maxValue * currentValue);
            indicatorController.progress = (currentValue / maxValue) * 100;

            //print(currentValue);
            //print("progress: "+(currentValue / maxValue) * 100);

        }

        public void LookAtTarget(Transform owner, Transform target)
        {
            float Angle = Vector3.Angle(owner.position, target.position);
            print("Angle = " + Angle);
        }

        public void RotateToTheTarget(Transform owner, Transform target)
        {
            if (owner)
            {
                position = owner.position;
                rotation = owner.rotation;
            }
            Vector3 direction = target.position - position;

            rotation = Quaternion.LookRotation(direction);
            rotation.z = -rotation.y;
            rotation.x = 0;
            rotation.y = 0;

            Vector3 northDirection = new Vector3(0, 0, target.eulerAngles.y - 180);
            Rect.localRotation = rotation * Quaternion.Euler(northDirection);
        }

        public void IdentificationAnimate()
        {
            animator.Play("IndicatorDetected");
        }

        public void IdentificationEnd()
        {
            animator.Play("IndicatorEnd");
        }

        public void StartBounceAnimate()
        {
            //Animation
            audioSource.PlayOneShot(identifyStartSound, soundVolume);
        }
    }
}

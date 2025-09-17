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
        public BotSetup setup;
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

        public void Register(Transform owner, Transform target, AudioSource audioSource, BotSetup setup)
        {
            this.owner = owner;
            this.target = target;
            this.audioSource = audioSource;
            this.setup = setup;

            if (TryGetComponent(out Animator animator))
            {
                this.animator = animator;
            }
            _destroySelfTimer = destroySelfTime;
            if (!target || !owner)
            {
                Destroy(gameObject);
                return;
            }
            StartBounceAnimate();

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
            if (!target || !owner)
            {
                Destroy(gameObject);
                return;
            }
            
            if(!setup) return;
            
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
                    var botObjects = setup.objects;
                    botObjects.enemy = botObjects.futureEnemy;
                    if (botObjects.futureEnemy) setup.overall.SetLastEnemyVisiblePoint(botObjects.futureEnemy.position);
                    botObjects.lastEnemy = botObjects.futureEnemy;
                    setup.status.MentalState = BotEnum.MentalState.Panic;
                    setup.memory.EnemyDetected = true;
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

        private RectTransform Rect
        {
            get
            {
                if (rect is not null) return rect;
                if (TryGetComponent(out RectTransform rectTransform))
                {
                    rect =  rectTransform;
                    return rect;
                }
                else
                {
                    //rect = gameObject.AddComponent<RectTransform>();
                    return null;
                }
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

        public void RotateToTheTarget(Transform _owner, Transform _target)
        {
            if (_owner)
            {
                position = _owner.position;
                rotation = _owner.rotation;
            }
            var direction = _target.position - position;

            rotation = Quaternion.LookRotation(direction);
            rotation.z = -rotation.y;
            rotation.x = 0;
            rotation.y = 0;

            var northDirection = new Vector3(0, 0, _target.eulerAngles.y - 180);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BotRoot
{
    public class Health : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        public int health;
        [HideInInspector] public int currentHealth;
        public bool died = false;
        [Space(2)]
        [SerializeField] float ragdollDrag;
        [SerializeField] float healthSliderSpeed;
        [SerializeField] RagdollObjects ragdoll;
        public Slider healthSlider;
        public float healthBarDestroyTime;
        [SerializeField] Transform bar;
        public Transform Marker;
        [SerializeField] Animator animator;

        private Camera mainCamera;
        private Vector3 hitPoint;
        private void Awake()
        {
            currentHealth = health;
            mainCamera = Camera.main;
            if (healthSlider)
            {
                healthSlider.maxValue = health;
                healthSlider.value = health;
            }
        }
        private void Update()
        {
            if (healthSlider)
            {
                healthSlider.value = Mathf.MoveTowards(healthSlider.value, currentHealth, healthSliderSpeed);
                healthBarDestroyTime -= Time.deltaTime;
                if (healthBarDestroyTime <= 0)
                {
                    Destroy(healthSlider.gameObject);
                    healthSlider = null;
                }
            }
        }
        public void Ragdoll()
        {
            animator.enabled = false;
            died = true;
            ragdoll.SetDrag(ragdollDrag);
            ragdoll.SetKinematic(false);
            StopCoroutine(setup.fieldOfView.FindDiedAlly);
            StopCoroutine(setup.fieldOfView.FindTarget);
        }

        public void DropWeapon()
        {
            Transform Weapon = setup.objects.Weapon;
            Weapon.parent = null;
            if(Weapon.TryGetComponent(out MeshCollider collider)) collider.enabled = true;
            if(Weapon.TryGetComponent(out Dragable dragable)) dragable.enabled = true;
            if(Weapon.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.AddExplosionForce(1000,Vector3.down,10);
            }
        }

        public void Hit(Vector3 point)
        {
            hitPoint = point;
            Invoke(nameof(HitInvoke),1);
        }

        public void HitInvoke()
        {
            setup.utility.CallToShotPosition(hitPoint);
        }
    }


}

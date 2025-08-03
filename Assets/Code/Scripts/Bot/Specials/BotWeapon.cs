using System.Collections;
using System.Collections.Generic;
using PlayerRoot;
using UnityEngine;
namespace BotRoot
{
    public class BotWeapon : MonoBehaviour
    {
        public int Damage;
        public int AverageShootCount;
        public float fireRate;
        public float fireRestTime;
        public float accuracy;
        public LayerMask hitableMask;
        [HideInInspector] public int _averageShootCount;
        [HideInInspector] public float _fireRate;
        [HideInInspector] public float _fireRestTime;

        public Transform firePoint;
        public Transform forward;

        [Space] [Header("Bullet Params")]
        public Transform bulletPrefab;
        public float bulletSpeed;
        public float bulletGravity;
        public Transform muzzleFlash;
        public AudioClip shootSound;
        private float randomFireRate;
        [HideInInspector] public BotSetup setup;

        void Start()
        {
            randomFireRate = Random.Range(fireRate - (fireRate / 2), fireRate + (fireRate / 2));
            _fireRestTime = fireRestTime;
        }
        public void Shooting()
        {
            _fireRestTime -= Time.deltaTime;
            if (_fireRestTime <= 0)
            {
                if (Time.time >= _fireRate)
                {
                    _fireRate = Time.time + 1f / randomFireRate;
                    Shoot();
                    _averageShootCount--;
                    if (_averageShootCount <= 0)
                    {
                        _fireRestTime = fireRestTime;
                        randomFireRate = Random.Range(fireRate - (fireRate / 2), fireRate + (fireRate / 2));
                    }
                }
            }
            else
            {
                _averageShootCount = AverageShootCount;
            }
        }
        public void Shoot()
        {
            if (setup.objects.enemy)
            {
                firePoint.LookAt(setup.objects.enemy);
                if(setup.objects.enemy.TryGetComponent(out Player player))
                {
                    if(player.Died)
                    {
                        setup.objects.enemy = null;
                        setup.objects.futureEnemy = null;
                        return;
                    }
                }
            }
            else
            {
                firePoint.eulerAngles = Vector3.zero;
            }

            setup.source.PlayOneShot(shootSound, 0.7f);
            Transform _muzzleFlash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
            Destroy(_muzzleFlash.gameObject, 0.05f);

            RaycastHit hit;
            float crosshairSize = accuracy * 0.001f;
            float randomX = Random.Range(-crosshairSize, crosshairSize);
            float randomY = Random.Range(-crosshairSize, crosshairSize);
            float randomZ = Random.Range(-crosshairSize, crosshairSize);
            Vector3 offset = new Vector3(randomX, randomY, randomZ);
            Ray ray = new(firePoint.position, firePoint.forward + offset);

            if (Physics.Raycast(ray, out hit, 200, hitableMask))
            {
                firePoint.LookAt(hit.point);
            }
            else
            {
                firePoint.LookAt(forward.position + offset * 150);
            }
            Transform bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            BotBullet parabolicBullet = bullet.GetComponent<BotBullet>();
            parabolicBullet.Initialize(firePoint, bulletSpeed, bulletGravity, Damage, setup);

            // bool random = Random.value > 0.6f;
            // if (setup.objects.enemy && random)
            // {
            //     if (setup.objects.enemy.TryGetComponent(out Player player))
            //     {
            //         bool stimulationRandom = Random.value > 0.7f;
            //         if (stimulationRandom) player.holster.currentWeapon.recoil.Stimulation();
            //         player.TakeDamage(Damage, setup.transform);
            //         player.damageNavigatorRegister.CreateIndicator(setup.transform, player.transform);
            //     }
            // }
            // else
            // {
            //     if (setup.objects.enemy.TryGetComponent(out Player player))
            //     {
            //         player.PlaySplitRandom(player.SwooshSound, 0.3f, 7, player.footstepAudioSource);
            //     }
            // }
        }
    }


}

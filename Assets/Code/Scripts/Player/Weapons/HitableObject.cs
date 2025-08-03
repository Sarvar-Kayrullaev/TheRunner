using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BotRoot;
using PlayerRoot;
//[ExecuteInEditMode]
public class HitableObject : MonoBehaviour
{
    public HitableType type;
    public HumanBody humanBody;
    [Header("Attributes")]
    public int MaxHealth = 100;
    public int armor = 5;
    [HideInInspector] public bool died = false;
    [Space]
    [Header("Objects")]
    [SerializeField] HitMarker hitMarker;
    [SerializeField] Transform hole;
    public Transform hitParticle;
    private int currentHealth;
    public BotRoot.Health health;
    public PlayerBody playerBody;
    public CarController carController;

    [HideInInspector] public float power;
    [HideInInspector] public float radius;
    [HideInInspector] public Vector3 forceDirection;

    public AudioClip hitBulletSound;

    private void Start()
    {
        //Resurrection();
        // if(type == HitableType.Human)
        // {
        //     if(!gameObject.TryGetComponent(out Body body)) 
        //     {
        //         gameObject.AddComponent<Body>();
        //     }
        // }
    }
    public void HitVisualize(Vector3 hitPosition, Vector3 hitNormal)
    {
        if (hole)
        {
            GameObject _hole = Instantiate(hole, hitPosition + (hitNormal * 0.025f), Quaternion.identity, transform).gameObject;
            _hole.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hitNormal);
            Destroy(_hole, 5);
        }
        if (hitParticle) Instantiate(hitParticle, hitPosition + (hitNormal * 0.025f), Quaternion.identity, transform);
    }
    public void HitBullet(int damage, Transform bulletOwner)
    {
        if (type == HitableType.Damageable) Live(damage);
        if (type == HitableType.Human) Human(damage, bulletOwner);
        if (type == HitableType.Player) Player(damage, bulletOwner);
        if (type == HitableType.Vehicle) Vehicle(damage, bulletOwner);
    }

    void Live(int damage)
    {
        int previowHealth = currentHealth - damage;
        if (previowHealth > 0)
        {
            currentHealth -= damage;
            if(health) health.setup.global.HitMarker.Hit();
        }
        else
        {
            if (died == false)
            {
                Die();
            }
        }
    }

    void Human(int damage, Transform bulletOwner)
    {
        damage = HumanDamage(damage);
        health.Hit(bulletOwner.position);
        int previowHealth = health.currentHealth - damage;
        if (previowHealth > 0)
        {
            health.currentHealth -= damage;
            if (!health.setup.memory.hitFirstBullet)
            {
                health.setup.botAudio.Play(PanicTalking.HIT_BODY, health.setup);
                health.setup.memory.hitFirstBullet = true;
            }
            if(health) health.setup.global.HitMarker.Hit();
        }
        else
        {
            if (health.died == false)
            {
                health.currentHealth = 0;
                health.Ragdoll();
                health.setup.overall.RelistBot();
                health.setup.memory.whoKilled = bulletOwner;
                health.setup.memory.timeToDied = Time.time;
                health.setup.DisableAllComponents();
                if(health) health.setup.global.HitMarker.Died();

            }
            Invoke(nameof(AddForce),0.01f);
        }
    }

    void Player(int damage, Transform bulletOwner)
    {
        if (playerBody.body == PlayerBodyName.Head)
        {
            playerBody.player.TakeDamage((int)(damage * 1.5f), bulletOwner);
        }
        else
        {
            playerBody.player.TakeDamage(damage, bulletOwner);
        }
    }

    void Vehicle(int damage, Transform bulletOwner)
    {
        if (carController.activate)
        {
            bool randomHit = Random.Range(0, 3) == 1;
            if (randomHit)
            {
                carController.player.TakeDamage(damage, bulletOwner);
            }
            else
            {
                //HitCar
            }
        }
    }

    public void Die()
    {
        if (health) health.setup.global.HitMarker.Died();
        GetComponent<Renderer>().enabled = false;
        Invoke("Resurrection", 3);
        died = true;
    }
    public void Resurrection()
    {
        died = false;
        GetComponent<Renderer>().enabled = true;
        currentHealth = MaxHealth;
    }

    int HumanDamage(int damage)
    {
        switch (humanBody)
        {
            case HumanBody.Head: return damage * 10;
            case HumanBody.Hip: return damage * 2;
            case HumanBody.Spine: return damage * 3;
            case HumanBody.upLeg: return damage;
            case HumanBody.leg: return damage / 2;
            case HumanBody.arm: return damage;
            case HumanBody.foreArm: return damage / 2;
            default: return damage;
        }
    }

    void AddForce()
    {
        if(humanBody == HumanBody.upLeg || humanBody == HumanBody.leg)
        {
            power /= 6;
        }
        else if(humanBody == HumanBody.arm || humanBody == HumanBody.foreArm)
        {
            power /= 12;
        }
        else if(humanBody == HumanBody.Head)
        {
            power /= 1;
        }
        power = humanBody == HumanBody.Head ? power/3 : power;
        Vector3 explosionPos = forceDirection;
        if(transform.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.AddExplosionForce(power, explosionPos, radius);
        }
    }
}
public enum HitableType
{
    Other,
    Metal,
    Wood,
    Water,
    Live,
    Human,
    Damageable,
    Player,
    Vehicle
}

public enum HumanBody
{
    Head,
    Hip,
    Spine,
    upLeg,
    leg,
    arm,
    foreArm
}

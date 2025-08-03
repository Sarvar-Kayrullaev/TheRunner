using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrowWeapon : MonoBehaviour
{
    Animator animator;
    [HideInInspector]public WeaponHolster holster;
    public Transform rockPrefab;
    public Transform throwPoint;
    public float throwSpeed;
    public float gravityForce;

    [SerializeField] private float ThrowTime = 0.5f;
    private bool throwed = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        ThrowTime -= Time.deltaTime;
        if (ThrowTime <= 0 && !throwed)
        {
            throwed = true;
            ThrowToRay();
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            holster.RedrawWeapon();
            Destroy(gameObject);
        }
    }

    void ThrowToRay()
    {
        Transform bullet = Instantiate(rockPrefab, throwPoint.position, throwPoint.rotation);
        ParabolicRock parabolicRock = bullet.GetComponent<ParabolicRock>();
        parabolicRock.Initialize(throwPoint, throwSpeed, gravityForce);
    }
}

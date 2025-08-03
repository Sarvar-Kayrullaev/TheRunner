using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletVisual : MonoBehaviour
{
    [SerializeField] float scaleSpeed;
    [SerializeField] float maxSize;
    void Update()
    {
        float scale = transform.localScale.x + Time.deltaTime * scaleSpeed;
        if (scale > maxSize) return;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}

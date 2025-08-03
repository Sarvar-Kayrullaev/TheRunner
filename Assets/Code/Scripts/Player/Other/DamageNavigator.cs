using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNavigator : MonoBehaviour
{
    public Image image;
    public float fadeSpeed;
    private Transform owner;
    private Transform target;
    private bool registered = false;

    private Color startColor;
    private float fade = 1;
    private RectTransform rect;
    private Quaternion rotation = Quaternion.identity;
    private Vector3 position = Vector3.zero;

    public void Register(Transform owner, Transform target)
    {
        this.owner = owner;
        this.target = target;

        startColor = image.color;
        fade = 1;
        registered = true;
        RotateToTheTarget(owner, target);
    }

    void Update()
    {
        if (registered)
        {
            if (fade <= 0)
            {
                Destroy(gameObject);
                return;
            }

            fade = Mathf.Lerp(fade, 0, fadeSpeed);
            image.color = new(startColor.r, startColor.g, startColor.b, fade);
            RotateToTheTarget(owner, target);
        }

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
}

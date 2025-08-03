using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ObjectiveIndicator : MonoBehaviour
{
    [Header("Setup")]
    public Transform target;
    public Vector3 offset = Vector3.zero;
    [SerializeField] private float pointSize = 50;
    [SerializeField] private float padding = 25;

    [Space]
    public IndicatorType type;

    [Header("Visual Objects")]
    [SerializeField] private GameObject visualPointPrefab;
    [SerializeField] private GameObject visualArrowPrefab;
    [SerializeField] private GameObject visualDotPrefab;


    private Camera mainCamera;
    private IndicatorManager indicatorManager;
    private RectTransform rect;
    private RectTransform pointRect;
    private RectTransform arrowRect;
    private RectTransform dotRect;
    private Vector2 position;
    private Vector2 anchoredPosition;

    public void Initialize(Transform target, IndicatorManager indicatorManager)
    {
        this.target = target;
        this.indicatorManager = indicatorManager;
        mainCamera = Camera.main;
        rect = transform as RectTransform;

        GeneratePoint();
    }

    public void ResetCamera(Camera camera)
    {
        mainCamera = camera;
    }

    private void Update()
    {
        position = mainCamera.WorldToScreenPoint(target.position + offset);
        anchoredPosition = position - new Vector2(Screen.width / 2, Screen.height / 2);
        float dot = Vector3.Dot(target.position - mainCamera.transform.position, mainCamera.transform.forward);

        if (type == IndicatorType.EdgeArrow)
        {
            dotRect.gameObject.SetActive(false);
            if (IsTargetInScreen(Screen.width, Screen.height, padding, position) && dot > 0)
            {
                transform.position = position;
                pointRect.anchoredPosition = Vector2.zero;
                pointRect.eulerAngles = Vector3.zero;
                arrowRect.gameObject.SetActive(false);
            }
            else
            {
                rect.anchoredPosition = Vector2.zero;
                Vector2 direction = anchoredPosition * (dot < 0 ? -1f : 1f) - Vector2.zero;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rect.eulerAngles = new(0, 0, angle);

                RectTransform parent = transform.parent as RectTransform;
                float parentWidth = parent.rect.size.x;
                float parentHeight = parent.rect.size.y;
                float distance = CalculateDistanceFromCenterToEdge(parentWidth - padding * 2, parentHeight - padding * 2, angle);
                pointRect.anchoredPosition = new(distance, 0);
                pointRect.eulerAngles = Vector3.zero - rect.eulerAngles;

                arrowRect.gameObject.SetActive(true);
                arrowRect.anchoredPosition = new(distance, 0);
                arrowRect.eulerAngles = new(0, 0, angle);
            }
        }
        else if (type == IndicatorType.EdgeDot)
        {
            arrowRect.gameObject.SetActive(false);
            if (IsTargetInScreen(Screen.width, Screen.height, padding, position) && dot > 0)
            {
                transform.position = position;
                pointRect.anchoredPosition = Vector2.zero;
                pointRect.eulerAngles = Vector3.zero;
                pointRect.gameObject.SetActive(true);
                dotRect.gameObject.SetActive(false);
            }
            else
            {
                rect.anchoredPosition = Vector2.zero;
                Vector2 direction = anchoredPosition * (dot < 0 ? -1f : 1f) - Vector2.zero;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rect.eulerAngles = new(0, 0, angle);

                RectTransform parent = transform.parent as RectTransform;
                float parentWidth = parent.rect.size.x;
                float parentHeight = parent.rect.size.y;
                float distance = CalculateDistanceFromCenterToEdge(parentWidth - padding * 2, parentHeight - padding * 2, angle);

                pointRect.anchoredPosition = new(distance, 0);
                pointRect.gameObject.SetActive(false);

                dotRect.gameObject.SetActive(true);
                dotRect.anchoredPosition = new(distance, 0);
                dotRect.eulerAngles = new(0, 0, angle);
            }
        }

    }

    private void GeneratePoint()
    {
        GameObject point1 = Instantiate(visualPointPrefab, transform);
        pointRect = point1.GetComponent<RectTransform>();
        pointRect.anchorMin = new(0.5f, 0.5f);
        pointRect.anchorMax = new(0.5f, 0.5f);
        pointRect.pivot = new(0.5f, 0.5f);
        pointRect.sizeDelta = new(pointSize, pointSize);
        pointRect.anchoredPosition = new Vector2(0f, 0f);
        pointRect.localScale = new(1, 1, 1);

        GameObject point2 = Instantiate(visualArrowPrefab, transform);
        arrowRect = point2.GetComponent<RectTransform>();
        arrowRect.anchorMin = new(0.5f, 0.5f);
        arrowRect.anchorMax = new(0.5f, 0.5f);
        arrowRect.pivot = new(0.5f, 0.5f);
        arrowRect.sizeDelta = new(pointSize, pointSize);
        arrowRect.anchoredPosition = new Vector2(0f, 0f);
        arrowRect.localScale = new(1, 1, 1);

        GameObject point3 = Instantiate(visualDotPrefab, transform);
        dotRect = point3.GetComponent<RectTransform>();
        dotRect.anchorMin = new(0.5f, 0.5f);
        dotRect.anchorMax = new(0.5f, 0.5f);
        dotRect.pivot = new(0.5f, 0.5f);
        dotRect.sizeDelta = new(pointSize, pointSize);
        dotRect.anchoredPosition = new Vector2(0f, 0f);
        dotRect.localScale = new(1, 1, 1);
    }

    public float CalculateDistanceFromCenterToEdge(float screenWidth, float screenHeight, float angleInCenterDegrees)
    {
        float angleInCenterRadians = angleInCenterDegrees * MathF.PI / 180.0f;

        float centerX = screenWidth / 2.0f;
        float centerY = screenHeight / 2.0f;

        float slope = MathF.Tan(angleInCenterRadians);

        float distance;

        if (MathF.Abs(slope) > centerY / centerX)
        {
            float y = (slope > 0) ? screenHeight / 2.0f : -screenHeight / 2.0f;
            float x = y / slope;
            distance = MathF.Sqrt(x * x + y * y);
        }
        else
        {
            float x = (angleInCenterDegrees > 90 || angleInCenterDegrees < -90) ? -screenWidth / 2.0f : screenWidth / 2.0f;
            float y = slope * x;
            distance = MathF.Sqrt(x * x + y * y);
        }

        return distance;
    }

    public static bool IsTargetInScreen(float screenWidth, float screenHeight, float padding, Vector2 target)
    {
        return target.x >= padding && target.x <= screenWidth - padding && target.y >= padding && target.y <= screenHeight - padding;
    }

    public enum IndicatorType
    {
        FreeScreen,
        EdgeArrow,
        EdgeDot,
        Edge
    }
}

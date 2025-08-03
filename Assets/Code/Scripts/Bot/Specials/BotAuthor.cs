using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

namespace BotRoot
{
    public class BotAuthor : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;

        [Header("Marker")]
        public bool Debug = false;
        public Sprite markerSprite;
        public Sprite wonderSprite;
        public Sprite alarmSprite;

        public Color wonderSignalColor;
        public Color alarmSignalColor;

        public float heightOffset;
        public RectTransform markerPrefab;
        public RectTransform DebugMarkerPrefab;
        public Transform markerPoint;
        public bool marked = false;
        private TMP_Text message;
        public MarkerPoint marker;
        
        
        [Space]
        [Header("Bot Bodies")]
        public Transform bodiesParent;
        public List<Body> bodies = new();

        private GameObject markerObject;

        void Start()
        {
            //SetMark(Camera.main);
            bodies.Clear();
            UpdateBodies(bodiesParent);
        }

        void UpdateBodies(Transform parent)
        {
            foreach(Transform child in parent)
            {
                if(child.TryGetComponent(out Body body))
                {
                    body.setup = setup;
                    bodies.Add(body);
                }
                UpdateBodies(child);
            }
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.M))
            {
                if (marked)
                {
                    Debug = !Debug;
                    RemoveMarker();
                    SetMark(Camera.main);
                }
                else
                {
                    SetMark(Camera.main);
                }
            }
        }

        public void SetMark(Camera camera)
        {
            if (setup.health.died) return;
            marked = true;
            Vector2 pos = camera.WorldToScreenPoint(markerPoint.position);

            if (Debug)
            {
                RectTransform createdMarker = Instantiate(DebugMarkerPrefab, pos, Quaternion.identity, setup.global.MarkerParent);

                if (createdMarker.TryGetComponent(out MarkerPoint screenPoint))
                {
                    marker = screenPoint;
                    screenPoint.target = markerPoint;
                    screenPoint.Initialize(markerSprite);
                    message = createdMarker.GetChild(1).GetComponent<TMP_Text>();
                }
                markerObject = createdMarker.gameObject;
            }
            else
            {
                RectTransform createdMarker = Instantiate(markerPrefab, pos, Quaternion.identity, setup.global.MarkerParent);

                if (createdMarker.TryGetComponent(out MarkerPoint screenPoint))
                {
                    marker = screenPoint;
                    screenPoint.target = markerPoint;
                    screenPoint.Initialize(markerSprite);
                }
                markerObject = createdMarker.gameObject;
            }
        }

        public void SetAlarmSignal(bool signal)
        {
            if (marker)
            {
                if(signal)
                {
                    marker.SetSignal(alarmSprite, alarmSignalColor, 1, 5, 3);
                }
                else
                {
                    marker.SetSignal(alarmSprite, alarmSignalColor, 0, 0, 0);
                }
            }
        }

        public void SetWonderSignal(bool signal)
        {
            if (marker)
            {
                if(signal)
                {
                    marker.SetSignal(wonderSprite, wonderSignalColor, 0, 2, 2);
                }
                else
                {
                    marker.SetSignal(wonderSprite, wonderSignalColor, 0, 0, 0);
                }
            }
        }

        public void Message(string value)
        {
            if (message)
            {
                message.text = value;
            }
        }

        // public void Message(string value, int fontSize)
        // {
        //     if(message)
        //     {
        //         message.text = value;
        //         message.fontSize = fontSize;
        //     }
        // }

        public void RemoveMarker()
        {
            if (markerObject) Destroy(markerObject);
        }
    }
}


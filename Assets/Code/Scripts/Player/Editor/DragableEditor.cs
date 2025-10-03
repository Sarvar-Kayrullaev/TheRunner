using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BotRoot;
using UnityEditor.UIElements;

namespace PlayerRoot
{
    [CustomEditor(typeof(Dragable))]
    public class DragableEditor : Editor
    {
        private Dragable _dragable;
        private DragableType _dragableType;

        void OnEnable()
        {
            _dragable = (Dragable)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dragableType"));
            _dragableType = _dragable.dragableType;
            
            switch (_dragableType)
            {
                case DragableType.Item:
                {
                    ItemField();
                    break;
                }
                case DragableType.Weapon:
                {
                    WeaponField();
                    break;
                }
                case DragableType.Bullet:
                {
                    BulletField();
                    break;
                }
                case DragableType.Box:
                {
                    BoxField();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ItemField()
        {
            EditorGUILayout.LabelField("Dragable Params", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("listenerDistance"));
            serializedObject.ApplyModifiedProperties();
        }

        private void WeaponField()
        {
            EditorGUILayout.LabelField("Weapon Fields", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAmmoSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("suppressorModel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sightModel"));

            EditorGUILayout.LabelField("Dragable Params", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("listenerDistance"));
            serializedObject.ApplyModifiedProperties();
        }

        private void BulletField()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isRefillable"));

            EditorGUILayout.LabelField("Dragable Params", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("listenerDistance"));
            serializedObject.ApplyModifiedProperties();
        }

        private void BoxField()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Dragable Params", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("listenerDistance"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
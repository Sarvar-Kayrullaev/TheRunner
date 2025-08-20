using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Data
{
    [CustomEditor(typeof(StartData))]
    public class StartDataEditor : Editor
    {
        private string[] tabs = { "OpenWorld", "Player", "Settings" };
        private int tabIndex = 0;
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            tabIndex = GUILayout.Toolbar(tabIndex, tabs);
            EditorGUILayout.EndVertical();

            switch (tabIndex)
            {
                case 0:
                    //base.OnInspectorGUI();
                    OpenWorld();
                    break;
                case 1:
                    Player();
                    break;
                case 2:
                    Settings();
                    break;
                default:
                    All();
                    break;
            }
        }

        void OpenWorld()
        {
            EditorGUILayout.Space();
            StartData inspector = (StartData)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("OutpostParent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OpenWorldData"));

            serializedObject.ApplyModifiedProperties();
        }
        void Player()
        {

            EditorGUILayout.Space();
            StartData inspector = (StartData)target;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CreatePlayerData"));

            if(inspector.CreatePlayerData)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerData"));
            }

            serializedObject.ApplyModifiedProperties();
        }
        void Settings()
        {

        }
        void All()
        {

        }
    }
}

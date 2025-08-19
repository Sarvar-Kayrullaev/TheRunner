using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Data
{
    [CustomEditor(typeof(SealedData))]
    public class SealedDataEditor : Editor
    {
        private readonly string[] tabs = { "Basic", "Levelable" };
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
                    Basic();
                    break;
                case 1:
                    Levelable();
                    break;
                default:
                    All();
                    break;
            }
        }

        void Basic()
        {
            EditorGUILayout.Space();
            SealedData inspector = (SealedData)target;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("WeaponBasics"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Sights"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Suppressors"));
            serializedObject.ApplyModifiedProperties();
        }

        void Levelable()
        {

            EditorGUILayout.Space();
            SealedData inspector = (SealedData)target;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BulletBag"));
            serializedObject.ApplyModifiedProperties();
        }

        void All()
        {

        }
    }
}

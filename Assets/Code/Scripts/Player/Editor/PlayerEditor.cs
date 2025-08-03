using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace PlayerRoot
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : Editor
    {
        private string[] tabs = { "Main", "Control", "Sound", "Setup" };
        private int tabIndex = 0;
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            tabIndex = GUILayout.Toolbar(tabIndex, tabs);
            EditorGUILayout.EndVertical();

            switch (tabIndex)
            {
                case 0:
                base.OnInspectorGUI();
                    MainTab();
                    break;
                case 1:
                    ControlTab();
                    break;
                case 2:
                    SoundTab();
                    break;
                case 3:
                    SetupTab();
                    break;
                default:
                    All();
                    break;
            }
        }

        void MainTab()
        {

        }
        void ControlTab()
        {

        }
        void SoundTab()
        {

        }
        void SetupTab()
        {

        }

        void All()
        {

        }
    }
}

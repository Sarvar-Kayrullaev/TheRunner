using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BotRoot;

namespace MissionManager
{
    [CustomEditor(typeof(MissionSequence))]
    public class MissionSequenceEditor : Editor
    {
        private readonly string[] tabs = { "Sequences", "Options","Setup" };
        private int tabIndex = 0;
        private MissionSequence inspector;
        List<bool> collapseList;

        void OnEnable()
        {
            inspector = (MissionSequence)target;
            if (collapseList == null)
            {
                collapseList = new();
                for (int i = 0; i < inspector.missions.Count; i++)
                {
                    collapseList.Add(false);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical();
            tabIndex = GUILayout.Toolbar(tabIndex, tabs);
            EditorGUILayout.EndVertical();

            switch (tabIndex)
            {
                case 0:
                    //base.OnInspectorGUI();
                    Sequences();
                    break;
                case 1:
                    Options();
                    break;
                case 2:
                    Setup();
                    break;
                default:
                    All();
                    break;
            }
        }

        void Sequences()
        {
            EditorGUILayout.Space();

            int i = 0;
            foreach (MissionModel model in inspector.missions)
            {
                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();
                GUIStyle foldoutStyle = new(EditorStyles.label);
                foldoutStyle.fontStyle = UnityEngine.FontStyle.Bold;
                foldoutStyle.padding = new(10,0,0,0);

                EditorGUILayout.LabelField("Mission: " + i, foldoutStyle);
                if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    collapseList[i] = !collapseList[i];
                    Event.current.Use();
                }

                Texture2D upIcon = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/Art/Sprites/Inspector/Editor_Up.png");
                if (upIcon != null)
                {
                    GUIContent buttonContent = new(upIcon);
                    if (i > 0 && GUILayout.Button(buttonContent, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        MoveItem(inspector.missions, i, i - 1);
                        EditorUtility.SetDirty(inspector);
                    }
                }

                Texture2D downIcon = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/Art/Sprites/Inspector/Editor_Down.png");
                if (downIcon != null)
                {
                    GUIContent buttonContent = new(downIcon);
                    if (i < inspector.missions.Count - 1 && GUILayout.Button(buttonContent, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        MoveItem(inspector.missions, i, i + 1);
                        EditorUtility.SetDirty(inspector);
                    }
                }

                Texture2D removeIcon = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/Art/Sprites/Inspector/Editor_Remove.png");
                if (removeIcon != null)
                {
                    GUIContent buttonContent = new(removeIcon);
                    if (GUILayout.Button(buttonContent, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        inspector.missions.RemoveAt(i);
                        collapseList.RemoveAt(i);
                        EditorUtility.SetDirty(inspector);
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (!collapseList[i])
                {
                    i++;
                    EditorGUILayout.EndVertical();
                    continue;
                }

                GUIStyle background = new();
                Texture2D texture = new(1, 1);
                texture.SetPixel(0, 0, new Color(0, 0, 0, 0.2f));
                texture.Apply();
                background.normal.background = texture;
                background.padding = new RectOffset(20, 5, 5, 5);

                EditorGUILayout.BeginVertical(background);
                model.missionType = (MissionType)EditorGUILayout.EnumPopup("Mission Type", model.missionType);
                model.point = (Transform)EditorGUILayout.ObjectField("Point", model.point, typeof(Transform), true);
                model.title = EditorGUILayout.TextField("Title", model.title);
                model.description = EditorGUILayout.TextField("Description", model.description);

                switch (model.missionType)
                {
                    case MissionType.MovePoint: MovePointPanel(model);
                        break;
                    case MissionType.MovePointStealth: MovePointPanel(model);
                        break;
                    case MissionType.DestroyPoint:
                        DestroyPointPanel(model);
                        break;
                    case MissionType.DestroyPointStealth:
                        DestroyPointStealhtPanel(model);
                        break;
                    case MissionType.DestroyCommander:
                        break;
                    default:
                        break;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                i++;
            }
            if (GUILayout.Button("Add Mission"))
            {
                inspector.missions.Add(new MissionModel());
                EditorUtility.SetDirty(inspector);
                collapseList.Add(false);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void MoveItem<T>(List<T> list, int from, int to)
        {
            T item = list[from];
            list.RemoveAt(from);
            list.Insert(to, item);
        }

        void Options()
        {

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("FailedMissionDelay"));
            serializedObject.ApplyModifiedProperties();
        }

        void Setup()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Indicator Prefabs");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IndicatorParent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IndicatorPrefab"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mission Text Parents");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CompleteParent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FailedParent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TitleParent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ContextParent"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Other");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Player"));
            serializedObject.ApplyModifiedProperties();
        }

        void All()
        {

        }

        /// Model Properties

        void MovePointPanel(MissionModel model)
        {
            model.triggerDistance = EditorGUILayout.FloatField("Trigger Distance", model.triggerDistance);
        }

        void DestroyPointPanel(MissionModel model)
        {
            EditorGUILayout.Space(10);
            GUIStyle background = new();
            Texture2D texture = new(1, 1);
            texture.SetPixel(0, 0, new Color(0, 0, 0, 0.15f));
            texture.Apply();
            background.normal.background = texture;
            background.padding = new RectOffset(10, 5, 5, 5);
            EditorGUILayout.BeginVertical(background);

            model.targets ??= new ();
            GUIStyle boldLabelStyle = new(EditorStyles.label);
            boldLabelStyle.fontStyle = UnityEngine.FontStyle.Bold;
            EditorGUILayout.LabelField("Targets");
            for (int j = 0; j < model.targets.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                model.targets[j] = (BotSpawner)EditorGUILayout.ObjectField(model.targets[j], typeof(BotSpawner), true);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    model.targets.RemoveAt(j);
                    EditorUtility.SetDirty(inspector);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                model.targets.Add(null);
                EditorUtility.SetDirty(inspector);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        void DestroyPointStealhtPanel(MissionModel model)
        {
            EditorGUILayout.Space(10);
            GUIStyle background = new();
            Texture2D texture = new(1, 1);
            texture.SetPixel(0, 0, new Color(0, 0, 0, 0.15f));
            texture.Apply();
            background.normal.background = texture;
            background.padding = new RectOffset(10, 5, 5, 5);
            EditorGUILayout.BeginVertical(background);

            model.targets ??= new ();
            GUIStyle boldLabelStyle = new(EditorStyles.label);
            boldLabelStyle.fontStyle = UnityEngine.FontStyle.Bold;
            EditorGUILayout.LabelField("Targets");
            for (int j = 0; j < model.targets.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                model.targets[j] = (BotSpawner)EditorGUILayout.ObjectField(model.targets[j], typeof(BotSpawner), true);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    model.targets.RemoveAt(j);
                    EditorUtility.SetDirty(inspector);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                model.targets.Add(null);
                EditorUtility.SetDirty(inspector);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}


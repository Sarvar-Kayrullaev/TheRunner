using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdvancedChunk))]
public class AdvancedChunkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AdvancedChunk myScript = (AdvancedChunk)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.GenerateChunkRecursive(0, myScript.transform);
        }
        else if (GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }
    }
}

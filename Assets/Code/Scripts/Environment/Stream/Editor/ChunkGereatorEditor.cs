using UnityEditor;
using UnityEngine;
namespace Environment
{
    [CustomEditor(typeof(ChunkGenerator))]
    public class ChunkGereatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ChunkGenerator myScript = (ChunkGenerator)target;
            if (GUILayout.Button("Generate"))
            {
                myScript.Generate();
            }
            else if (GUILayout.Button("Update"))
            {
                myScript.UpdateChunks();
            }
            else if (GUILayout.Button("Clear"))
            {
                myScript.Clear();
            }
            else if(GUILayout.Button("Collect")){
                myScript.Collect();
            }
            else if(GUILayout.Button("Save")){
                if(myScript.chunkMode == ChunkGenerator.ChunkMode.LoadFromAsset)
                {
                    myScript.SaveChunks();
                }else
                {
                    ///Waste
                }
            }
        }
    }
}
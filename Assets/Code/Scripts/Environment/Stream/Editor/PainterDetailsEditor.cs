using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Environment
{
    [CustomEditor(typeof(Painter))]
    public class PainterDetailsEditor : Editor
    {
        void OnSceneGUI()
        {
            Painter obj = (Painter)target;

            Vector3 mousepos = Event.current.mousePosition;

            mousepos = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(mousepos);
            mousepos.y = -mousepos.y;

            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                //Mouse
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.G)
            {
                obj.OnMouseDown();
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.J)
            {
                obj.Undo();
            }
        }
    }
}


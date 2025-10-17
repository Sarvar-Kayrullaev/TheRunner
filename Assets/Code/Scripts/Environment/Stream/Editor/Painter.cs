using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Environment
{
    [ExecuteInEditMode]
    public class Painter : MonoBehaviour
    {
        public bool edit;
        
        [Space][Header("Pen")]
        public float radius = 1;
		public float scaleMin ,scaleMax;
        public int density = 1;
        public bool randomDensity;

        [Space][Header("TerrainMask")]
        public LayerMask drawMask;

        [Space][Header("Setup")]
        public Transform parent;
        public List<GameObject> props;

        private readonly List<GameObject> _cacheProp = new();

        public void OnMouseDown()
        {
            _cacheProp.Clear();
            var mousePos = Event.current.mousePosition;
            mousePos *= EditorGUIUtility.pixelsPerPoint;
            var sceneCamera = SceneView.lastActiveSceneView.camera;
            Vector3 vec = new(mousePos.x, sceneCamera.pixelHeight - mousePos.y, 1);
            var ray = sceneCamera.ScreenPointToRay(vec);
            
            if (!Physics.Raycast(ray, out var hit, 1000, drawMask)) return;
            if (!randomDensity) return;
            
            var point = hit.point;
            var random = Mathf.Clamp(Random.Range(density - density / 2, density + density / 2), 1, 10000);
            for (var i = 0; i < random; i++)
            {
                /*--- PLANE POSITION ---*/
                var randomProp = Random.Range(0, props.Count);
                Vector3 propPosition = new(Random.Range(point.x - radius, point.x + radius), point.y, Random.Range(point.z - radius, point.z + radius));
                Vector3 upPosition = new(propPosition.x, propPosition.y + 10, propPosition.z);

                if (Physics.Raycast(new Ray(upPosition, Vector3.down * 10), out var hit2, 10000, drawMask))
                {
                    Vector3 signedPosition = new(propPosition.x, hit2.point.y, propPosition.z);
                    var eulerAngles = props[randomProp].transform.eulerAngles;
                    eulerAngles.y = Random.Range(0, 360);
                    var propRotation = Quaternion.Euler(eulerAngles);
                    var prop = Instantiate(props[randomProp], signedPosition, propRotation, parent);
                    prop.transform.localScale *= Random.Range(scaleMin, scaleMax);
                    _cacheProp.Add(prop);
                }
                /*-------- END ---------*/
            }
        }

        public void Undo()
        {
            foreach (var cache in _cacheProp)
                DestroyImmediate(cache);
            _cacheProp.Clear();
        }

        public Vector3 MouseHitPoisiton()
        {
            var mousePos = Event.current.mousePosition;
            mousePos *= EditorGUIUtility.pixelsPerPoint;
            var sceneCamera = SceneView.lastActiveSceneView.camera;
            Vector3 vec = new(mousePos.x, sceneCamera.pixelHeight - mousePos.y, 1);
            var ray = sceneCamera.ScreenPointToRay(vec);

            if (Physics.Raycast(ray, out var hit, 1000, drawMask))
            {
                return hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!edit) return;
            var mousePos = Event.current.mousePosition;
            mousePos *= EditorGUIUtility.pixelsPerPoint;
            var camera = SceneView.lastActiveSceneView.camera;
            Vector3 vec = new(mousePos.x, camera.pixelHeight - mousePos.y, 1);
            var ray = camera.ScreenPointToRay(vec);
            if (Physics.Raycast(ray, out var hit, 1000, drawMask))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(hit.point, Vector3.up);
                Handles.color = Color.blue;
                Handles.DrawWireDisc(hit.point, Vector3.up, radius, 3);

                Vector3 upPosition = new(hit.point.x, hit.point.y + 10, hit.point.z);
                Vector3 downPosition = new(hit.point.x, hit.point.y - 10, hit.point.z);
                Handles.color = Color.green;
                Handles.DrawLine(upPosition, downPosition);
            }
        }
#endif
    }

}

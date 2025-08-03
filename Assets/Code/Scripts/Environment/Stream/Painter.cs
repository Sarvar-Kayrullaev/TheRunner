using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace Environment
{
    [ExecuteInEditMode]
    public class Painter : MonoBehaviour
    {
        public bool Edit = false;
        
        [Space][Header("Pen")]
        public float radius = 1;

        [Space][Header("Density")]
        public int Density = 1;
        public bool RandomDensity = false;

        [Space][Header("TerrainMask")]
        public LayerMask DrawMask;

        [Space][Header("Setup")]
        public Transform ChunksParent;
        public List<GameObject> props;

        //Readonly

        private readonly List<GameObject> cacheProp = new();

        public void OnMouseDown()
        {
            cacheProp.Clear();
            Vector2 mousePos = Event.current.mousePosition;
            mousePos *= EditorGUIUtility.pixelsPerPoint;
            var camera = SceneView.lastActiveSceneView.camera;
            Vector3 vec = new(mousePos.x, camera.pixelHeight - mousePos.y, 1);
            Ray ray = camera.ScreenPointToRay(vec);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, DrawMask))
            {
                Vector3 point = hit.point;
                if (RandomDensity)
                {
                    int random = Mathf.Clamp(Random.Range(Density - Density / 2, Density + Density / 2), 1, 10000);

                    for (int i = 0; i < random; i++)
                    {
                        /*--- PLANE POSITION ---*/
                        int randomProp = Random.Range(0, props.Count);
                        Vector3 propPosition = new(Random.Range(point.x - radius, point.x + radius), point.y, Random.Range(point.z - radius, point.z + radius));
                        Vector3 upPosition = new(propPosition.x, propPosition.y + 10, propPosition.z);

                        if (Physics.Raycast(new Ray(upPosition, Vector3.down * 10), out RaycastHit hit2, 10000, DrawMask))
                        {
                            Vector3 signedPosition = new(propPosition.x, hit2.point.y, propPosition.z);
                            Vector3 eulerAngles = props[randomProp].transform.eulerAngles;
                            eulerAngles.y = Random.Range(0, 360);
                            Quaternion propRotation = Quaternion.Euler(eulerAngles);
                            Transform chunk = NearChunk(MouseHitPoisiton());
                            GameObject prop = Instantiate(props[randomProp], signedPosition, propRotation, chunk);
                            cacheProp.Add(prop);
                        }
                        /*-------- END ---------*/
                    }
                }
            }
        }

        public void Undo()
        {
            foreach (GameObject cache in cacheProp)
                DestroyImmediate(cache);
            cacheProp.Clear();
        }

        Transform NearChunk(Vector3 position)
        {
            Transform closestChunk = null;
            float minimumDistance = Mathf.Infinity;

            foreach (Transform chunk in ChunksParent)
            {
                float distance = Vector3.Distance(chunk.transform.position, position);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    closestChunk = chunk;
                }
            }
            return closestChunk;
        }

        public Vector3 MouseHitPoisiton()
        {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos *= EditorGUIUtility.pixelsPerPoint;
            var camera = SceneView.lastActiveSceneView.camera;
            Vector3 vec = new(mousePos.x, camera.pixelHeight - mousePos.y, 1);
            Ray ray = camera.ScreenPointToRay(vec);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000, DrawMask))
            {
                return hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Edit) return;
            Vector2 mousePos = Event.current.mousePosition;
            mousePos *= EditorGUIUtility.pixelsPerPoint;
            var camera = SceneView.lastActiveSceneView.camera;
            Vector3 vec = new(mousePos.x, camera.pixelHeight - mousePos.y, 1);
            Ray ray = camera.ScreenPointToRay(vec);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, DrawMask))
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

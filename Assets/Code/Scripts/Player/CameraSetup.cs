using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CameraSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        Camera.main.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.FrontToBack;
    }
}

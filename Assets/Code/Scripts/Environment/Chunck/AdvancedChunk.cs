using UnityEngine;
using System.Diagnostics; // Required for Stopwatch
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using System;

public class AdvancedChunk : MonoBehaviour
{
    public GameObject ChunkPrefab;
    public Transform Player;
    [Space]
    public int GridSizeX = 4;
    public int GridSizeY = 4;
    [Space]
    public int GridDense = 20;
    [Space]
    public int LayerSize = 2;

    [Space]
    [Header("Optimization")]
    public int CullingDistance = 500; //m


    [HideInInspector] public Transform[] parentChunks;
    [HideInInspector] public Transform[] subdivisionChunks;

    private int layerDepth = 0;

    void Start()
    {
        Collect();
    }

    private int updateFrame = 60;
    private int trackedChunks = 0;

    void Update()
    {
        if (updateFrame > 0)
        {
            updateFrame -= 1;
            return;
        }
        else
        {
            Stopwatch stopwatch = new();
            if (true)
            {
                trackedChunks = 0;
                stopwatch.Start();
            }
            ActivateChunk(transform, 0);

            if (true)
            {
                updateFrame = 60;
                stopwatch.Stop();
                long elapsedMs = stopwatch.ElapsedMilliseconds;
                Debug.Log($"Time: {elapsedMs} ms");
            }

            //
        }
    }

    public void ActivateChunk(Transform parent, int depth)
    {
        if(depth >= LayerSize) return;
        foreach (Transform chunk in parent)
        {
            trackedChunks++;
            layerDepth = CountParentsBeforeTarget(chunk, transform);
            Vector3 playerPos = new(Player.position.x, 0, Player.position.z);
            Vector3 chunkPos = new(chunk.position.x, 0, chunk.position.z);
            int minSize = Mathf.Min(GridSizeX, GridSizeY);
            float dense = (GridDense / Mathf.Pow(minSize, layerDepth + 1)) / 2;
            float distanceToCorner = Mathf.Sqrt((dense * dense) + (dense * dense));
            float distance = Vector3.Distance(playerPos, chunkPos);

            if (distance > CullingDistance + distanceToCorner)
            {
                Debug.Log(layerDepth + " >= " + LayerSize + "     :" + chunk.name + "  = Disabled");
                chunk.gameObject.SetActive(false);
            }
            else
            {
                chunk.gameObject.SetActive(true);

                if (layerDepth >= LayerSize)
                {
                    Debug.Log(layerDepth + " >= " + LayerSize + "     :" + chunk.name);
                    return; //Leaf
                }
                else
                {
                    ActivateChunk(chunk, layerDepth+1);
                }
            }
        }

        if (parent.GetInstanceID() == transform.GetInstanceID())
        {
            Debug.Log("TRACKED: " + trackedChunks);
        }
    }

    public void Collect()
    {
        List<GameObject> elements = GetGameObjectsInLayer("Environments");
        foreach (GameObject element in elements)
        {
            Transform leafChunk = GetClosestChunk(element.transform, transform);
            if (leafChunk)
            {
                element.transform.parent = leafChunk;
            }
            else
            {
                
            }
        }
    }

    public Transform GetClosestChunk(Transform target, Transform parenta)
    {
        float closestChunkDistance = Mathf.Infinity;
        Transform closestChunk = null;
        int cycles = 0;
        foreach (Transform chunk in parenta)
        {
            cycles++;
            float distance = Vector3.Distance(target.position, chunk.position);
            if (distance < closestChunkDistance)
            {
                closestChunk = chunk;
                closestChunkDistance = distance;
            }
        }

        if (closestChunk)
        {
            int currentLayer = CountParentsBeforeTarget(closestChunk, transform);
            if (currentLayer == LayerSize-1)
            {
                return closestChunk;
            }
            else
            {
                Debug.Log("current Layer: " + currentLayer);
                return GetClosestChunk(target, closestChunk);
            }
        }
        else
        {
            Debug.LogError("Null Returned: " + cycles);
            return null;
        }
    }

    public void Clear()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (transform.GetChild(0)) DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void GenerateChunkRecursive(int currentLayer, Transform parent)
    {
        if (currentLayer >= LayerSize)
        {
            //Populate Leaf
            return;
        }
        else if (currentLayer == 0)
        {
            // Parental Chunks
            parentChunks = new Transform[GridSizeX * GridSizeY];
            int chunkCount = 0;
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    GameObject chunk = Instantiate(ChunkPrefab, parent);
                    int minSize = Mathf.Min(GridSizeX, GridSizeY);
                    float dense = GridDense / Mathf.Pow(minSize, currentLayer + 1);
                    chunk.transform.localPosition = new Vector3((x * dense) + (dense / 2), 0, (y * dense) + (dense / 2));
                    parentChunks[chunkCount] = chunk.transform;
                    chunkCount++;
                    GenerateChunkRecursive(currentLayer + 1, chunk.transform);
                }
            }
        }
        else
        {
            // Subdivision Chunks
            int subdivisionSize = Mathf.Min(GridSizeX, GridSizeY);
            subdivisionChunks = new Transform[subdivisionSize * subdivisionSize];
            int chunkCount = 0;
            for (int x = 0; x < subdivisionSize; x++)
            {
                for (int y = 0; y < subdivisionSize; y++)
                {
                    GameObject chunk = Instantiate(ChunkPrefab, parent);
                    float dense = GridDense / Mathf.Pow(subdivisionSize, currentLayer + 1);
                    float offsetValue = ((dense * subdivisionSize) - dense) / 2;
                    Vector3 position = new((x * dense) - offsetValue, 0, (y * dense) - offsetValue);
                    chunk.transform.localPosition = position;
                    subdivisionChunks[chunkCount] = chunk.transform;
                    chunkCount++;
                    GenerateChunkRecursive(currentLayer + 1, chunk.transform);
                }
            }
        }
    }

    public int CountParentsBeforeTarget(Transform child, Transform targetParent)
    {
        if (child == null)
        {
            Debug.LogWarning("Child Transform is null.");
            return 0;
        }

        if (child == targetParent)
        {
            return 0;
        }

        int count = 0;
        Transform currentParent = child.parent;

        // Traverse up the hierarchy
        while (currentParent != null && currentParent != targetParent)
        {
            count++;
            currentParent = currentParent.parent;
        }

        // Check if we reached the targetParent or the root without finding it
        if (currentParent == targetParent)
        {
            return count;
        }
        else
        {
            // currentParent is null, meaning we reached the root without finding targetParent
            // This indicates targetParent is not an ancestor of child
            return -1;
        }
    }

    public List<GameObject> GetGameObjectsInLayer(string layerName)
    {
        List<GameObject> objectsInLayer = new List<GameObject>();
        int layer = LayerMask.NameToLayer(layerName);

        if (layer == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' does not exist. Please check your Tags & Layers settings.");
            return objectsInLayer; // Return empty list if layer doesn't exist
        }

        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);// Finds all active GameObjects in the scene

        foreach (GameObject obj in allGameObjects)
        {
            if (obj.layer == layer)
            {
                objectsInLayer.Add(obj);
            }
        }

        return objectsInLayer;
    }

}
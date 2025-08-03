using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class ChunkGenerator : MonoBehaviour
    {
        public GameObject ChunkPrefab;
        public int ChunkSize = 16;
        public int ChunkDense = 30;
        public ChunkMode chunkMode;

        [Space]
        [Header("Collect Properties")]
        public LayerMask PropsMask;
        int ChunkNumberX;
        int ChunkNumberY;

        [HideInInspector] public Transform[] chunks;

        void Update()
        {
            if(Input.GetKeyUp(KeyCode.C))
            {
                Collect();
            }
            if(Input.GetKeyUp(KeyCode.J))
            {
                SaveChunks();
            }
        }

        public void SaveChunks()
        {
            foreach (Transform chunkTransform in transform)
            {
                Chunk chunk = chunkTransform.GetComponent<Chunk>();
                chunk.SaveChunk();
            }
        }

        public void Collect()
        {
            foreach (Transform chunkTransform in transform)
            {
                Chunk chunk = chunkTransform.GetComponent<Chunk>();
                chunk.Collect(ChunkDense+5, PropsMask);
            }
        }

        public void Generate()
        {
            Clear();
            ChunkNumberX = ChunkSize;
            ChunkNumberY = ChunkSize;
            chunks = new Transform[ChunkNumberY * ChunkNumberX];

            int chunkCount = 0;
            for (int y = 0; y < ChunkNumberX; y++)
            {
                for (int x = 0; x < ChunkNumberY; x++)
                {
                    GameObject chunk = Instantiate(ChunkPrefab, transform);
                    chunk.transform.localPosition = new Vector3(x * ChunkDense, 0, y * ChunkDense);
                    chunks[chunkCount] = chunk.transform;
                    Chunk chunkCode = chunk.GetComponent<Chunk>();
                    chunkCode.index = chunkCount;
                    chunkCode.vector = new Vector2(x, y);
                    chunkCode.Position = chunk.transform.position;
                    chunkCode.PrePlacedChunk = chunkMode == ChunkMode.LoadFromAsset? false : true;
                    chunkCount++;
                }
            }
        }

        public void UpdateChunks()
        {
            foreach (GameObject chunk in transform)
            {
                Chunk chunkCode = chunk.GetComponent<Chunk>();
                chunkCode.PrePlacedChunk = chunkMode == ChunkMode.LoadFromAsset? false : true;
            }
        }

        public void Clear()
        {
            if (chunks == null) return;
            foreach (Transform child in chunks)
            {
                if (child) DestroyImmediate(child.gameObject);
            }
        }

        public enum ChunkMode{
            LoadFromAsset,
            PrePlacedGameObjects
        }
    }
}

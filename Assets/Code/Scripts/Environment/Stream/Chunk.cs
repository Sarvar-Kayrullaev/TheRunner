using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using UnityEditor;
using UnityEngine;

namespace Environment
{
	public class Chunk : MonoBehaviour
	{
		public bool PrePlacedChunk = false;
		public float LoadDistance = 60;
		[HideInInspector] public int index;
		[HideInInspector] public Vector2 vector;
		public Vector3 Position;

		// Readonly 
		private Transform playerCamera;
		private bool isLoaded = false;
		private Material mat;

		void Start()
		{
			if (true)
			{
				playerCamera = Camera.main.transform;
				if (PrePlacedChunk) InvokeRepeating(nameof(UpdateChunks), 0, 2);
				else InvokeRepeating(nameof(Control), 0, 2);

				mat = GetComponent<Renderer>().material;
			}
		}

		public void Control()
		{
			float playerDistance = Vector3.Distance(transform.position, playerCamera.position);
			if (LoadDistance > playerDistance) //Load
			{
				if (!isLoaded)
				{
					StartCoroutine(LoadChunk2());
					isLoaded = true;
				}
			}
			else
			{
				if (transform.childCount > 0)
				{
					DestroyAsync(transform.GetChild(0).gameObject);
					isLoaded = false;
				}
			}
		}

		public void UpdateChunks()
		{
			float playerDistance = Vector3.Distance(transform.position, playerCamera.position);
			if (LoadDistance > playerDistance) //Load
			{
				if (!isLoaded)
				{
					EnableChunk();
					isLoaded = true;
				}
			}
			else
			{
				if (transform.childCount > 0)
				{
					DisableChunk();
					isLoaded = false;
				}
			}
		}
		public void EnableChunk()
		{
			foreach (Transform item in transform)
			{
				item.gameObject.SetActive(true);
			}
		}

		public void DisableChunk()
		{
			foreach (Transform item in transform)
			{
				item.gameObject.SetActive(false);
			}
		}

		public void SaveChunk()
		{
			string prefabName = "chunk_" + index + ".prefab";
			GameObject parent = new GameObject("chunk_" + index);
			parent.transform.position = transform.position;

			List<Transform> props = new();
			foreach (Transform item in transform) props.Add(item);

			foreach (Transform item in props)
			{
				item.parent = parent.transform;
			}

			//PrefabUtility.SaveAsPrefabAsset(parent, "Assets/Resources/Chunks/" + prefabName);
			//DestroyImmediate(gameObject);
		}

		public void LoadChunk()
		{
			string prefabName = "chunk_" + index + ".prefab";
			Queue<GameObject> prefabQueue = new Queue<GameObject>();

			Thread thread = new Thread(() =>
			{
				GameObject prefab = Resources.Load<GameObject>("Chunks/" + prefabName);
				prefabQueue.Enqueue(prefab);
			});
			thread.Start();
			bool isDone = false;
			while (!isDone)
			{
				if (prefabQueue.Count > 0)
				{
					GameObject prefab = prefabQueue.Dequeue();
					// Instantiate the prefab on the main thread
					GameObject instance = Instantiate(prefab, transform.localPosition, transform.rotation, transform);
					isDone = true;
					mat.color = Color.white;
					break;
				}
			}
		}

		IEnumerator LoadChunk2()
		{
			string prefabName = "chunk_" + index;
			//Instantiate(Resources.Load<GameObject>("Chunks/" + prefabName), transform);

			ResourceRequest request = Resources.LoadAsync("Chunks/" + prefabName);

			while (!request.isDone)
			{
				yield return null;
			}

			Instantiate(request.asset, transform.position, transform.rotation, transform);
			isLoaded = true;
		}
		public void DestroyAsync(GameObject gameObject)
		{
			Destroy(gameObject);
		}
		public void Collect(float collectDistance, LayerMask propMask)
		{
			Collider[] propColliders = Physics.OverlapSphere(transform.position, collectDistance, propMask);
			foreach (Collider propCollider in propColliders)
			{
				GameObject prop = propCollider.gameObject;
				Transform chunk = nearChunk(prop.transform);

				if (ReferenceEquals(chunk.gameObject, gameObject))
				{
					prop.transform.parent = transform;
				}
			}
		}

		Transform nearChunk(Transform prop)
		{
			Transform closestChunk = null;
			float minimumDistance = Mathf.Infinity;

			foreach (Transform chunk in transform.parent)
			{
				float distance = Vector3.Distance(chunk.transform.position, prop.position);
				if (distance < minimumDistance)
				{
					minimumDistance = distance;
					closestChunk = chunk;
				}
			}
			return closestChunk;
		}
	}

	internal class AsyncOperationHandle
	{
	}
}

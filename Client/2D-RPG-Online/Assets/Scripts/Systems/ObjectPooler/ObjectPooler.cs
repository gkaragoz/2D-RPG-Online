using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    [System.Serializable]
    public class Pool {
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    public List<Pool> pools = new List<Pool>();
    public Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    private void Start() {
        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int jj = 0; jj < pool.size; jj++) {
                GameObject newObject = Instantiate(pool.prefab, transform);
                newObject.SetActive(false);

                objectPool.Enqueue(newObject);
            }

            poolDictionary.Add(pool.prefab.GetInstanceID(), objectPool);
        }
    }

    public GameObject SpawnFromPool(int key, Vector3 position, Quaternion rotation) {
        if (!poolDictionary.ContainsKey(key)) {
            Debug.LogError("Pool with key " + key + " doesn't exists.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[key].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
    
        if (pooledObject != null) {
            pooledObject.OnObjectReused();
        }

        poolDictionary[key].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

}

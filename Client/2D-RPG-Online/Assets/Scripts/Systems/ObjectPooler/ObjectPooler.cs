using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    [System.Serializable]
    public class Pool {
        public string name;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools = new List<Pool>();
    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    public void InitializePool(string name) {
        Pool pool = pools.Where(p => p.name == name).First(); 

        if (pool == null) {
            Debug.LogError("Pool with key " + name + " doesn't exists.");
            return;
        }

        if (poolDictionary.ContainsKey(pool.name)) {
            Debug.LogError("Pool with key " + pool.name + " already exists on dictionary.");
            return;
        }

        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int ii = 0; ii < pool.size; ii++) {
            GameObject newObject = Instantiate(pool.prefab, transform);
            newObject.SetActive(false);
            objectPool.Enqueue(newObject);
        }

        poolDictionary.Add(pool.name, objectPool);
    }

    public void ClearPool(string name) {
        Pool pool = pools.Where(p => p.name == name).First();

        if (pool == null) {
            Debug.LogError("Pool with key " + name + " doesn't exists.");
            return;
        }

        if (!poolDictionary.ContainsKey(pool.name)) {
            Debug.LogError("Pool with key " + pool.name + " didn't initiliaze yet.");
            return;
        }

        Queue<GameObject> willDeletePool = poolDictionary[pool.name];

        for (int ii = 0; ii < willDeletePool.Count; ii++) {
            Destroy(willDeletePool.Dequeue());
        }
    }

    public GameObject SpawnFromPool(string name) {
        if (!poolDictionary.ContainsKey(name)) {
            Debug.LogError("Pool with key " + name + " doesn't exists.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[name].Dequeue();
        objectToSpawn.SetActive(true);

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
    
        if (pooledObject != null) {
            pooledObject.OnObjectReused();
        }

        poolDictionary[name].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

}

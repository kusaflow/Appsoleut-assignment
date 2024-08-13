using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab; // The prefab for pooled objects
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public GameObject GetObject(Transform parent)
    {
        if (availableObjects.Count > 0)
        {
            var obj = availableObjects.Dequeue();
            obj.transform.SetParent(parent);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instantiate(prefab, parent);
            return newObj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        availableObjects.Enqueue(obj);
    }
}

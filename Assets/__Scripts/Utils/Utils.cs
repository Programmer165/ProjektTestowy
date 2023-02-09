using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class Utils
{
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-20, 20), 1.01f, Random.Range(-20, 20));
    }

    public static void SetRenderLayerInChildren(Transform transform, int layerNum)
    {
        foreach (Transform _transform in transform.GetComponentInChildren<Transform>(true))
        {
            if (transform.CompareTag("IgnoreLayer"))
                continue;

            _transform.gameObject.layer = layerNum;
        }
    }
}

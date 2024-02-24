using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonoBehaviour : MonoBehaviour
{
    protected T InstantiateObj<T>(GameObject tObj, Vector3 vec, Quaternion quaternion)
    {
        var obj = Instantiate(tObj, this.transform);
        obj.transform.position = vec;
        obj.transform.rotation = quaternion;
        return obj.GetComponent<T>();
    }
}

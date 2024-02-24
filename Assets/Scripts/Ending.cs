using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Ending : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField]
    float alpha2 = 0;

    [SerializeField]
    Material alphaMat2;

    private void Start()
    {

        alphaMat2.SetFloat("_Alpha", 0);
    }

    private void Update()
    {

        alphaMat2.SetFloat("_Alpha", alpha2);
    }
}

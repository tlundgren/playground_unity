using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines an area, and returns random points within.
public class WanderRegion : MonoBehaviour
{
    [Tooltip("The region will extend from the center forward/backward half the size.")]
    public Vector3 size;

    public Vector3 GetRandomPointWithin()
    {
        float x = transform.position.x + Random.Range(size.x * -.5f, size.x * .5f);
        float z = transform.position.z + Random.Range(size.z * -.5f, size.z * .5f);
        return new Vector3(x, transform.position.y, z);
    }

    void Awake()
    {
        // set self as region in attached wanderers
        var wanderers = gameObject.GetComponentsInChildren<Wanderer>();
        for (int i = 0; i < wanderers.Length; i++)
        {
            wanderers[i].region = this;
        }
    }
}

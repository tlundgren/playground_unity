using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves Projectiles on Update().
public class Projectile : MonoBehaviour
{
    [Header("References")]
    public Transform trans;
    [Header("Stats")]
    [Tooltip("How many units the projectile moves forward per second.")]
    public float speed = 34;
    [Tooltip("The distance the projectile travels before coming to a stop.")]
    public float range = 70;
    private Vector3 spawnPoint;

    void Start()
    {
        spawnPoint = trans.position;
    }

    void Update()
    {
        // move along z axis
        trans.Translate(0, 0, speed * Time.deltaTime, Space.Self);
        // destroy after distance limit
        if (Vector3.Distance(trans.position, spawnPoint) >= range)
            Destroy(gameObject);
    }
}

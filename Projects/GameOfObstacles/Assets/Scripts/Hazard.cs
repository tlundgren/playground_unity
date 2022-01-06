using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kills Player OnTriggerEnter.
public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) // ie player layer
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
                player.Die(); // kill player on the spot
        }
    }
}

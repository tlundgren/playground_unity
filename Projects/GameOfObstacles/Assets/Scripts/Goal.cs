using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Reloads Main scene OnTriggerEnter against Player.
public class Goal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) // ie player layer
            SceneManager.LoadScene("Main");
    }
}

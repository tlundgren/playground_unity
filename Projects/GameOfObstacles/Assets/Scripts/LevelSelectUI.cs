using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    private int currentSceneIndex = 0;
    private GameObject levelViewCamera;
    private AsyncOperation currentLoadOperation;

    void Start()
    {
        DontDestroyOnLoad(gameObject); // outlive scenes
    }

    void Update()
    {
        if (currentLoadOperation != null && currentLoadOperation.isDone)
        {
            currentLoadOperation = null;
            levelViewCamera = GameObject.Find("Level View Camera");
            if (levelViewCamera == null)
                Debug.LogError("No level view camera was found in the scene.");
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Obstacle Course");

        // show play if a level other than main/menu is selected, on click play
        if (currentSceneIndex != 0)
        {
            GUILayout.Label("Currently viewing Level " + currentSceneIndex);
            if (GUILayout.Button("Play")) // on click, play
            {
                PlayCurrentLevel();
            }
        }
        else
            GUILayout.Label("Select a level to preview it.");

        // show available levels, on click load
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (GUILayout.Button("Level " + i))
            {
                if (currentLoadOperation == null)
                {
                    currentLoadOperation = SceneManager.LoadSceneAsync(i);
                    currentSceneIndex = i;
                }
            }
        }
    }

    private void PlayCurrentLevel()
    {
        // deactivate preview camera, activate player script and camera
        levelViewCamera.SetActive(false);
        var playerGobj = GameObject.Find("Player");
        if (playerGobj == null)
            Debug.LogError("No player was found in the scene.");
        else
        {
            var playerScript = playerGobj.GetComponent<Player>();
            playerScript.enabled = true;
            playerScript.playerCamera.SetActive(true);
            Destroy(this.gameObject);
        }
    }

}

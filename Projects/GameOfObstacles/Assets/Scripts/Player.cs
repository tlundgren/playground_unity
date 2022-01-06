using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Moves Player (position of Character controller, rotation of model) on Update().
// Respawns the Player on Die().
// Pauses game on Update(); pauses game, shows menu, responds to menu items.
public class Player : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform of the Player object, eg does not rotate with input.")]
    public Transform trans;
    [Tooltip("Transform of the visible part of the Player, eg rotates with input.")]
    public Transform modelTrans;
    public CharacterController characterController;
    public GameObject playerCamera;

    [Header("Death and Respawning")]
    [Tooltip("Seconds after death before player is respawned.")]
    public float respawnWaitTime = 2f;
    private bool dead = false;
    private Vector3 spawnPoint;
    private Quaternion spawnRotation;

    [Header("Dashing")]
    [Tooltip("Units moved with a dash.")]
    public float dashDistance = 17;
    [Tooltip("Duration of a dash, in seconds.")]
    public float dashDuration = .26f;
    [Tooltip("Minimum time between dashes, in seconds.")]
    public float dashCooldown = 1.8f;
    private Vector3 dashDirection; // values of -1, 0, 1 for x, z
    private float dashBeginTime = Mathf.NegativeInfinity;
    private bool IsDashing
    {
        get
        {
            return (Time.time < dashBeginTime + dashDuration);
        }
    }
    private bool CanDash
    {
        get
        {
            return (Time.time >= dashBeginTime + dashDuration + dashCooldown);
        }
    }


    [Header("Movement")]
    [Tooltip("Units moved per second at maximum speed.")]
    public float movespeed = 24;
    [Tooltip("Time, in seconds, to reach maximum speed.")]
    public float timeToMaxSpeed = .26f;
    private float VelocityGainPerSecond
    {
        get
        {
            return movespeed / timeToMaxSpeed;
        }
    }
    [Tooltip("Time, in seconds, to go from maximum speed to stationary.")]
    public float timeToLoseMaxSpeed = .2f;
    private float VelocityLossPerSecond
    {
        get
        {
            return movespeed / timeToLoseMaxSpeed;
        }
    }
    [Tooltip("Multiplier for momentum when attempting to move in a direction opposite the current traveling direction (eg from left to right).")]
    public float reverseMomentumMultiplier = 2.2f;
    private Vector3 movementVelocity = Vector3.zero; // will vary between +/- movespeed

    private bool paused = false;



    void Start()
    {
        spawnPoint = trans.position; // the starting position
        spawnRotation = trans.rotation;
    }

    void Update()
    {
        if (!paused)
        {
            Move();
            Dash();
        }
        Pause();
    }

    void OnGUI()
    {
        if (paused)
        {
            float boxWidth = Screen.width * .4f;
            float boxHeight = Screen.height * .4f;
            GUILayout.BeginArea(new Rect((Screen.width * .5f) - (boxWidth * .5f),(Screen.height * .5f) - (boxHeight * .5f),boxWidth,boxHeight));
            if (GUILayout.Button("Resume Game", GUILayout.Height(boxHeight *.5f)))
            {
                paused = false;
                Time.timeScale = 1;
            }
            if (GUILayout.Button("Quit to Main Menu", GUILayout.Height(boxHeight * .5f)))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            }
            GUILayout.EndArea();
        }
    }

    private void Move()
    {
        if (IsDashing)
            return;
        // for testing respawns
        if (Input.GetKey(KeyCode.R))
            Die();

        // update movementVelocity forward/backward
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            // if already forward (velocity positive) increase normally, else (velocity negative) decrease to zero faster
            if (movementVelocity.z >= 0)
                movementVelocity.z = Mathf.Min(movespeed, movementVelocity.z + VelocityGainPerSecond * Time.deltaTime);
            else
                movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            // same for backward movement
            if (movementVelocity.z > 0)
                movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            else
                movementVelocity.z = Mathf.Max(-movespeed, movementVelocity.z - VelocityGainPerSecond * Time.deltaTime);
        }
        else
        {
            // take speed to 0
            if (movementVelocity.z > 0)
                movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityLossPerSecond * Time.deltaTime);
            else
                movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityLossPerSecond * Time.deltaTime);
        }

        // update movementVelocity right/left
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            // if already right (velocity positive) increase normally, else (velocity negative) decrease to zero faster
            if (movementVelocity.x >= 0)
                movementVelocity.x = Mathf.Min(movespeed, movementVelocity.x + VelocityGainPerSecond * Time.deltaTime);
            else
                movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            // same for left movement
            if (movementVelocity.x > 0)
                movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            else
                movementVelocity.x = Mathf.Max(-movespeed, movementVelocity.x - VelocityGainPerSecond * Time.deltaTime);
        }
        else
        {
            // take speed to 0
            if (movementVelocity.x > 0)
                movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityLossPerSecond * Time.deltaTime);
            else
                movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityLossPerSecond * Time.deltaTime);
        }

        // apply movementVelocity
        if (movementVelocity.x != 0 || movementVelocity.z != 0)
        {
            characterController.Move(movementVelocity * Time.deltaTime); // move
            modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, Quaternion.LookRotation(movementVelocity), .18f); // rotate model
        }

    }

    private void Dash()
    {
        if (!IsDashing)
        {
            // Player can dash pressing space while holding down at least one directional key
            if (Input.GetKey(KeyCode.Space) && CanDash)
            {
                Vector3 movementDir = Vector3.zero;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    movementDir.z = 1;
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    movementDir.z = -1;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    movementDir.x = 1;
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    movementDir.x = -1;
                if (movementDir.x != 0 || movementDir.z != 0)
                {
                    dashDirection = movementDir;
                    dashBeginTime = Time.time;
                    movementVelocity = dashDirection * movespeed;
                    modelTrans.forward = dashDirection;
                }
            }
        }
        else
        {
            characterController.Move(dashDirection * (dashDistance / dashDuration) * Time.deltaTime);
        }
    }

    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            if (paused)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            Invoke("Respawn", respawnWaitTime);
            movementVelocity = Vector3.zero;
            dashBeginTime = Mathf.NegativeInfinity;
            enabled = false;
            characterController.enabled = false;
            modelTrans.gameObject.SetActive(false);
        }
    }


    public void Respawn()
    {
        dead = false;
        trans.position = spawnPoint;
        modelTrans.rotation = spawnRotation;
        enabled = true;
        characterController.enabled = true;
        modelTrans.gameObject.SetActive(true);
    }

}

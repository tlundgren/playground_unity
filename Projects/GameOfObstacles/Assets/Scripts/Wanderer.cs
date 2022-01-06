using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gets a point from the WanderRegion, rotates towards it, moves; gets another point.
public class Wanderer : MonoBehaviour
{
    [Header("References")]
    public Transform trans;
    public Transform modelTrans;

    [Header("Stats")]
    public float movespeed = 18;
    [Tooltip("Minimum wait time before retargeting again.")]
    public float minRetargetInterval = 2.2f;
    [Tooltip("Maximum wait time before retargeting again.")]
    public float maxRetargetInterval = 4.4f;
    [Tooltip("Time in seconds taken to rotate after targeting, before moving begins.")]
    public float rotationTime = .6f;
    [Tooltip("Time in seconds after rotation finishes before movement starts.")]
    public float postRotationWaitTime = .3f;

    [HideInInspector]
    public WanderRegion region;
    private Vector3 currentTarget; 
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float rotationStartTime;
    private enum State
    {
        Idle,
        Rotating,
        Moving
    }
    private State state = State.Idle;

    void Retarget()
    {
        currentTarget = region.GetRandomPointWithin();
        initialRotation = modelTrans.rotation;
        targetRotation = Quaternion.LookRotation((currentTarget - trans.position).normalized);
        state = State.Rotating;
        rotationStartTime = Time.time;
        Invoke("BeginMoving", rotationTime + postRotationWaitTime);
    }

    void BeginMoving()
    {
        // normally, rotation would already be targetRotation
        modelTrans.rotation = targetRotation;
        state = State.Moving;
    }

    void Start()
    {
        Retarget();
    }

    void Update()
    {
        if (state == State.Moving)
        {
            // move root game object
            float distance = movespeed * Time.deltaTime;
            trans.position = Vector3.MoveTowards(trans.position, currentTarget, distance);
            if (trans.position == currentTarget)
            {
                state = State.Idle;
                Invoke("Retarget", Random.Range(minRetargetInterval, maxRetargetInterval));
            }
        }
        else if (state == State.Rotating)
        {
            // rotate model
            float timeSpentRotating = Time.time - rotationStartTime;
            modelTrans.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeSpentRotating / rotationTime);
        }
    }

}

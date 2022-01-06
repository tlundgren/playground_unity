using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cycles scaling spike holder up/down.
// Makes trap hazard or collisionable, accordingly.
public class SpikeTrap : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Time in seconds between two raises.")]
    public float interval = 2f;
    [Tooltip("Time in seconds between spikes raised and spikes lowering.")]
    public float raiseWaitTime = .3f;
    [Tooltip("Time in seconds to lower the spikes.")]
    public float lowerTime = .6f;
    [Tooltip("Time in seconds to raise the spikes.")]
    public float raiseTime = .08f;

    [Header("References")]
    [Tooltip("Reference to the parent of all the spikes.")]
    public Transform spikeHolder;
    public GameObject hitboxGameObject;
    public GameObject collisionGameObject;

    private enum State
    {
        Lowered,
        Lowering,
        Raising,
        Raised
    }
    private State state = State.Lowered;
    private const float SpikeHeight = 5.0f;
    private const float LoweredSpikeHeight = 1.0f;
    private float lastSwitchTime = Mathf.NegativeInfinity;

    void Start()
    {
        Invoke("StartRaising", interval);
    }

    // scales spike holder down/up to simulate lowering/raising spikes
    void Update()
    {
        if (state == State.Lowering)
        {
            Vector3 scale = spikeHolder.localScale;
            scale.y = Mathf.Lerp(SpikeHeight, LoweredSpikeHeight, (Time.time - lastSwitchTime) / lowerTime);
            spikeHolder.localScale = scale;
            if (scale.y == LoweredSpikeHeight)
            {
                Invoke("StartRaising", interval);
                state = State.Lowered;
                collisionGameObject.SetActive(false);
            }
        }
        else if (state == State.Raising)
        {
            Vector3 scale = spikeHolder.localScale;
            scale.y = Mathf.Lerp(LoweredSpikeHeight, SpikeHeight, (Time.time - lastSwitchTime) / raiseTime);
            spikeHolder.localScale = scale;
            if (scale.y == SpikeHeight)
            {
                Invoke("StartLowering", raiseWaitTime);
                state = State.Raised;
                collisionGameObject.SetActive(true);
                hitboxGameObject.SetActive(false);
            }
        }
    }

    void StartRaising()
    {
        lastSwitchTime = Time.time;
        state = State.Raising;
        hitboxGameObject.SetActive(true);
    }

    void StartLowering()
    {
        lastSwitchTime = Time.time;
        state = State.Lowering;
    }
}

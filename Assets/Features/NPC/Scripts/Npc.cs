using System;
using System.Collections.Generic;
using BabbittsUnityUtils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class Npc : MonoBehaviour, INpc
{
    // NEW: Instrument name for music spatialization
    [SerializeField] private string instrumentName;
    public string InstrumentName => instrumentName;
    public void SetInstrumentName(string name) => instrumentName = name;
    // END NEW

    [SerializeField] private ENpcState state = ENpcState.Idle;
    [SerializeField] private NavMeshAgent agent;
    private bool wasAtDestination;
    private float lastUpdateTime;

    [SerializeField] private float minWaitTime = 5f;
    [SerializeField] private float maxWaitTime = 15f;
    private CountdownTimer requestNextDestinationTimer = new(0);

    private bool hasReachedDestination;

    public Vector3 GetPosition() => transform.position;
    public void RequestDestination() => NpcManager.Instance.RequestDestination(this);

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        NpcManager.Instance.RegisterNpc(this);
        requestNextDestinationTimer.OnTimerStop += RequestDestination;
    }

    private void OnDisable()
    {
        NpcManager.Instance.UnregisterNpc(this);
        requestNextDestinationTimer.OnTimerStop -= RequestDestination;
    }

    private void Start()
    {
        RequestDestination();
    }

    public void SetDestination(Vector3 destination)
    {
        state = ENpcState.Walking;
        agent.SetDestination(destination);
    }

    public void NpcUpdate()
    {
        var deltaTime = Time.time - lastUpdateTime;

        switch (state)
        {
            case ENpcState.Idle:
                IdleStateUpdate(deltaTime);
                break;
            case ENpcState.Walking:
                WalkStateUpdate(deltaTime);
                break;
        }

        lastUpdateTime = Time.time;
    }

    private void IdleStateUpdate(float deltaTime)
    {
        if (requestNextDestinationTimer.IsRunning)
            requestNextDestinationTimer.Tick(deltaTime);
    }

    private void WalkStateUpdate(float deltaTime)
    {
        hasReachedDestination = HasReachedDestination();

        if (hasReachedDestination)
            OnDestinationReached();
    }

    private void OnDestinationReached()
    {
        state = ENpcState.Idle;
        var timeBeforeRequest = Random.Range(minWaitTime, maxWaitTime);
        requestNextDestinationTimer.Reset(timeBeforeRequest);
        requestNextDestinationTimer.Start();
    }

    private bool HasReachedDestination()
    {
        if (agent.pathPending) return false;
        if (agent.remainingDistance > agent.stoppingDistance) return false;
        if (agent.velocity.sqrMagnitude > 0.01f) return false;

        return true;
    }

    private void OnDrawGizmos()
    {
        var position = transform.position + (3 * Vector3.up);

        switch (state)
        {
            case ENpcState.Idle:
                Gizmos.color = Color.red;
                break;
            case ENpcState.Walking:
                Gizmos.color = Color.green;
                break;
        }

        Gizmos.DrawSphere(position, 0.25f);

        if (!Application.isPlaying) return;

        var offset = Vector3.right * (1f / 3f);
        var boolChecks = new List<bool>
        {
            agent.pathPending,
            agent.remainingDistance > agent.stoppingDistance,
            agent.velocity.sqrMagnitude > 0.01f
        };

        for (int i = 0; i < boolChecks.Count; i++)
        {
            var multiplier = i + 1;
            Gizmos.color = boolChecks[i] ? Color.red : Color.green;
            Gizmos.DrawSphere(position + (multiplier * offset), 0.125f);
        }
    }
}
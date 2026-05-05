using System;
using BabbittsUnityUtils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MidiMite : MonoBehaviour, IMidiMite
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

    public void RequestDestination() => MidiMiteManager.Instance.RequestDestination(this);

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        MidiMiteManager.Instance.RegisterMidiMite(this);
        requestNextDestinationTimer.OnTimerStop += RequestDestination;
    }

    private void OnDisable()
    {
        MidiMiteManager.Instance.UnregisterMidiMite(this);
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

    public void MidiMiteUpdate()
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
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
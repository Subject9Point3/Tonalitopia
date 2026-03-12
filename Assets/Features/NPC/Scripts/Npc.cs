using BabbittsUnityUtils;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Npc : MonoBehaviour, INpc
{
    [SerializeField] private NavMeshAgent agent;
    private bool wasAtDestination;
    private float lastUpdateTime;
    
    [SerializeField] private float minWaitTime = 5f;
    [SerializeField] private float maxWaitTime = 15f;
    private CountdownTimer requestNextDestinationTimer = new(0);
    
    public Vector3 GetPosition() => transform.position;
    
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

    public void RequestDestination()
    {
        NpcManager.Instance.RequestDestination(this);
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void NpcUpdate()
    {
        if (requestNextDestinationTimer.IsRunning)
        {
            var deltaTime = Time.time - lastUpdateTime;
            requestNextDestinationTimer.Tick(deltaTime);
        }
        
        var atDestination = HasReachedDestination();
        if (atDestination && !wasAtDestination)
            OnDestinationReached();
    
        wasAtDestination = atDestination;
        lastUpdateTime = Time.time;
    }

    private void OnDestinationReached()
    {
        var timeBeforeRequest = Random.Range(minWaitTime, maxWaitTime);
        requestNextDestinationTimer.Reset(timeBeforeRequest);
        requestNextDestinationTimer.Start();
    }
    
    private bool HasReachedDestination()
    {
        if (agent.pathPending) return false;
        if (agent.remainingDistance > agent.stoppingDistance) return false;
        if (agent.hasPath || agent.velocity.sqrMagnitude > 0f) return false;
    
        return true;
    }
}
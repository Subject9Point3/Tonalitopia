using System.Collections.Generic;
using BabbittsUnityUtils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NpcManager : Singleton<NpcManager>
{
    [SerializeField] private float maxDestinationDistance = 40f;
    
    private NavMeshTriangulation triangulation;
    private List<INpc> npcs = new();
    private int currentNpcIndex = 0;

    public void RegisterNpc(INpc npc) => npcs.Add(npc);
    public void UnregisterNpc(INpc npc) => npcs.Remove(npc);

    private void Start()
    {
        triangulation = NavMesh.CalculateTriangulation();
        Spawner.Instance.SpawnNpcs(triangulation);
    }

    private void Update()
    {
        if (npcs.Count < 1) return;
        
        // Update one npc per frame
        npcs[currentNpcIndex].NpcUpdate();
        currentNpcIndex = (currentNpcIndex + 1) % npcs.Count;
    }
    
    public void RequestDestination(INpc npc)
    {
        CalculateNpcDestination(npc);
    }

    private void CalculateNpcDestination(INpc npc)
    {
        // Destination set logic
        var destination = NavMeshUtils.GetRandomNavMeshPositionInRange(
            triangulation, 
            npc.GetPosition(), 
            maxDestinationDistance
        );

        npc.SetDestination(destination);
    }
}
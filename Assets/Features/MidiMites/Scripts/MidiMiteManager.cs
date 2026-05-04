using System.Collections.Generic;
using BabbittsUnityUtils;
using UnityEngine;
using UnityEngine.AI;

public class MidiMiteManager : Singleton<MidiMiteManager>
{
    [SerializeField] private float maxDestinationDistance = 40f;
    
    private NavMeshTriangulation triangulation;
    private List<IMidiMite> mites = new();
    private int currentNpcIndex = 0;

    public void RegisterMidiMite(IMidiMite mite) => mites.Add(mite);
    public void UnregisterMidiMite(IMidiMite mite) => mites.Remove(mite);
    
    private void Start()
    {
        triangulation = NavMesh.CalculateTriangulation();
        Spawner.Instance.SpawnMidiMites(triangulation);
    }

    private void Update()
    {
        if (mites.Count < 1) return;
        
        // Update one npc per frame
        mites[currentNpcIndex].MidiMiteUpdate();
        currentNpcIndex = (currentNpcIndex + 1) % mites.Count;
    }
    
    public void RequestDestination(IMidiMite mite)
    {
        CalculateNpcDestination(mite);
    }

    private void CalculateNpcDestination(IMidiMite mite)
    {
        // Destination set logic
        var destination = NavMeshUtils.GetRandomNavMeshPositionInRange(
            triangulation, 
            mite.GetPosition(), 
            maxDestinationDistance
        );

        mite.SetDestination(destination);
    }
}
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using BabbittsUnityUtils;

public class Spawner : Singleton<Spawner>
{
    [SerializeField] private GameObject midiMitePrefab;
    [SerializeField] private GameObject npcPrefab;
    
    [SerializeField] private List<InstrumentObjectSO> instruments = new List<InstrumentObjectSO>();
    
    public void SpawnAll(NavMeshTriangulation triangulation)
    {
        for (int i = 0; i <= instruments.Count; i++)
        {
            Spawn<Npc>(npcPrefab, triangulation);
            Spawn<MidiMite>(midiMitePrefab, triangulation);
        }

        Debug.Log($"Spawned NPCs and MidiMites for {instruments.Count} instruments");
    }

    public void SpawnNpcs(NavMeshTriangulation triangulation)
    {
        for (int i = 0; i <= instruments.Count; i++)
        {
            Spawn<Npc>(npcPrefab, triangulation);
        }

        Debug.Log($"Spawned NPCs for {instruments.Count} instruments");
    }

    public void SpawnMidiMites(NavMeshTriangulation triangulation)
    {
        for (int i = 0; i <= instruments.Count; i++)
        {
            Spawn<MidiMite>(midiMitePrefab, triangulation);
        }

        Debug.Log($"Spawned MidiMites for {instruments.Count} instruments");
    }

    private T Spawn<T>(GameObject prefab, NavMeshTriangulation triangulation) where T : MonoBehaviour
    {
        var position = NavMeshUtils.GetRandomNavMeshPosition(triangulation);
        var obj = Instantiate(prefab, position, Quaternion.identity);

        if (obj.TryGetComponent(out T component))
            return component;

        Debug.LogWarning($"Spawner: {prefab.name} does not have a {typeof(T).Name} component");
        return null;
    }
}
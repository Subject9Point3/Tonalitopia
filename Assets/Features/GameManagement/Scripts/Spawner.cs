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
        SpawnNpcs(triangulation);
        SpawnMidiMites(triangulation);
    }

    public void SpawnNpcs(NavMeshTriangulation triangulation)
    {
        for (int i = 0; i < instruments.Count; i++)
        {
            var randomPosition = NavMeshUtils.GetRandomNavMeshPosition(triangulation);
            GameObject npcObject = Instantiate(npcPrefab, randomPosition, Quaternion.identity);

            // Set instrument name for RTPC tracking
            if (npcObject.TryGetComponent<Npc>(out var npc))
            {
                npc.SetInstrumentName(instruments[i].Name);
            }

            // NPCs start empty but know which instrument they need
            if (npcObject.TryGetComponent<InstrumentHolder>(out var holder))
            {
                holder.SetRequiredInstrument(instruments[i]);
            }
        }

        Debug.Log($"Spawned {instruments.Count} NPCs");
    }

    public void SpawnMidiMites(NavMeshTriangulation triangulation)
    {
        for (int i = 0; i < instruments.Count; i++)
        {
            var randomPosition = NavMeshUtils.GetRandomNavMeshPosition(triangulation);
            GameObject miteObject = Instantiate(midiMitePrefab, randomPosition, Quaternion.identity);

            // Set instrument name for RTPC tracking
            if (miteObject.TryGetComponent<MidiMite>(out var mite))
            {
                mite.SetInstrumentName(instruments[i].Name);
            }

            // Tell Unity this MidiMite has this instrument (for tracking)
            // Your Wwise start event handles the actual playback
            if (miteObject.TryGetComponent<InstrumentHolder>(out var holder))
            {
                holder.InitializeWithInstrument(instruments[i]);
            }
        }

        Debug.Log($"Spawned {instruments.Count} MidiMites");
    }
}
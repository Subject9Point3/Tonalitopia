using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[System.Serializable]
public class NpcSpawner
{
    [SerializeField] private GameObject npcPrefab;
    // [SerializeField] private int numNpcsToSpawn = 12;

    [SerializeField]
    private List<string> instrumentNames = new List<string>
    {
        // NPC instruments
        "Piano",
        "Flute",
        "Violin",
        "Viola",
        "Cello",
        "Bass",
        "Oboe",
        "Clarinet",
        "Bassoon",
        "Celeste",
        "Vibraphone",
        "Marimba",
        // Midimite instruments
        "Anvil",
        "Bamboo",
        "Conga",
        "Cymbal",
        "Darabuka",
        "HighCowbell",
        "HighKit",
        "KickSnare",
        "LowCowbell",
        "LowKit",
        "Tabla",
        "Udu"
    };

    public void SpawnNpcs(NavMeshTriangulation triangulation)
    {
        // for (int i = 0; i < numNpcsToSpawn; i++)
        // {
        //     var randomPosition = NavMeshUtils.GetRandomNavMeshPosition(triangulation);
        //     GameObject.Instantiate(npcPrefab, randomPosition, Quaternion.identity);
        // }

        int numNpcsToSpawn = instrumentNames.Count;

        for (int i = 0; i < numNpcsToSpawn; i++)
        {
            var randomPosition = NavMeshUtils.GetRandomNavMeshPosition(triangulation);
            GameObject npcObject = GameObject.Instantiate(npcPrefab, randomPosition, Quaternion.identity);

            Npc npc = npcObject.GetComponent<Npc>();
            if (npc != null)
            {
                npc.SetInstrumentName(instrumentNames[i]);
            }
        }

        Debug.Log($"Spawned {numNpcsToSpawn} NPCs with instruments");
    }
}
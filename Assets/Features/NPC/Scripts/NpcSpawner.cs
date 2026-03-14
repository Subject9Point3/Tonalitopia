using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class NpcSpawner
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int numNpcsToSpawn = 10;

    public void SpawnNpcs(NavMeshTriangulation triangulation)
    {
        for (int i = 0; i < numNpcsToSpawn; i++)
        {
            var randomPosition = NavMeshUtils.GetRandomNavMeshPosition(triangulation);
            GameObject.Instantiate(npcPrefab, randomPosition, Quaternion.identity);
        }
    }
}
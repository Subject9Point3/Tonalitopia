using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static Vector3 GetRandomNavMeshPosition(NavMeshTriangulation triangulation)
    {
        int randomTriangle = Random.Range(0, triangulation.indices.Length / 3) * 3;

        Vector3 a = triangulation.vertices[triangulation.indices[randomTriangle]];
        Vector3 b = triangulation.vertices[triangulation.indices[randomTriangle + 1]];
        Vector3 c = triangulation.vertices[triangulation.indices[randomTriangle + 2]];

        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;

        return (1 - r1) * a + r1 * (1 - r2) * b + r1 * r2 * c;
    }
    
    public static Vector3 GetRandomNavMeshPositionInRange(NavMeshTriangulation triangulation, Vector3 origin, float maxDistance)
    {
        var validTriangles = new List<int>();

        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            Vector3 a = triangulation.vertices[triangulation.indices[i]];
            Vector3 b = triangulation.vertices[triangulation.indices[i + 1]];
            Vector3 c = triangulation.vertices[triangulation.indices[i + 2]];

            Vector3 center = (a + b + c) / 3f;

            if (Vector3.Distance(origin, center) <= maxDistance)
                validTriangles.Add(i);
        }

        if (validTriangles.Count == 0)
            return NavMeshUtils.GetRandomNavMeshPosition(triangulation);

        int randomTriangle = validTriangles[Random.Range(0, validTriangles.Count)];

        Vector3 va = triangulation.vertices[triangulation.indices[randomTriangle]];
        Vector3 vb = triangulation.vertices[triangulation.indices[randomTriangle + 1]];
        Vector3 vc = triangulation.vertices[triangulation.indices[randomTriangle + 2]];

        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;

        return (1 - r1) * va + r1 * (1 - r2) * vb + r1 * r2 * vc;
    }
}
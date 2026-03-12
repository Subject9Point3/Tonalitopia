using UnityEngine;

public interface INpc
{
    void RequestDestination();
    void SetDestination(Vector3 destination);
    void NpcUpdate();
    Vector3 GetPosition();
}
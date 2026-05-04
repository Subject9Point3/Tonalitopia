using UnityEngine;

public interface IMidiMite
{
    void RequestDestination();
    void SetDestination(Vector3 destination);
    void MidiMiteUpdate();
    Vector3 GetPosition();
}
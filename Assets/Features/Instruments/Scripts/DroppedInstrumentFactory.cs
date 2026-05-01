using BabbittsUnityUtils;
using UnityEngine;

public class DroppedInstrumentFactory : Singleton<DroppedInstrumentFactory>
{
    [SerializeField] private GameObject droppedInstrumentPrefab;
    
    public InstrumentHolder SpawnDroppedInstrument(Vector3 position, Vector3 forward)
    {
        var dropped = Instantiate(droppedInstrumentPrefab, position + forward, Quaternion.identity);
        return dropped.GetComponent<InstrumentHolder>();
    }
}
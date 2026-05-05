using UnityEngine;

[CreateAssetMenu(fileName = "NewInstrumentObject", menuName = "Instruments/InstrumentObject")]
public class InstrumentObjectSO : ScriptableObject, IInstrument
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public Color Colour { get; private set; } = Color.white;

    [Header("Wwise Events")]
    public AK.Wwise.Event npcEvent;
    public AK.Wwise.Event miteEvent;
    public AK.Wwise.Event playerEvent;
    public AK.Wwise.Event droppedEvent;

    public void Play(EInstrumentHolderType holder, GameObject gameObject)
    {
        Debug.Log($"InstrumentObjectSO.Play() - Holder: {holder}, Instrument: {Name}");

        switch (holder)
        {
            case EInstrumentHolderType.Player:
                Debug.Log($"Posting playerEvent: {playerEvent?.Name}");
                playerEvent.Post(gameObject);
                break;
            case EInstrumentHolderType.Npc:
                Debug.Log($"Posting npcEvent: {npcEvent?.Name}");
                npcEvent.Post(gameObject);
                break;
            case EInstrumentHolderType.Mite:
                Debug.Log($"Posting miteEvent: {miteEvent?.Name}");
                miteEvent.Post(gameObject);
                break;
            case EInstrumentHolderType.Dropped:
                Debug.Log($"Posting droppedEvent: {droppedEvent?.Name}");
                droppedEvent.Post(gameObject);
                break;
        }
    }
}
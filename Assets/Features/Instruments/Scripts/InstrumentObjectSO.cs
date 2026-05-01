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
        switch (holder)
        {
           case EInstrumentHolderType.Player:
               playerEvent.Post(gameObject);
               break;
           case EInstrumentHolderType.Npc:
               npcEvent.Post(gameObject);
               break;
           case EInstrumentHolderType.Mite:
               miteEvent.Post(gameObject);
               break;
           case EInstrumentHolderType.Dropped:
               droppedEvent.Post(gameObject);
               break;
        }
    }
}
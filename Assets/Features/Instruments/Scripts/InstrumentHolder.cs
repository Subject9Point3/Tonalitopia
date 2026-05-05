using UnityEngine;
using UnityEngine.Events;

public class InstrumentHolder : MonoBehaviour
{
    [SerializeField] private EInstrumentHolderType holderType;
    [SerializeField] private InstrumentObjectSO instrument;
    [SerializeField] private InstrumentObjectSO requiredInstrument;
    [SerializeField] private bool canBeGivenTo = true;

    public UnityEvent<InstrumentObjectSO> OnInstrumentSet;
    public UnityEvent OnInstrumentCleared;

    public bool CanBeGivenTo => canBeGivenTo;

    private void Start()
    {
        if (instrument == null) return;
        SetInstrument(instrument);
    }

    public bool HasInstrument => instrument != null;
    public EInstrumentHolderType GetHolderType() => holderType;
    public InstrumentObjectSO GetInstrument() => instrument;
    public InstrumentObjectSO GetRequiredInstrument() => requiredInstrument;

    /// <summary>
    /// Used by Spawner to assign an instrument at spawn time.
    /// Start() will then call SetInstrument() to trigger the Wwise event.
    /// </summary>
    public void InitializeWithInstrument(InstrumentObjectSO startingInstrument)
    {
        instrument = startingInstrument;
    }

    /// <summary>
    /// Used by Spawner to set which instrument this NPC needs
    /// </summary>
    public void SetRequiredInstrument(InstrumentObjectSO required)
    {
        requiredInstrument = required;
    }

    private void SetInstrument(InstrumentObjectSO newInstrument)
    {
        instrument = newInstrument;

        if (newInstrument == null) return;

        // Notify the spatial manager that this instrument changed hands
        MusicSpatialManager.Instance?.UpdateInstrumentHolder(newInstrument.Name, this);

        OnInstrumentSet.Invoke(newInstrument);
        PlayInstrument();
    }

    private void ClearInstrument()
    {
        // Fire the dropped/none event BEFORE clearing the reference
        if (instrument != null)
        {
            instrument.Play(EInstrumentHolderType.Dropped, gameObject);
        }

        instrument = null;
        OnInstrumentCleared.Invoke();
    }

    public void GiveInstrument(InstrumentHolder instrumentHolder)
    {
        if (instrumentHolder.GetInstrument() != null) return;
        if (!instrumentHolder.CanBeGivenTo) return;
        if (instrumentHolder.GetRequiredInstrument() != null && instrumentHolder.GetRequiredInstrument() != instrument) return;

        instrumentHolder.SetInstrument(instrument);
        ClearInstrument();
    }

    public void TakeInstrument(InstrumentHolder instrumentHolder)
    {
        if (instrumentHolder.GetInstrument() == null) return;

        if (instrument != null) DropInstrument();

        instrumentHolder.GiveInstrument(this);
    }

    private void DropInstrument()
    {
        if (instrument == null) return;
        var droppedHolder = DroppedInstrumentFactory.Instance.SpawnDroppedInstrument(transform.position, transform.forward);
        GiveInstrument(droppedHolder);
    }

    private void PlayInstrument()
    {
        instrument?.Play(holderType, gameObject);
    }
}
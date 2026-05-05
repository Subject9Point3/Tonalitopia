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

    public void InitializeWithInstrument(InstrumentObjectSO startingInstrument)
    {
        instrument = startingInstrument;
    }

    public void SetRequiredInstrument(InstrumentObjectSO required)
    {
        requiredInstrument = required;
    }

    private void SetInstrument(InstrumentObjectSO newInstrument)
    {
        instrument = newInstrument;

        if (newInstrument == null) return;

        MusicSpatialManager.Instance?.UpdateInstrumentHolder(newInstrument.Name, this);

        if (holderType == EInstrumentHolderType.Player)
        {
            InstrumentPairIndicator.Instance?.OnPlayerPickedUpInstrument(newInstrument.Name);
        }

        OnInstrumentSet.Invoke(newInstrument);
        PlayInstrument();
    }

    private void ClearInstrument(bool fireDroppedEvent = true)
    {
        if (holderType == EInstrumentHolderType.Player && instrument != null)
        {
            InstrumentPairIndicator.Instance?.OnPlayerReleasedInstrument();
        }

        if (fireDroppedEvent && instrument != null)
        {
            instrument.Play(EInstrumentHolderType.Dropped, gameObject);
        }

        instrument = null;
        OnInstrumentCleared.Invoke();
    }

    // ONE GiveInstrument method - merged logic with no dropped event on transfer
    public void GiveInstrument(InstrumentHolder instrumentHolder)
    {
        if (instrument == null) return;
        if (instrumentHolder.GetInstrument() != null) return;
        if (!instrumentHolder.CanBeGivenTo) return;
        if (instrumentHolder.GetRequiredInstrument() != null && instrumentHolder.GetRequiredInstrument() != instrument) return;

        instrumentHolder.SetInstrument(instrument);
        ClearInstrument(false);  // Don't fire dropped event - instrument was transferred
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
        Debug.Log($"PlayInstrument called - HolderType: {holderType}, Instrument: {instrument?.Name}");
        instrument?.Play(holderType, gameObject);
    }
}
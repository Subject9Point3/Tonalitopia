using UnityEngine;
using UnityEngine.Events;

public class InstrumentHolder : MonoBehaviour
{
    [SerializeField] private InstrumentObjectSO instrument;

    public UnityEvent<InstrumentObjectSO> OnInstrumentSet;
    public UnityEvent OnInstrumentCleared;

    private void Start()
    {
        if (instrument == null) return;
        SetInstrument(instrument);
    }

    public InstrumentObjectSO GetInstrument() => instrument;
    
    private void SetInstrument(InstrumentObjectSO newInstrument)
    {
        instrument = newInstrument;
        
        if (newInstrument == null) return;
        
        OnInstrumentSet.Invoke(newInstrument);
        PlayInstrument();
    }
    
    private void ClearInstrument() 
    {
        SetInstrument(null);
        OnInstrumentCleared.Invoke();
    }

    public void GiveInstrument(InstrumentHolder instrumentHolder)
    {
        if (instrumentHolder.GetInstrument() != null) return;
        
        instrumentHolder.SetInstrument(instrument);
        ClearInstrument();
    }

    public void TakeInstrument(InstrumentHolder instrumentHolder)
    {
        if (instrumentHolder.GetInstrument() == null) return;
        
        instrumentHolder.GiveInstrument(this);
    }

    private void PlayInstrument()
    {
        instrument?.Play();
    }
}
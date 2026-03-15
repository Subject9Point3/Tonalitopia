using UnityEngine;

[CreateAssetMenu(fileName = "NewInstrumentObject", menuName = "Instruments/InstrumentObject")]
public class InstrumentObjectSO : ScriptableObject, IInstrument
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    
    // Wwise sound reference
    // Wwise switch reference
    
    public void Play()
    {
        Debug.Log($"Playing {Name}");
    }
}

public interface IInstrument
{
    public void Play();
}
using UnityEngine;

public interface IInstrument
{
    public string Name { get; }
    public void Play(EInstrumentHolderType holder, GameObject gameObject);
}
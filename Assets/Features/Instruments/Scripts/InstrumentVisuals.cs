using UnityEngine;

public class InstrumentVisuals : MonoBehaviour
{
    [SerializeField] private Transform instrumentTransform;

    public void SetInstrument(InstrumentObjectSO newInstrument)
    {
        var instrument = Instantiate(newInstrument.Prefab, instrumentTransform);
        instrument.GetComponent<MeshRenderer>().material.color = newInstrument.Colour;
    }

    public void ClearInstrument()
    {
        if (instrumentTransform.childCount < 1) return;
        Destroy(instrumentTransform.GetChild(0).gameObject);
    }
}
using UnityEngine;

public class InstrumentVisuals : MonoBehaviour
{
    [SerializeField] private Transform instrumentTransform;

    public void SetInstrument(InstrumentObjectSO newInstrument)
    {
        Instantiate(newInstrument.Prefab, instrumentTransform);
    }

    public void ClearInstrument()
    {
        Destroy(instrumentTransform.GetChild(0).gameObject);
    }
}
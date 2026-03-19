using BabbittsUnityUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstrumentHolderInteractor : MonoBehaviour
{
    private InstrumentHolder currentInstrumentHolder;
    private float interactionStartTime;
    private bool isInteracting;

    [SerializeField] private bool canInteract = true;
    [SerializeField] private InstrumentHolder instrumentHolder;

    [Header("Overlap Settings")] 
    [SerializeField] private Transform interactPoint;
    [SerializeField] private float interactRadius;
    [SerializeField] private LayerMask interactableLayers;
    
    [Header("Input Settings")] 
    [SerializeField] private bool interactPressed;
    public InputActionReference interactAction;

    private void OnEnable()
    {
        interactAction.action.Enable();
        InputUtils.RegisterInputPhases(interactAction.action, OnInteract, InputPhases.Started);
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
        InputUtils.UnregisterInputPhases(interactAction.action, OnInteract);
    }

    private void Update() => DetectHolder();

    private void DetectHolder()
    {
        var colliders = Physics.OverlapSphere(interactPoint.position, interactRadius, interactableLayers);

        InstrumentHolder closest = null;
        float closestDistance = float.MaxValue;

        foreach (var col in colliders)
        {
            if (!col.TryGetComponent(out InstrumentHolder holder)) continue;
            if (holder == instrumentHolder) continue;

            float distance = Vector3.Distance(interactPoint.position, col.transform.position);
            if (distance >= closestDistance) continue;

            closestDistance = distance;
            closest = holder;
        }

        currentInstrumentHolder = closest;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!canInteract || currentInstrumentHolder == null) return;
        if (currentInstrumentHolder.HasInstrument)
            instrumentHolder.TakeInstrument(currentInstrumentHolder);
        else 
            instrumentHolder.GiveInstrument(currentInstrumentHolder);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blueViolet;
        Gizmos.DrawWireSphere(interactPoint.position, interactRadius);
    }
}
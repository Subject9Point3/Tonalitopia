using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSpatialManager : MonoBehaviour
{
    private Camera cam;

    [Header("Wwise Event")]
    public AK.Wwise.Event masterMusicEvent;

    [Header("Player Reference")]
    public GameObject player;

    [Header("Distance Settings")]
    public float maxDistance = 50f;

    [Header("Pan Smoothing")]
    [Tooltip("Higher = faster response, Lower = smoother. Try 5-15.")]
    public float panSmoothSpeed = 8f;

    [Header("Stereo Spread")]
    [Range(0f, 1f)]
    [Tooltip("At max distance, stereo spread is reduced to this fraction. 0 = fully mono, 1 = no narrowing")]
    public float minSpreadAtMaxDistance = 0.2f;

    [Tooltip("Distance at which spread starts narrowing. Below this, full stereo spread.")]
    public float spreadNarrowingStartDistance = 5f;

    // Track instruments by their current holder
    private Dictionary<string, InstrumentHolder> trackedInstruments = new Dictionary<string, InstrumentHolder>();
    private Dictionary<string, float> smoothedPanAngles = new Dictionary<string, float>();
    private bool isSetupComplete = false;

    // Singleton for easy access from InstrumentHolder
    public static MusicSpatialManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(0.5f);

        // Find all InstrumentHolders that currently have instruments
        var allHolders = FindObjectsOfType<InstrumentHolder>();
        foreach (var holder in allHolders)
        {
            if (holder.HasInstrument)
            {
                string instrumentName = holder.GetInstrument().Name;
                trackedInstruments[instrumentName] = holder;
                Debug.Log($"Tracking '{instrumentName}' held by {holder.GetHolderType()} at {holder.transform.position}");
            }
        }

        Debug.Log($"Total instruments tracked: {trackedInstruments.Count}");

        masterMusicEvent.Post(gameObject);
        Debug.Log("Music started!");

        isSetupComplete = true;
    }

    /// <summary>
    /// Called by InstrumentHolder when an instrument changes hands
    /// </summary>
    public void UpdateInstrumentHolder(string instrumentName, InstrumentHolder newHolder)
    {
        if (newHolder != null)
        {
            trackedInstruments[instrumentName] = newHolder;
            Debug.Log($"Instrument '{instrumentName}' now held by {newHolder.GetHolderType()}");
        }
    }

    void Update()
    {
        // === TEST CONTROLS ===
        if (UnityEngine.InputSystem.Keyboard.current.mKey.wasPressedThisFrame)
        {
            Debug.Log("TEST: Setting all instruments to MIDIMITE state");
            foreach (var kvp in trackedInstruments)
            {
                var instrument = kvp.Value.GetInstrument();
                if (instrument != null)
                {
                    instrument.Play(EInstrumentHolderType.Mite, kvp.Value.gameObject);
                }
            }
        }

        if (UnityEngine.InputSystem.Keyboard.current.nKey.wasPressedThisFrame)
        {
            Debug.Log("TEST: Setting all instruments to NPC state");
            foreach (var kvp in trackedInstruments)
            {
                var instrument = kvp.Value.GetInstrument();
                if (instrument != null)
                {
                    instrument.Play(EInstrumentHolderType.Npc, kvp.Value.gameObject);
                }
            }
        }

        if (UnityEngine.InputSystem.Keyboard.current.pKey.wasPressedThisFrame)
        {
            Debug.Log("TEST: Setting all instruments to PLAYER state");
            foreach (var kvp in trackedInstruments)
            {
                var instrument = kvp.Value.GetInstrument();
                if (instrument != null)
                {
                    instrument.Play(EInstrumentHolderType.Player, kvp.Value.gameObject);
                }
            }
        }

        if (UnityEngine.InputSystem.Keyboard.current.oKey.wasPressedThisFrame)
        {
            Debug.Log("TEST: Setting all instruments to NONE/OFF state");
            foreach (var kvp in trackedInstruments)
            {
                var instrument = kvp.Value.GetInstrument();
                if (instrument != null)
                {
                    instrument.Play(EInstrumentHolderType.Dropped, kvp.Value.gameObject);
                }
            }
        }
        // === END TEST CONTROLS ===

        if (!isSetupComplete || player == null) return;

        foreach (var kvp in trackedInstruments)
        {
            string instrumentName = kvp.Key;
            InstrumentHolder holder = kvp.Value;

            // Safety check
            if (holder == null) continue;

            // Skip spatialization if player is holding it (attached to listener)
            if (holder.GetHolderType() == EInstrumentHolderType.Player)
            {
                // Set to center/close values when player holds it
                AkUnitySoundEngine.SetRTPCValue($"{instrumentName}_Distance", 0f);
                AkUnitySoundEngine.SetRTPCValue($"{instrumentName}_Panning", 0f);
                continue;
            }

            // Calculate distance
            float distance = Vector3.Distance(player.transform.position, holder.transform.position);
            distance = Mathf.Min(distance, maxDistance);

            // Calculate panning with spread narrowing
            float panAngle = CalculatePanAngle(holder.transform.position, instrumentName, distance);

            // Set RTPCs (matching your Wwise naming: CelesteHighKit_Distance, CelesteHighKit_Panning)
            AkUnitySoundEngine.SetRTPCValue($"{instrumentName}_Distance", distance);
            AkUnitySoundEngine.SetRTPCValue($"{instrumentName}_Panning", panAngle);
        }
    }

    float CalculatePanAngle(Vector3 targetPosition, string instrumentName, float distance)
    {
        // Get direction from camera to target
        Vector3 directionToTarget = targetPosition - cam.transform.position;
        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude < 0.001f)
            return smoothedPanAngles.ContainsKey(instrumentName) ? smoothedPanAngles[instrumentName] : 0f;

        // Get raw angle (-180 to 180)
        float rawAngle = Vector3.SignedAngle(cam.transform.forward, directionToTarget, Vector3.up);

        // === Distance-based stereo spread narrowing ===
        float spreadMultiplier;

        if (distance <= spreadNarrowingStartDistance)
        {
            // Within close range: full stereo spread
            spreadMultiplier = 1f;
        }
        else
        {
            // Beyond threshold: gradually narrow
            float effectiveDistance = distance - spreadNarrowingStartDistance;
            float effectiveMaxDistance = maxDistance - spreadNarrowingStartDistance;

            if (effectiveMaxDistance <= 0f)
            {
                spreadMultiplier = minSpreadAtMaxDistance;
            }
            else
            {
                float t = Mathf.Clamp01(effectiveDistance / effectiveMaxDistance);
                spreadMultiplier = Mathf.Lerp(1f, minSpreadAtMaxDistance, t);
            }
        }

        // Apply spread narrowing using trigonometric decomposition
        // This preserves front/back position while narrowing left/right spread
        float angleRad = rawAngle * Mathf.Deg2Rad;
        float frontBackComponent = Mathf.Cos(angleRad);
        float leftRightComponent = Mathf.Sin(angleRad);

        // Only narrow the left/right spread
        leftRightComponent *= spreadMultiplier;

        // Reconstruct the angle
        float targetAngle = Mathf.Atan2(leftRightComponent, frontBackComponent) * Mathf.Rad2Deg;

        // === Smoothing ===
        if (!smoothedPanAngles.ContainsKey(instrumentName))
        {
            smoothedPanAngles[instrumentName] = targetAngle;
            return targetAngle;
        }

        float currentAngle = smoothedPanAngles[instrumentName];
        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
        float smoothedAngle = currentAngle + angleDiff * Time.deltaTime * panSmoothSpeed;

        // Normalize to -180 to 180
        if (smoothedAngle > 180f) smoothedAngle -= 360f;
        if (smoothedAngle < -180f) smoothedAngle += 360f;

        smoothedPanAngles[instrumentName] = smoothedAngle;

        return smoothedAngle;
    }
}
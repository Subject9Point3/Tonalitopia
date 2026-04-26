using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MusicSpatialManager : MonoBehaviour
{
    private Camera cam;
    
    [Header("Wwise Event")]
    public AK.Wwise.Event masterMusicEvent;

    [Header("Player Reference")]
    public GameObject player;

    [Header("Settings")]
    public float maxDistance = 50f;

    [Header("Pan Smoothing")]
    [Tooltip("Higher = faster response, Lower = smoother. Try 5-15.")]
    public float panSmoothSpeed = 8f;

    private Dictionary<string, GameObject> instrumentNpcs = new Dictionary<string, GameObject>();
    private Dictionary<string, float> smoothedPanAngles = new Dictionary<string, float>();
    private bool isSetupComplete = false;

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(0.5f);

        // Find all NPCs and map them to instruments
        var allNpcs = FindObjectsOfType<Npc>();
        foreach (var npc in allNpcs)
        {
            string instrumentName = npc.InstrumentName;
            if (!string.IsNullOrEmpty(instrumentName))
            {
                instrumentNpcs[instrumentName] = npc.gameObject;
                Debug.Log($"Mapped instrument '{instrumentName}' to NPC at {npc.transform.position}");
            }
        }

        Debug.Log($"Total instruments mapped: {instrumentNpcs.Count}");

        // Post the music event on a game object
        masterMusicEvent.Post(gameObject);
        Debug.Log("Music started!");

        isSetupComplete = true;
    }

    void Update()
    {
        if (!isSetupComplete || player == null) return;

        // Update distance and panning RTPCs for each instrument
        foreach (var kvp in instrumentNpcs)
        {
            string instrumentName = kvp.Key;
            GameObject npcObject = kvp.Value;

            // Calculate distance
            float distance = Vector3.Distance(player.transform.position, npcObject.transform.position);
            distance = Mathf.Min(distance, maxDistance);

            // Calculate panning angle with smoothing (-180 to 180)
            float panAngle = CalculatePanAngle(npcObject.transform.position, instrumentName);

            // Get RTPC names
            string distanceRtpc = GetDistanceRTPCName(instrumentName);
            string panRtpc = GetPanningRTPCName(instrumentName);

            // Set the RTPC values
            if (!string.IsNullOrEmpty(distanceRtpc))
                AkSoundEngine.SetRTPCValue(distanceRtpc, distance);

            if (!string.IsNullOrEmpty(panRtpc))
                AkSoundEngine.SetRTPCValue(panRtpc, panAngle);
        }
    }

    float CalculatePanAngle(Vector3 npcPosition, string instrumentName)
    {
        // Get direction from camera to NPC
        Vector3 directionToNpc = npcPosition - cam.transform.position;
        directionToNpc.y = 0;

        if (directionToNpc.sqrMagnitude < 0.001f)
            return 0f;

        // Get target angle (-180 to 180)
        float targetAngle = Vector3.SignedAngle(cam.transform.forward, directionToNpc, Vector3.up);

        // Initialize if first time
        if (!smoothedPanAngles.ContainsKey(instrumentName))
        {
            smoothedPanAngles[instrumentName] = targetAngle;
            return targetAngle;
        }

        float currentAngle = smoothedPanAngles[instrumentName];

        // Use Mathf.DeltaAngle to handle wrap-around correctly!
        // This gives the shortest path between angles (handles -180/180 crossing)
        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Smoothly move toward target
        float smoothedAngle = currentAngle + angleDiff * Time.deltaTime * panSmoothSpeed;

        // Normalize back to -180 to 180
        if (smoothedAngle > 180f) smoothedAngle -= 360f;
        if (smoothedAngle < -180f) smoothedAngle += 360f;

        // Store for next frame
        smoothedPanAngles[instrumentName] = smoothedAngle;

        return smoothedAngle;
    }

    string GetDistanceRTPCName(string instrumentName)
    {
        switch (instrumentName)
        {
            // NPC instruments (orchestral)
            case "Piano": return "NPC_Distance_Piano";
            case "Flute": return "NPC_Distance_Flute";
            case "Violin": return "NPC_Distance_Violin";
            case "Viola": return "NPC_Distance_Viola";
            case "Cello": return "NPC_Distance_Cello";
            case "Bass": return "NPC_Distance_Bass";
            case "Oboe": return "NPC_Distance_Oboe";
            case "Clarinet": return "NPC_Distance_Clarinet";
            case "Bassoon": return "NPC_Distance_Bassoon";
            case "Celeste": return "NPC_Distance_Celeste";
            case "Vibraphone": return "NPC_Distance_Vibraphone";
            case "Marimba": return "NPC_Distance_Marimba";

            // Midimite instruments (percussion)
            case "Anvil": return "Midimite_Distance_Anvil";
            case "Bamboo": return "Midimite_Distance_Bamboo";
            case "Conga": return "Midimite_Distance_Conga";
            case "Cymbal": return "Midimite_Distance_Cymbal";
            case "Darabuka": return "Midimite_Distance_Darabuka";
            case "HighCowbell": return "Midimite_Distance_HighCowbell";
            case "HighKit": return "Midimite_Distance_HighKit";
            case "KickSnare": return "Midimite_Distance_KickSnare";
            case "LowCowbell": return "Midimite_Distance_LowCowbell";
            case "LowKit": return "Midimite_Distance_LowKit";
            case "Tabla": return "Midimite_Distance_Tabla";
            case "Udu": return "Midimite_Distance_Udu";

            default:
                Debug.LogWarning($"No Distance RTPC mapping for instrument: {instrumentName}");
                return "";
        }
    }

    string GetPanningRTPCName(string instrumentName)
    {
        switch (instrumentName)
        {
            // NPC instruments (orchestral)
            case "Piano": return "NPC_Panning_Piano";
            case "Flute": return "NPC_Panning_Flute";
            case "Violin": return "NPC_Panning_Violin";
            case "Viola": return "NPC_Panning_Viola";
            case "Cello": return "NPC_Panning_Cello";
            case "Bass": return "NPC_Panning_Bass";
            case "Oboe": return "NPC_Panning_Oboe";
            case "Clarinet": return "NPC_Panning_Clarinet";
            case "Bassoon": return "NPC_Panning_Bassoon";
            case "Celeste": return "NPC_Panning_Celeste";
            case "Vibraphone": return "NPC_Panning_Vibraphone";
            case "Marimba": return "NPC_Panning_Marimba";

            // Midimite instruments (percussion)
            case "Anvil": return "Midimite_Panning_Anvil";
            case "Bamboo": return "Midimite_Panning_Bamboo";
            case "Conga": return "Midimite_Panning_Conga";
            case "Cymbal": return "Midimite_Panning_Cymbal";
            case "Darabuka": return "Midimite_Panning_Darabuka";
            case "HighCowbell": return "Midimite_Panning_HighCowbell";
            case "HighKit": return "Midimite_Panning_HighKit";
            case "KickSnare": return "Midimite_Panning_KickSnare";
            case "LowCowbell": return "Midimite_Panning_LowCowbell";
            case "LowKit": return "Midimite_Panning_LowKit";
            case "Tabla": return "Midimite_Panning_Tabla";
            case "Udu": return "Midimite_Panning_Udu";

            default:
                Debug.LogWarning($"No Panning RTPC mapping for instrument: {instrumentName}");
                return "";
        }
    }
}
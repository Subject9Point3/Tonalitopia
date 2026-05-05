using UnityEngine;
using System.Collections.Generic;

public class InstrumentPairIndicator : MonoBehaviour
{
    public static InstrumentPairIndicator Instance { get; private set; }

    [Header("Indicator Colors")]
    public Color midiMiteDefaultColor = Color.red;
    public Color npcDefaultColor = Color.green;
    public Color highlightColor = new Color(1f, 0.5f, 0f); // Orange

    // Track original instrument holders
    private Dictionary<string, MidiMite> instrumentToMidiMite = new Dictionary<string, MidiMite>();
    private Dictionary<string, Npc> instrumentToNpc = new Dictionary<string, Npc>();

    // Track renderers for color changes
    private Dictionary<MidiMite, Renderer> midiMiteRenderers = new Dictionary<MidiMite, Renderer>();
    private Dictionary<Npc, Renderer> npcRenderers = new Dictionary<Npc, Renderer>();

    // Currently highlighted pair
    private string currentlyHighlightedInstrument = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Called by Spawner when MidiMite is created with an instrument
    /// </summary>
    public void RegisterMidiMite(string instrumentName, MidiMite midiMite)
    {
        instrumentToMidiMite[instrumentName] = midiMite;

        // Find and cache the renderer (adjust path based on your prefab structure)
        var renderer = midiMite.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            midiMiteRenderers[midiMite] = renderer;
        }

        Debug.Log($"Registered MidiMite for instrument: {instrumentName}");
    }

    /// <summary>
    /// Called by Spawner when NPC is created with a required instrument
    /// </summary>
    public void RegisterNpc(string instrumentName, Npc npc)
    {
        instrumentToNpc[instrumentName] = npc;

        // Find and cache the renderer (adjust path based on your prefab structure)
        var renderer = npc.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            npcRenderers[npc] = renderer;
        }

        Debug.Log($"Registered NPC for instrument: {instrumentName}");
    }

    /// <summary>
    /// Called when player picks up an instrument
    /// </summary>
    public void OnPlayerPickedUpInstrument(string instrumentName)
    {
        // Clear any previous highlight
        ClearHighlight();

        // Highlight the new pair
        currentlyHighlightedInstrument = instrumentName;

        if (instrumentToMidiMite.TryGetValue(instrumentName, out var midiMite))
        {
            SetMidiMiteColor(midiMite, highlightColor);
        }

        if (instrumentToNpc.TryGetValue(instrumentName, out var npc))
        {
            SetNpcColor(npc, highlightColor);
        }

        Debug.Log($"Highlighted pair for: {instrumentName}");
    }

    /// <summary>
    /// Called when player drops or gives away an instrument
    /// </summary>
    public void OnPlayerReleasedInstrument()
    {
        ClearHighlight();
    }

    private void ClearHighlight()
    {
        if (string.IsNullOrEmpty(currentlyHighlightedInstrument)) return;

        if (instrumentToMidiMite.TryGetValue(currentlyHighlightedInstrument, out var midiMite))
        {
            SetMidiMiteColor(midiMite, midiMiteDefaultColor);
        }

        if (instrumentToNpc.TryGetValue(currentlyHighlightedInstrument, out var npc))
        {
            SetNpcColor(npc, npcDefaultColor);
        }

        currentlyHighlightedInstrument = null;
    }

    private void SetMidiMiteColor(MidiMite midiMite, Color color)
    {
        if (midiMite == null) return;

        if (midiMiteRenderers.TryGetValue(midiMite, out var renderer))
        {
            renderer.material.color = color;
        }
    }

    private void SetNpcColor(Npc npc, Color color)
    {
        if (npc == null) return;

        if (npcRenderers.TryGetValue(npc, out var renderer))
        {
            renderer.material.color = color;
        }
    }
}
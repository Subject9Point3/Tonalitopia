using UnityEngine;

public class MusicCallbackTest : MonoBehaviour
{
    [Header("Wwise Event")]
    public AK.Wwise.Event masterMusicEvent;

    void Start()
    {
        // Post event with ALL music callback types
        uint callbackFlags =
            (uint)AkCallbackType.AK_MusicSyncBeat |
            (uint)AkCallbackType.AK_MusicSyncBar |
            (uint)AkCallbackType.AK_MusicSyncEntry |
            (uint)AkCallbackType.AK_MusicSyncExit |
            (uint)AkCallbackType.AK_MusicSyncGrid |
            (uint)AkCallbackType.AK_MusicSyncPoint |
            (uint)AkCallbackType.AK_MusicSyncUserCue |
            (uint)AkCallbackType.AK_MusicPlaylistSelect |
            (uint)AkCallbackType.AK_MusicSyncAll;

        AkSoundEngine.PostEvent(
            masterMusicEvent.Id,
            gameObject,
            callbackFlags,
            MusicCallback,
            null
        );

        Debug.Log("Music started with callbacks enabled");
    }

    void MusicCallback(object cookie, AkCallbackType type, AkCallbackInfo callbackInfo)
    {
        Debug.Log($"=== CALLBACK: {type} ===");

        // Try to cast to music-specific callback info
        if (callbackInfo is AkMusicSyncCallbackInfo musicSync)
        {
            Debug.Log($"  Music Sync Info:");
            Debug.Log($"    playingID: {musicSync.playingID}");
            Debug.Log($"    segmentInfo_iCurrentPosition: {musicSync.segmentInfo_iCurrentPosition}");
            Debug.Log($"    segmentInfo_iPreEntryDuration: {musicSync.segmentInfo_iPreEntryDuration}");
            Debug.Log($"    segmentInfo_iActiveDuration: {musicSync.segmentInfo_iActiveDuration}");
            Debug.Log($"    segmentInfo_iPostExitDuration: {musicSync.segmentInfo_iPostExitDuration}");
            Debug.Log($"    segmentInfo_iRemainingLookAheadTime: {musicSync.segmentInfo_iRemainingLookAheadTime}");
            Debug.Log($"    segmentInfo_fBeatDuration: {musicSync.segmentInfo_fBeatDuration}");
            Debug.Log($"    segmentInfo_fBarDuration: {musicSync.segmentInfo_fBarDuration}");
            Debug.Log($"    segmentInfo_fGridDuration: {musicSync.segmentInfo_fGridDuration}");
            Debug.Log($"    segmentInfo_fGridOffset: {musicSync.segmentInfo_fGridOffset}");
            Debug.Log($"    pszUserCueName: {musicSync.userCueName}");
        }

        if (callbackInfo is AkMusicPlaylistCallbackInfo playlistInfo)
        {
            Debug.Log($"  Playlist Info:");
            Debug.Log($"    playingID: {playlistInfo.playingID}");
            Debug.Log($"    playlistID: {playlistInfo.playlistID}");
            Debug.Log($"    uNumPlaylistItems: {playlistInfo.uNumPlaylistItems}");
            Debug.Log($"    uPlaylistSelection: {playlistInfo.uPlaylistSelection}");
            Debug.Log($"    uPlaylistItemDone: {playlistInfo.uPlaylistItemDone}");
        }

        // Log the raw callback info type
        Debug.Log($"  Callback info type: {callbackInfo.GetType().Name}");
    }
}
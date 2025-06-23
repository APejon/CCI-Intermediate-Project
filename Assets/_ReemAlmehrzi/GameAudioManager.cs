using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioEntry
{
    public string key;
    public AudioClip clip;
}

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource player1Source;
    public AudioSource player2Source;
    public AudioSource otherSource;

    [Header("Player1 Sounds")]
    public List<AudioEntry> player1Clips = new List<AudioEntry>();

    [Header("Player2 Sounds")]
    public List<AudioEntry> player2Clips = new List<AudioEntry>();

    [Header("Other Sounds")]
    public List<AudioEntry> otherClips = new List<AudioEntry>();

    private Dictionary<string, (AudioClip clip, AudioSource source)> soundMap = new();

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildSoundMap();
    }

    private void BuildSoundMap()
    {
        foreach (var entry in player1Clips)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.clip != null)
                soundMap[entry.key] = (entry.clip, player1Source);
        }

        foreach (var entry in player2Clips)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.clip != null)
                soundMap[entry.key] = (entry.clip, player2Source);
        }

        foreach (var entry in otherClips)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.clip != null)
                soundMap[entry.key] = (entry.clip, otherSource);
        }
    }

    public void Play(string key)
    {
        if (soundMap.TryGetValue(key, out var soundData))
        {
            soundData.source.PlayOneShot(soundData.clip);
        }
        else
        {
            Debug.LogWarning($"Audio key '{key}' not found.");
        }
    }
}

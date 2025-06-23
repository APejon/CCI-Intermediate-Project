using System.Collections.Generic;
using UnityEngine;


public enum AudioCategory
{
    
    Player1,
    Player2,
    Other
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource uiSource;
    public AudioSource player1Source;
    public AudioSource player2Source;
    public AudioSource otherSource;

    private Dictionary<string, AudioClip> uiClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> player1Clips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> player2Clips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> otherClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddSounds(AudioCategory category, List<KeyValuePair<string, AudioClip>> sounds)
    {
        Dictionary<string, AudioClip> targetDict = GetDictionary(category);

        foreach (var pair in sounds)
        {
            if (!targetDict.ContainsKey(pair.Key))
            {
                targetDict[pair.Key] = pair.Value;
            }
            else
            {
                Debug.LogWarning($"Audio key '{pair.Key}' already exists in {category}. Skipping.");
            }
        }
    }

    public void PlaySound(AudioCategory category, string key)
    {
        Dictionary<string, AudioClip> targetDict = GetDictionary(category);
        AudioSource source = GetAudioSource(category);

        if (targetDict.TryGetValue(key, out AudioClip clip))
        {
            source.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound key '{key}' not found in {category} category.");
        }
    }

    private Dictionary<string, AudioClip> GetDictionary(AudioCategory category)
    {
        switch (category)
        {
            case AudioCategory.UI: return uiClips;
            case AudioCategory.Player1: return player1Clips;
            case AudioCategory.Player2: return player2Clips;
            case AudioCategory.Other: return otherClips;
            default: return otherClips;
        }
    }

    private AudioSource GetAudioSource(AudioCategory category)
    {
        switch (category)
        {
            case AudioCategory.UI: return uiSource;
            case AudioCategory.Player1: return player1Source;
            case AudioCategory.Player2: return player2Source;
            case AudioCategory.Other: return otherSource;
            default: return otherSource;
        }
    }
}

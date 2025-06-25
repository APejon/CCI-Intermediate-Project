using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioDictionary
{
    public List<string> keys = new();
    public List<AudioClip> clips = new();

    public Dictionary<string, AudioClip> ToDictionary()
    {
        Dictionary<string, AudioClip> dict = new();

        for (int i = 0; i < Mathf.Min(keys.Count, clips.Count); i++)
        {
            if (!string.IsNullOrEmpty(keys[i]) && clips[i] != null)
            {
                dict[keys[i]] = clips[i];
            }
        }

        return dict;
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource player1Source;
    public AudioSource player2Source;
    public AudioSource otherSource;

    [Header("Audio Clip Dictionaries")]
    public AudioDictionary player1Sounds;
    public AudioDictionary player2Sounds;
    public AudioDictionary otherSounds;

    // Internal map of key -> (clip, source)
    private Dictionary<string, (AudioClip, AudioSource)> soundMap = new();

    private void Awake()
    {
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
        foreach (var pair in player1Sounds.ToDictionary())
            soundMap[pair.Key] = (pair.Value, player1Source);

        foreach (var pair in player2Sounds.ToDictionary())
            soundMap[pair.Key] = (pair.Value, player2Source);

        foreach (var pair in otherSounds.ToDictionary())
            soundMap[pair.Key] = (pair.Value, otherSource);
    }

    public void Play(string key)
    {
        if (soundMap.TryGetValue(key, out var data))
        {
            data.Item2.PlayOneShot(data.Item1);
        }
        else
        {
            Debug.LogWarning($"Audio key '{key}' not found.");
        }
    }
}

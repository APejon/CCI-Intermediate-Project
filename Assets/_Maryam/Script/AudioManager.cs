using System.Collections;
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

    public void Play(string key, float pitch, float volume)
    {
        if (soundMap.TryGetValue(key, out var data))
        {
            data.Item2.pitch = pitch;
            data.Item2.volume = volume;
            data.Item2.PlayOneShot(data.Item1);
        }
        else
        {
            Debug.LogWarning($"Audio key '{key}' not found.");
        }
    }

    public void PlayWithEcho(string key, float volume, float pitch, float delay,  float decay)
    {
        if (soundMap.TryGetValue(key, out var data))
        {
            data.Item2.pitch = pitch;
            data.Item2.volume = volume;

            StartCoroutine(PlayEcho(key, 5, delay, decay));

            // Disable echo after the sound plays
            // StartCoroutine(DisableEchoAfter(key, data.Item1.length / pitch + 5f));
        }
    }

    private IEnumerator PlayEcho(string key, int repeats, float delay, float decay)
    {
        if (soundMap.TryGetValue(key, out var data))
        {
            float volume = data.Item2.volume;
            data.Item2.PlayOneShot(data.Item1);
            for (int i = 0; i < repeats; i++)
            {
                yield return new WaitForSecondsRealtime(delay);
                data.Item2.Stop();
                volume *= decay;
                data.Item2.volume = volume;
                data.Item2.PlayOneShot(data.Item1);
            }
        }

    }
    
    // private IEnumerator DisableEchoAfter(string key, float time)
    // {
    //     yield return new WaitForSeconds(time);
    //     if (soundMap.TryGetValue(key, out var data))
    //     {
    //         var echo = data.Item2.GetComponent<AudioEchoFilter>();
    //         if (echo != null)
    //             echo.enabled = false;
    //     }
    // }
}

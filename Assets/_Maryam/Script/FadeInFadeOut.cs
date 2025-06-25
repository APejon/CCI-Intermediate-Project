using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FadeInFadeOut : MonoBehaviour
{
    // Singleton reference
    public static FadeInFadeOut Instance { get; private set; }

    public float FadeDuration = 1f;

    [SerializeField] private CanvasGroup _Fader;

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            // Optional: Keep between scenes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_Fader != null)
        {
            _Fader.alpha = 1f;
            FadeOut(); // fade from black at start
        }
    }

    public void FadeIn()
    {
        StartCoroutine(FadeImage(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeImage(1f, 0f));
    }

    public void FadeAndDo(Action actionAfterFade)
    {
        StartCoroutine(FadeAndRun(actionAfterFade));
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        _Fader.blocksRaycasts = true;

        while (timer < FadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / FadeDuration);
            _Fader.alpha = alpha;
            timer += Time.deltaTime;
            yield return null;
        }

        _Fader.alpha = endAlpha;
        _Fader.blocksRaycasts = endAlpha == 1f;
    }

    private IEnumerator FadeAndRun(Action actionAfterFade)
    {
        yield return FadeImage(0f, 1f);     // fade to black
        actionAfterFade?.Invoke();          // run panel logic
        yield return new WaitForSeconds(0.05f); // optional pause
        FadeOut();                          // fade back in
    }
}
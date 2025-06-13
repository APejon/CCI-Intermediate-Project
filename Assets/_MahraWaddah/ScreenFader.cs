using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    public float defaultFadeDuration = 1f;

    private void Awake()
    {
        // Set up singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Fades in a UI GameObject (to alpha 0), using its CanvasGroup.
    /// </summary>
    public void FadeIn(GameObject target, float duration = -1f)
    {
        target.SetActive(true);
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group != null)
        {
            StartCoroutine(FadeCanvasGroup(group, 1f, 0f, duration < 0 ? defaultFadeDuration : duration));
        }
        else
        {
            Debug.LogWarning("FadeIn failed: No CanvasGroup found on " + target.name);
        }
    }

    /// <summary>
    /// Fades out a UI GameObject (to alpha 1), using its CanvasGroup.
    /// </summary>
    public void FadeOut(GameObject target, float duration = -1f)
    {
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group != null)
        {
            StartCoroutine(FadeCanvasGroup(group, 0f, 1f, duration < 0 ? defaultFadeDuration : duration));
        }
        else
        {
            Debug.LogWarning("FadeOut failed: No CanvasGroup found on " + target.name);
        }
        //target.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        group.alpha = from;
        group.blocksRaycasts = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        group.alpha = to;
        group.blocksRaycasts = to > 0f;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoFader : MonoBehaviour
{
    [Header("UI & Timing")]
    public Image logoImage;              // Assign in Inspector
    public float holdDuration = 2f;      // Adjustable duration for holding the logo
    public string nextSceneName;         // Set the name of the next scene in Inspector

    private void Start()
    {
        if (logoImage == null)
        {
            Debug.LogError("Logo Image not assigned.");
            return;
        }

        // Ensure the image starts fully transparent
        Color c = logoImage.color;
        c.a = 0f;
        logoImage.color = c;

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // Fade in
        yield return StartCoroutine(FadeImage(0f, 1f, 1f));

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        yield return StartCoroutine(FadeImage(1f, 0f, 1f));

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeImage(float from, float to, float duration)
    {
        float timer = 0f;
        Color c = logoImage.color;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(from, to, timer / duration);
            logoImage.color = new Color(c.r, c.g, c.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        logoImage.color = new Color(c.r, c.g, c.b, to);
    }
}
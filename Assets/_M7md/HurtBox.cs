using UnityEngine;
using System.Collections;

public class HurtBox : MonoBehaviour
{
    [SerializeField] GameObject otherSprite;
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] Material originalMaterial;
    [SerializeField] Material flashMaterial;
    private float shakeMagnitude;
    private float shakeTimer;
    private float OriginalshakeTimer;
    private Transform originalCamTransform;
    [SerializeField] GameObject Camera;
    [SerializeField] GameManager gameManager;
    [SerializeField] SpriteRenderer lastHitBackground;

    void Start()
    {
        shakeMagnitude = 0.05f; 
        lastHitBackground.enabled = true;
        Color alph = lastHitBackground.color;
        alph.a = 0;
        lastHitBackground.color = alph;
    }

    public void PauseAndShake()
    {
        StartCoroutine(PauseAndShake2());
    }
    
    IEnumerator PauseAndShake2()
    {
        Vector3 originalCamPosition = Camera.transform.localPosition;
        Color originalColor = playerRenderer.color;
        otherSprite.SetActive(true);
        playerRenderer.material = flashMaterial;
        if (gameManager.p1Score == 3 || gameManager.p2Score == 3)
        {
            Color alph = lastHitBackground.color;
            alph.a = 1;
            lastHitBackground.color = alph;
        }
        Time.timeScale = 0f;
        shakeTimer = (gameManager.p1Score == 3 || gameManager.p2Score == 3) ? 2f : 0.5f;
        while (shakeTimer > 0f)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            Camera.transform.localPosition = originalCamPosition + new Vector3(shakeOffset.x, 0f, 0f);
            shakeTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        shakeTimer = (gameManager.p1Score == 3 || gameManager.p2Score == 3) ? 2f : 0.5f;
        while (shakeTimer > 0f)
        {
            if (gameManager.p1Score == 3 || gameManager.p2Score == 3)
            {
                Color alph = lastHitBackground.color;
                alph.a = Mathf.Lerp(0, 1, shakeTimer);
                lastHitBackground.color = alph;
            }
            Time.timeScale = Mathf.Lerp(1, 0, shakeTimer);
            shakeTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        // Camera.transform.localPosition = originalCamPosition;
        playerRenderer.material = originalMaterial;
        otherSprite.SetActive(false);
        Time.timeScale = 1f;
    }

}

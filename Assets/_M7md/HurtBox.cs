using UnityEngine;
using System.Collections;

public class HurtBox : MonoBehaviour
{
    private SpriteRenderer playerRenderer;
    private Material originalMaterial;
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
        playerRenderer = GetComponentInParent<SpriteRenderer>();
        originalMaterial = playerRenderer.material;
        shakeMagnitude = 0.05f; 
        lastHitBackground.enabled = true;
        Color alph = lastHitBackground.color;
        alph.a = 0;
        lastHitBackground.color = alph;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("HitBox")) return;

       StartCoroutine(PauseAndShake2());
    }

    IEnumerator PauseAndShake()
    {
        originalCamTransform = Camera.transform;
        playerRenderer.material = flashMaterial;
        Time.timeScale = 0f;
        if (gameManager.p1Score == 3 || gameManager.p2Score == 3)
            OriginalshakeTimer = 2f;
        else
            OriginalshakeTimer = 0.5f;
        shakeTimer = OriginalshakeTimer;
        while (shakeTimer > 0f)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            Camera.transform.localPosition = Camera.transform.localPosition + new Vector3(shakeOffset.x * 4, 0f, 0f);
            if (gameManager.p1Score == 3 || gameManager.p2Score == 3)
            {
                Color alph = lastHitBackground.color;
                alph.a = Mathf.Lerp(0, 1, shakeTimer);
                Time.timeScale = Mathf.Lerp(1, 0, shakeTimer);
                lastHitBackground.color = alph;
            }
            shakeTimer -= Time.unscaledDeltaTime;
            Camera.transform.localPosition = originalCamTransform.localPosition;
            yield return null;
        }
        shakeTimer = OriginalshakeTimer;
        playerRenderer.material = originalMaterial;
        Time.timeScale = 1f;
    }
    
    IEnumerator PauseAndShake2()
    {
        Vector3 originalCamPosition = Camera.transform.localPosition;
        Color originalColor = playerRenderer.color;
        playerRenderer.color = Color.white;
        // playerRenderer.material = flashMaterial;

        Time.timeScale = 0f;
        shakeTimer = (gameManager.p1Score == 3 || gameManager.p2Score == 3) ? 2f : 0.5f;

        while (shakeTimer > 0f)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            Camera.transform.localPosition = originalCamPosition + new Vector3(shakeOffset.x, 0f, 0f);

            if (gameManager.p1Score == 3 || gameManager.p2Score == 3)
            {
                Color alph = lastHitBackground.color;
                alph.a = Mathf.Lerp(0, 1, shakeTimer);
                Time.timeScale = Mathf.Lerp(1, 0, shakeTimer);
                lastHitBackground.color = alph;
            }

            shakeTimer -= Time.unscaledDeltaTime;
            yield return null;
        }

        Camera.transform.localPosition = originalCamPosition;
        playerRenderer.color = originalColor;
        // playerRenderer.material = originalMaterial;
        Time.timeScale = 1f;
    }

}

using UnityEngine;
using System.Collections;

public class HurtBox : MonoBehaviour
{
    private SpriteRenderer playerRenderer;
    private Material originalMaterial;
    [SerializeField] Material flashMaterial;
    private float shakeMagnitude;
    private float shakeTimer;
    private Transform originalCamTransform;
    [SerializeField] GameObject Camera;
    [SerializeField] GameManager gameManager;
    [SerializeField] SpriteRenderer lastHitBackground;

    void Start()
    {
        playerRenderer = GetComponentInParent<SpriteRenderer>();
        originalMaterial = playerRenderer.material;
        shakeMagnitude = 0.01f;
        lastHitBackground.enabled = true;
        Color alph = lastHitBackground.color;
        alph.a = 0;
        lastHitBackground.color = alph;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("HitBox")) return;

        StartCoroutine(PauseAndShake());
    }

    IEnumerator PauseAndShake()
    {
        originalCamTransform = Camera.transform;
        playerRenderer.material = flashMaterial;
        Time.timeScale = 0f;
        if (gameManager.p1Score == 3 || gameManager.p2Score == 3)
            shakeTimer = 2f;
        else
            shakeTimer = 0.5f;
        while (shakeTimer > 0f)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            Camera.transform.localPosition = Camera.transform.localPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0f);
            if (gameManager.p1Score == 3 || gameManager.p2Score == 3)
            {
                Color alph = lastHitBackground.color;
                alph.a = Mathf.Lerp(0, 1, shakeTimer);
                lastHitBackground.color = alph;
            }
            shakeTimer -= Time.unscaledDeltaTime;
            Camera.transform.localPosition = originalCamTransform.localPosition;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(shakeTimer);
        playerRenderer.material = originalMaterial;
        Time.timeScale = 1f;
        shakeTimer = 0.5f;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VerticalLooper : MonoBehaviour
{
    [Header("Image Settings")]
    public Sprite[] images;
    public float scrollSpeed = 50f;
    public float spacing = 10f;
    public bool preserveAspect = true;

    [Header("Loop Control")]
    public float startPointOffset = 100f;
    public float delayBetweenLoops = 1f;
    public float delayBeforeStart = 1f;

    [Header("Background")]
    public Color backgroundColor = Color.black;

    private RectTransform container;
    private int imageCount;

    private bool isPaused = true;
    private bool delayDone = false;
    private float startTime;
    private float pauseEndTime;

    private float totalScrollHeight = 0f;

    void Start()
    {
        CreateBlackBackground();
        BuildScrollingImages();
        container.anchoredPosition = new Vector2(0, -startPointOffset);

        startTime = Time.realtimeSinceStartup;
        isPaused = true;
        delayDone = false;
    }

    void Update()
    {
        if (!delayDone)
        {
            if (Time.realtimeSinceStartup - startTime >= delayBeforeStart)
            {
                delayDone = true;
                isPaused = false;
                Debug.Log("Initial delay finished. Scrolling begins.");
            }
            return;
        }

        if (isPaused)
        {
            if (Time.realtimeSinceStartup >= pauseEndTime)
            {
                isPaused = false;
                Debug.Log("Loop pause finished. Resuming scroll.");
            }
            return;
        }

        if (container == null) return;

        container.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (container.anchoredPosition.y >= totalScrollHeight)
        {
            container.anchoredPosition -= new Vector2(0, totalScrollHeight);
            isPaused = true;
            pauseEndTime = Time.realtimeSinceStartup + delayBetweenLoops;
            Debug.Log("Reached reset point. Pausing before next loop.");
        }
    }

    void CreateBlackBackground()
    {
        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bg.transform.SetParent(transform, false);
        Image bgImage = bg.GetComponent<Image>();
        bgImage.color = backgroundColor;
        RectTransform rt = bg.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        bg.transform.SetAsFirstSibling();
    }

    void BuildScrollingImages()
    {
        GameObject containerGO = new GameObject("Scroller", typeof(RectTransform));
        containerGO.transform.SetParent(transform, false);
        container = containerGO.GetComponent<RectTransform>();
        container.anchorMin = Vector2.zero;
        container.anchorMax = Vector2.one;
        container.offsetMin = Vector2.zero;
        container.offsetMax = Vector2.zero;

        imageCount = images.Length;
        float currentY = spacing/2;
        float containerWidth = ((RectTransform)transform).rect.width;

        for (int i = 0; i < imageCount * 2; i++)
        {
            Sprite sprite = images[i % imageCount];
            float spriteAspect = sprite.rect.height / sprite.rect.width;
            float targetWidth = containerWidth;
            float targetHeight = targetWidth * spriteAspect;

            GameObject go = new GameObject("Image_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(container, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(targetWidth, targetHeight);
            rt.anchoredPosition = new Vector2(0, -currentY);

            Image img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.preserveAspect = preserveAspect;
            img.raycastTarget = false;

            currentY += targetHeight + spacing;
        }

        totalScrollHeight = currentY / 2f; // Only scroll by the first full set
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (images == null || images.Length == 0) return;

        float heightEstimate = 600f + spacing;
        float totalHeight = heightEstimate * images.Length * 2;
        Vector3 top = transform.position;
        Vector3 bottom = top + Vector3.down * totalHeight;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(top + Vector3.left * 500, top + Vector3.right * 500);
        Gizmos.DrawLine(bottom + Vector3.left * 500, bottom + Vector3.right * 500);
    }
#endif
}

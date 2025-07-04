using UnityEngine;
using UnityEngine.UI;

public class VerticalLooper : MonoBehaviour
{
    [Header("Image Settings")]
    public Sprite[] images;
    public Vector2 imageSize = new Vector2(800, 600);
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
    private float imageHeight;
    private int imageCount;

    private bool isPaused = true;
    private bool delayDone = false;
    private float startTime;
    private float pauseEndTime;

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

        float resetPoint = imageCount * imageHeight;
        if (container.anchoredPosition.y >= resetPoint)
        {
            container.anchoredPosition -= new Vector2(0, resetPoint);
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

        imageHeight = imageSize.y + spacing;
        imageCount = images.Length;

        for (int i = 0; i < imageCount * 2; i++)
        {
            GameObject go = new GameObject("Image_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(container, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = imageSize;
            rt.anchoredPosition = new Vector2(0, -i * imageHeight);

            Image img = go.GetComponent<Image>();
            img.sprite = images[i % imageCount];
            img.preserveAspect = preserveAspect;
            img.raycastTarget = false;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (images == null || images.Length == 0) return;

        float totalHeight = (imageSize.y + spacing) * images.Length * 2;
        Vector3 top = transform.position;
        Vector3 bottom = top + Vector3.down * totalHeight;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(top + Vector3.left * 500, top + Vector3.right * 500);
        Gizmos.DrawLine(bottom + Vector3.left * 500, bottom + Vector3.right * 500);
    }
#endif
}

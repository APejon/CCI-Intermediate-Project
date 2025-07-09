using UnityEngine;
using UnityEngine.UI;

public class VerticalLooper : MonoBehaviour
{
    [Header("Image Settings")]
    public Sprite[] images;
    public float scrollSpeed = 50f;
    public float spacing = 10f;
    public bool preserveAspect = true;

    [Header("Timing")]
    public float delayBeforeStart = 1f;
    public float delayBetweenLoops = 1f;

    [Header("Background")]
    public Color backgroundColor = Color.black;

    private RectTransform container;
    private float totalScrollHeight;
    private bool scrolling = false;
    private float startTime;
    private float loopPauseUntil;
    
    [Header("Start Offset")]
    public float startYOffset = 100f; // Positive = starts lower on screen


    void Start()
    {
        CreateBlackBackground();
        BuildImageLoop();

        container.anchoredPosition = new Vector2(0, startYOffset);
        scrolling = false;
        startTime = Time.time;
    }


    void Update()
    {
        if (!scrolling)
        {
            if (Time.time >= startTime + delayBeforeStart && Time.time >= loopPauseUntil)
            {
                scrolling = true;
            }
            return;
        }

        Vector2 pos = container.anchoredPosition;
        pos.y += scrollSpeed * Time.deltaTime;

        if (pos.y >= totalScrollHeight + startYOffset)
        {
            pos.y -= totalScrollHeight;
            scrolling = false;
            loopPauseUntil = Time.time + delayBetweenLoops;
        }

        container.anchoredPosition = pos;
    }

    void CreateBlackBackground()
    {
        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bg.transform.SetParent(transform, false);
        Image img = bg.GetComponent<Image>();
        img.color = backgroundColor;
        RectTransform rt = bg.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        bg.transform.SetAsFirstSibling();
    }

    void BuildImageLoop()
    {
        GameObject containerGO = new GameObject("ImageContainer", typeof(RectTransform));
        containerGO.transform.SetParent(transform, false);
        container = containerGO.GetComponent<RectTransform>();
        container.anchorMin = new Vector2(0, 0);
        container.anchorMax = new Vector2(1, 1);
        container.pivot = new Vector2(0.5f, 0.5f);
        container.anchoredPosition = Vector2.zero;
        container.offsetMin = Vector2.zero;
        container.offsetMax = Vector2.zero;

        float width = ((RectTransform)transform).rect.width;
        float currentY = 0f;

        for (int i = 0; i < images.Length * 2; i++) // duplicate images for seamless loop
        {
            Sprite sprite = images[i % images.Length];
            float aspect = sprite.rect.height / sprite.rect.width;
            float height = preserveAspect ? width * aspect : sprite.rect.height;

            GameObject imgGO = new GameObject("Img_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imgGO.transform.SetParent(container, false);
            RectTransform rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(width, height);
            rt.anchoredPosition = new Vector2(0, -currentY);

            Image img = imgGO.GetComponent<Image>();
            img.sprite = sprite;
            img.preserveAspect = preserveAspect;
            img.raycastTarget = false;

            currentY += height + spacing;
        }

        totalScrollHeight = currentY / 2f; // only scroll through one set
    }
}

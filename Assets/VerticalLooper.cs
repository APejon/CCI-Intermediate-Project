using UnityEngine;
using UnityEngine.UI;

public class VerticalLooper : MonoBehaviour
{
    [Header("Image Settings")]
    public Sprite[] images;
    public Vector2 imageSize = new Vector2(800, 600);
    public float scrollSpeed = 50f;
    public float spacing = 10f; // New: spacing between images

    [Header("Background")]
    public Color backgroundColor = Color.black; // New: background color
    
    private RectTransform container;
    private float imageHeight;
    private int imageCount;

    void Start()
    {
        CreateBlackBackground();
        BuildScrollingImages();
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

        for (int i = 0; i < imageCount * 2; i++) // double up for looping
        {
            GameObject go = new GameObject("Image_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(container, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = imageSize;
            rt.anchoredPosition = new Vector2(0, -i * imageHeight);

            Image img = go.GetComponent<Image>();
            img.sprite = images[i % imageCount];
            img.preserveAspect = true;
        }
    }

    void Update()
    {
        container.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        float resetPoint = imageCount * imageHeight;

        if (container.anchoredPosition.y >= resetPoint)
            container.anchoredPosition -= new Vector2(0, resetPoint);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || container == null)
        {
            float height = imageSize.y + spacing;
            DrawMarginGizmos(transform as RectTransform, height);
        }
        else
        {
            DrawMarginGizmos(container, imageHeight);
        }
    }

    void DrawMarginGizmos(RectTransform rt, float height)
    {
        Gizmos.color = Color.green;

        Vector3 worldTop = rt.TransformPoint(Vector3.zero);
        Vector3 worldBottom = rt.TransformPoint(new Vector3(0, -imageCount * height * 2, 0));

        Gizmos.DrawLine(worldTop + Vector3.left * 100, worldTop + Vector3.right * 100);
        Gizmos.DrawLine(worldBottom + Vector3.left * 100, worldBottom + Vector3.right * 100);
    }
#endif
}

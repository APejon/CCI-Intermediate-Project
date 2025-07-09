using UnityEngine;
using UnityEngine.UI;

public class txtBoxScroll : MonoBehaviour
{
    //public Text TextToScroll;            // UnityEngine.UI.Text � not TextMesh
    public float speed = 50f;            // Scroll speed in units per second

    public RectTransform rectScrollText;
    public float startY;
    public float endY;
    public float screenHeight;

    void Start()
    {
        //rectScrollText = TextToScroll.GetComponent<RectTransform>();

        // Get height of the screen (in canvas space)
        Canvas canvas = GetComponentInParent<Canvas>();
        screenHeight = canvas.GetComponent<RectTransform>().rect.height;

        float textHeight = rectScrollText.rect.height;

        // Start just below screen
        //startY = -textHeight;
        // End just above screen
        //endY = screenHeight;

        // Apply initial position
        Vector2 startPos = rectScrollText.anchoredPosition;
        startPos.y = startY;
        rectScrollText.anchoredPosition = startPos;
    }

    void Update()
    {
        Vector2 pos = rectScrollText.anchoredPosition;
        pos.y += speed * Time.deltaTime;
        rectScrollText.anchoredPosition = pos;

        // Reset to start if moved off the top
        if (pos.y > endY)
        {
            pos.y = startY;
            rectScrollText.anchoredPosition = pos;
        }
    }
}

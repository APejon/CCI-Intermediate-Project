using UnityEngine;

public class FlashSpriteManager : MonoBehaviour
{
    private SpriteRenderer mainSprite;
    private SpriteRenderer flashSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainSprite = transform.parent.GetComponent<SpriteRenderer>();
        flashSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        flashSprite.sprite = mainSprite.sprite;
    }
}

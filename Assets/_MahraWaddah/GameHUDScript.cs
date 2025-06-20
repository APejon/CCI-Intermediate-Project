using UnityEngine;
using UnityEngine.UI;
public class GameHUDScript : MonoBehaviour
{
    public Text timerText;
    public float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        timerText.text = Time.deltaTime.ToString();
    }
}

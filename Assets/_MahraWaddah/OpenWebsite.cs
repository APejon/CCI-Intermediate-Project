using UnityEngine;

public class OpenWebsite : MonoBehaviour
{
    public string url = "https://example.com"; // Replace with your desired URL

    public void btnOpenWeb()
    {
        Application.OpenURL(url);
    }
}

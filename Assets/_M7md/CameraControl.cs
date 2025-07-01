using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public float minZoom = 4f;
    public float maxZoom = 8f;
    public float zoomSpeed = 10f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        float distance = Vector2.Distance(player1.position, player2.position);

        // Convert distance to zoom size (inverse relationship)
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / 10f); // You can tweak the `/ 10f` scaling
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

        // Optional: Center the camera between players
        Vector3 midPoint = (player1.position + player2.position) / 2f;
        transform.position = new Vector3(midPoint.x, midPoint.y, transform.position.z);
    }
}
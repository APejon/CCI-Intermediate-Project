using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Players")]
    public Transform player1;
    public Transform player2;

    [Header("Zoom Settings")]
    public float minZoom = 4f;
    public float maxZoom = 8f;
    public float zoomSpeed = 10f;

    [Header("Camera Bounds")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!player1 || !player2 || !cam) return;

        // Zoom based on distance
        float distance = Vector2.Distance(player1.position, player2.position);
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / 10f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

        // Center between players
        Vector3 midPoint = (player1.position + player2.position) / 2f;

        // Clamp using camera edges
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float minCamX = minX + horzExtent;
        float maxCamX = maxX - horzExtent;
        float minCamY = minY + vertExtent;
        float maxCamY = maxY - vertExtent;

        float clampedX = Mathf.Clamp(midPoint.x, minCamX, maxCamX);
        float clampedY = Mathf.Clamp(midPoint.y, minCamY, maxCamY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        float width = maxX - minX;
        float height = maxY - minY;
        Vector3 center = new Vector3(minX + width / 2f, minY + height / 2f, 0f);

        Gizmos.DrawWireCube(center, new Vector3(width, height, 0f));
    }
}
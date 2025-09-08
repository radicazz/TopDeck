using UnityEngine;

public class StandardCameraController : MonoBehaviour
{
    private MapController wfc;
    private Vector3 center;
    private float distance = 20f;
    private float minDistance = 5f;
    private float maxDistance = 200f;

    [SerializeField]
    private float zoomSpeed = 5f;

    [SerializeField]
    private float moveSpeed = 3f;

    void Start()
    {
        wfc = FindFirstObjectByType<MapController>();
        if (wfc != null)
        {
            float centerX = (wfc.width * wfc.cellSize) / 2f;
            float centerZ = (wfc.height * wfc.cellSize) / 2f;
            center = new Vector3(centerX, 0f, centerZ);

            // Calculate diagonal distance for isometric view
            float diagonal = Mathf.Sqrt(wfc.width * wfc.width + wfc.height * wfc.height) * wfc.cellSize;
            float initialDistance = diagonal * 0.5f; // Factor to fit diagonally
            initialDistance = Mathf.Clamp(initialDistance, minDistance, maxDistance);

            // Position camera at 45-degree isometric angle
            Vector3 offset = new Vector3(initialDistance * 0.7071f, initialDistance, initialDistance * 0.7071f); // 45-degree angle
            Camera.main.transform.position = center + offset;
            Camera.main.transform.LookAt(center);

            // Set distance to match the actual initial position
            distance = Vector3.Distance(Camera.main.transform.position, center);
        }
    }

    void Update()
    {
        if (wfc == null) return;

        // W/S for zooming in/out, A/D for orbiting around map
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            distance -= zoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            distance += zoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // A/D for orbiting around the center
        float currentAngle = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            currentAngle += moveSpeed * 10f * Time.deltaTime; // Orbit left (reversed)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            currentAngle -= moveSpeed * 10f * Time.deltaTime; // Orbit right (reversed)

        if (currentAngle != 0f)
        {
            // Rotate camera around the center
            Camera.main.transform.RotateAround(center, Vector3.up, currentAngle);
        }

        // Update camera position for zoom changes
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            // Maintain current direction but adjust distance
            Vector3 direction = (Camera.main.transform.position - center).normalized;
            Camera.main.transform.position = center + direction * distance;
        }

        // Always look at center
        Camera.main.transform.LookAt(center);
    }
}

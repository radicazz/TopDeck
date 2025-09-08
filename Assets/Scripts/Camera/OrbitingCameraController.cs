using UnityEngine;

public class OrbitingCameraController : MonoBehaviour
{
    private MapController wfc;
    private Vector3 center;
    private float distance = 15f;
    private float orbitSpeed = 10f; // degrees per second

    void Start()
    {
        wfc = FindFirstObjectByType<MapController>();
        if (wfc != null)
        {
            float centerX = (wfc.width * wfc.cellSize) / 2f;
            float centerZ = (wfc.height * wfc.cellSize) / 2f;
            center = new Vector3(centerX, (wfc.depth * wfc.cellSize) / 2f, centerZ);

            // Set initial camera position at a slight angle
            Camera.main.transform.position = center + new Vector3(0, (wfc.depth * wfc.cellSize) / 2f + 10f, -distance);
            Camera.main.transform.LookAt(center);
        }

        // Mouse cursor remains visible for UI interactions
    }

    void Update()
    {
        if (wfc != null)
        {
            // Continuous orbiting
            Camera.main.transform.RotateAround(center, Vector3.up, orbitSpeed * Time.deltaTime);
            Camera.main.transform.LookAt(center);
        }
    }
}

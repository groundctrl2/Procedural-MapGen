using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public WFCRenderer rendererRef; // WFCRenderer script
    public float padding = 1f; // Extra space around grid

    void Start()
    {
        FitToGrid();
    }

    public void FitToGrid()
    {
        int rows = rendererRef.rows;
        int cols = rendererRef.cols;
        float tileSize = rendererRef.tileSize;

        float width = cols * tileSize;
        float height = rows * tileSize;

        // Center position
        float centerX = (cols - 1) * tileSize / 2f;
        float centerY = -(rows - 1) * tileSize / 2f; // negative Y for top-down

        // Move camera
        transform.position = new Vector3(centerX, centerY, -10f); // -10 for default camera z

        // Resize orthographic size
        Camera cam = GetComponent<Camera>();

        float verticalSize = (height / 2f) + padding;
        float horizontalSize = ((width / cam.aspect) / 2f) + padding;

        cam.orthographic = true;
        cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }
}

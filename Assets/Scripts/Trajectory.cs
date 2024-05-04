using UnityEngine;

/// <summary>
/// This class visualizes a trajectory from a specified fire point in a given direction,
/// </summary>
public class Trajectory : MonoBehaviour
{
    [SerializeField] private Transform pointsParent;        // Parent object for trajectory visualization points
    [SerializeField] private Transform firePoint;           // Point from which the projectile is fired
    [SerializeField] private GameObject trajectoryPointPrefab;  // Prefab for trajectory visualization points

    [SerializeField] private float rayCastMaxDistance = 3f;        // Raycast Maximum distance to visualize trajectory
    [SerializeField] private float spacing = 3f;             // Spacing between trajectory points
    [SerializeField] private LayerMask collisionLayer;       // Layer mask for collision detection

    private Vector3 initialDirection;                        
    private GameObject[] trajectoryPoints;                   
    private int numPoints = 50;                              

    private Color currentColor = Color.white;                 // Current color of trajectory points
    private BlockType detectedBlockType = BlockType.None;     // Detected block type on trajectory

    public BlockType DetectedBlockType { get { return detectedBlockType; } }

    private void Start()
    {
        trajectoryPoints = new GameObject[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            trajectoryPoints[i] = Instantiate(trajectoryPointPrefab, Vector3.zero, Quaternion.identity, pointsParent);
        }
    }

    /// <summary>
    /// Sets the trajectory points position and color based on direction
    /// </summary>
    public void VisualizeTrajectory(Vector3 direction)
    {
        initialDirection = direction;
        Vector3 currentPoint = firePoint.position;
        Vector3 currentDirection = initialDirection;
        int index = -1;  // Index of the trajectory point where a collision occurs

        for (int i = 0; i < numPoints; i++)
        {
            currentPoint += currentDirection * spacing;

            RaycastHit hit;
            // Check for collision along the trajectory path
            if (Physics.Raycast(currentPoint, currentDirection, out hit, rayCastMaxDistance, collisionLayer))
            {
                // Reflect the direction upon collision and update the current point
                currentDirection = Vector3.Reflect(currentDirection, hit.normal).normalized;
                currentPoint = hit.point;
                trajectoryPoints[i].SetActive(true);
                trajectoryPoints[i].transform.position = currentPoint;

                // Get the Block component from the collided object
                Block block = hit.collider.gameObject.GetComponent<Block>();
                if (block != null)
                {
                    index = i + 1; 
                    SetTrajectoryPointColors(block.GetBlockColor());
                    detectedBlockType = block.BlockType;
                    break;
                }
            }
            else
            {
                // No collision detected, continue showing the trajectory point
                trajectoryPoints[i].SetActive(true);
                trajectoryPoints[i].transform.position = currentPoint;
            }
        }

        // Disable remaining trajectory points after the collision point
        if (index != -1)
        {
            for (int i = index; i < trajectoryPoints.Length; i++)
            {
                trajectoryPoints[i].SetActive(false);
            }
        }
        else
        {
            SetTrajectoryPointColors(Color.white);
        }
    }

    /// <summary>
    /// Disables all trajectory visualization points.
    /// </summary>
    public void DisableTrajectory()
    {
        for (int i = 0; i < trajectoryPoints.Length; i++)
        {
            trajectoryPoints[i].SetActive(false);
        }
    }

    /// <summary>
    /// Sets the color of all trajectory points to the specified color.
    /// </summary>
    public void SetTrajectoryPointColors(Color color)
    {
        // Only update colors if the new color is different from the current color
        if (currentColor == color) { return; }

        currentColor = color;
        foreach (GameObject point in trajectoryPoints)
        {
            if (point != null)
            {
                MeshRenderer renderer = point.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
            }
        }
    }
}

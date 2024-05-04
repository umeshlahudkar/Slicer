using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the behavior of a bullet launcher, allowing the player to aim and fire bullets.
/// </summary>
public class BulletLauncher : MonoBehaviour
{
    [SerializeField] private Transform firePoint;       // Point from which bullets are fired
    [SerializeField] private GameObject bulletPrefab;   // Prefab for the bullet projectile
    [SerializeField] private Transform thisTransform;   // Reference to the transform of this GameObject

    [SerializeField] private float rotationSpeed = 50f; // Speed at which the launcher rotates
    [SerializeField] private float maxAngle = 85f;      // Maximum angle the launcher can rotate
    [SerializeField] private float minAngle = -85f;     // Minimum angle the launcher can rotate

    private bool isRotatingWithMouse = false;           // Flag to indicate if rotating with mouse input
    private Vector3 lastMousePosition;                  // Last recorded mouse position for rotation tracking

    [SerializeField] private Trajectory trajectory;     // Reference to the trajectory visualizer component

    private List<Bullet> activeBullets = new List<Bullet>(); // List of currently active bullets
    private bool hasBulletActive = false;                // Flag to track if there's an active bullet

    private void Update()
    {
        // Allow rotation and firing only if there's no active bullet
        if (!hasBulletActive)
        {
            HandleRotationInput();

            if (Input.GetMouseButtonUp(0))
            {
                FireBullet(firePoint.forward);     // Fire bullet in the current facing direction
                trajectory.DisableTrajectory();    // Disable trajectory visualization after firing
            }
        }
    }

    /// <summary>
    /// Handles input for rotating the launcher.
    /// </summary>
    private void HandleRotationInput()
    {
        float rotateInput = Input.GetAxis("Horizontal");

        if (Input.GetMouseButtonDown(0))
        {
            isRotatingWithMouse = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isRotatingWithMouse = false;
        }

        float rotationAmount = 0f;
        if (isRotatingWithMouse)
        {
            // Calculate rotation amount based on mouse movement
            Vector3 currentMousePosition = Input.mousePosition;
            rotationAmount = (currentMousePosition.x - lastMousePosition.x) * rotationSpeed * Time.deltaTime;
            lastMousePosition = currentMousePosition;

            // Visualize trajectory based on the current facing direction
            trajectory.VisualizeTrajectory(firePoint.forward);
        }
        else if (rotateInput != 0)
        {
            // Rotate based on horizontal input (e.g., arrow keys)
            rotationAmount = rotateInput * rotationSpeed * Time.deltaTime;

            // Visualize trajectory based on the current facing direction
            trajectory.VisualizeTrajectory(firePoint.forward);
        }

        thisTransform.Rotate(Vector3.up, rotationAmount);
        ClampRotation();
    }

    /// <summary>
    /// Clamps the rotation of the launcher within specified angle limits.
    /// </summary>
    private void ClampRotation()
    {
        Vector3 currentAngles = thisTransform.localEulerAngles;
        currentAngles.y = Mathf.Repeat(currentAngles.y + 180f, 360f) - 180f;
        currentAngles.y = Mathf.Clamp(currentAngles.y, minAngle, maxAngle);
        thisTransform.localEulerAngles = currentAngles;
    }

    /// <summary>
    /// Fires a bullet in the specified direction from the fire point.
    /// </summary>
    private void FireBullet(Vector3 direction)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            hasBulletActive = true;     // Set flag to indicate an active bullet is fired

            // Instantiate a bullet at the fire point with the specified direction and rotation
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.SetActive(true);
            Bullet bulletMovement = bullet.GetComponentInChildren<Bullet>();
            if (bulletMovement != null)
            {
                bulletMovement.InitBullet(direction, trajectory.DetectedBlockType, this);
                activeBullets.Add(bulletMovement); 
            }
        }
    }

    /// <summary>
    /// Get called when Call Back button pressed
    /// Returns the bullets to the launcher
    /// </summary>
    public void CallBackBullets()
    {
        for (int i = 0; i < activeBullets.Count; i++)
        {
            // Calculate direction towards the launcher's position
            Vector3 direction = thisTransform.position - activeBullets[i].ThisTransform.position;
            activeBullets[i].SetDirection(direction, this.transform.position);
        }

        activeBullets.Clear();
    }

    /// <summary>
    /// Sets the active bullet status to control firing behavior.
    /// </summary>
    public void SetBulletActiveStatus(bool status)
    {
        hasBulletActive = status; 
    }
}
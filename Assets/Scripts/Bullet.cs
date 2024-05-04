using UnityEngine;

/// <summary>
/// Controls the behavior of a bullet projectile.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;  // Reference to the transform of this GameObject
    [SerializeField] private float speed = 10f;        // Speed at which the bullet moves

    private Vector3 moveDirection;          // Current movement direction of the bullet
    private BlockType detectedBlockType;    // Type of block that bullet will destroy
    private Vector3 target = Vector3.zero;  // Target position for the bullet to return to
    private BulletLauncher bulletLauncher;  
    private bool isReturning = false;       // Flag indicating if the bullet is returning to its launcher

    public Transform ThisTransform { get { return thisTransform; } }

    /// <summary>
    /// Initializes the bullet with a direction, detected block type, and reference to the bullet launcher.
    /// </summary>
    public void InitBullet(Vector3 direction, BlockType detectedBlockType, BulletLauncher bulletLauncher)
    {
        moveDirection = direction.normalized;
        moveDirection.y = 0;

        this.detectedBlockType = detectedBlockType; 
        this.bulletLauncher = bulletLauncher;        
    }

    /// <summary>
    /// Sets the bullet's direction when returning to the launcher.
    /// </summary>
    public void SetDirection(Vector3 direction, Vector3 target)
    {
        moveDirection = direction.normalized;
        moveDirection.y = 0;

        this.target = target;    
        isReturning = true;     
    }

    private void Update()
    {
        // Move the bullet in its current direction at a specified speed
        thisTransform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // Rotate the bullet to face its movement direction
        if (moveDirection != Vector3.zero)
        {
            thisTransform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        }

        // Check if the bullet has reached its return target and should be destroyed
        if (target != Vector3.zero && Vector3.Distance(thisTransform.position, target) <= 1)
        {
            Destroy(thisTransform.gameObject);  
            bulletLauncher.SetBulletActiveStatus(false);  // Set the bullet launcher to inactive (no active bullet)
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Block block = other.gameObject.GetComponent<Block>();
        if (block != null && block.BlockType == detectedBlockType)
        {
            GameplayController.Instance.ClearBlock(block); 
        }
        else
        {
            if (isReturning) { return; }  // Ignore collisions when the bullet is returning
            Reflect(other);          
        }
    }

    /// <summary>
    /// Reflects the bullet's direction upon collision with an object.
    /// </summary>
    private void Reflect(Collider other)
    {
        Vector3 closestPoint = other.ClosestPoint(thisTransform.position);
        Vector3 normal = (closestPoint - thisTransform.position).normalized;
        moveDirection = Vector3.Reflect(moveDirection, normal);
        moveDirection = moveDirection.normalized;
        moveDirection.y = 0; 
    }
}
